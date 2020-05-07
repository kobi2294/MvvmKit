using MvvmKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MvvmKit
{
    /// <summary>
    /// Follows a list of items and list of values and synchronizes the listbox selected items and the list of values.
    /// 
    /// We respond to the following events:
    /// A - ItemsSource
    ///     A1. ItemsSource replaced (new items to select from)
    ///         - Remove all old items
    ///         - Dispose path notifiers
    ///         - dettach INCC
    ///         - Add all new items
    ///         - attach INCC
    ///         - Create new notifiers
    ///     A2. ItemsSource changed INCC events
    ///         - removed all removed items
    ///         - dispose path notifiers
    ///         - add all added items
    ///         - create new notifiers
    ///     A3. Item key changed (the property that we treat as "key" - its value changed)
    ///         - modify key in lookups
    ///         
    /// After all A events, we do this:
    ///     - Apply on list box
    /// 
    /// B - SelectedValues
    ///     B1. List replaced
    ///        - Dettach INCC of old list
    ///        - Attach INCC of new list
    ///     B2. INCC events
    ///     
    ///     - Apply on list box
    /// 
    /// C - SelectedValuePath changed
    ///     - Dispose all old notifiers
    ///     - Recalculate lookups
    ///     - Apply on list box
    /// 
    /// All these events cause us to modify the list box SelectedItems and ignore D while doing so
    ///    
    /// 
    /// D - ListBox.SelectedItems 
    ///     D1. Initial event - First scan of the entire list
    ///     D2. INCC events
    ///     
    /// This will cause us to fire the command
    /// 
    /// 
    ///
    /// 
    /// </summary>
    public class SelectionHelperBehavior
    {

        private ListBox _listBox = null;

        private IEnumerable _itemsSource = null;
        private IEnumerable _selectedValues = null;
        private PropertyPath _selectedValuePath = null;
        private ICommand _command = null;
        private Func<IEnumerable, object> _cmdCast = null;

        Dictionary<object, BindingNotifyier> _itemToNotifier;
        EditableLookup<object, object> _keyToItems;
        Dictionary<object, object> _itemToKey;


        public SelectionHelperBehavior(ListBox listBox)
        {
            _listBox = listBox;

            _itemToNotifier = new Dictionary<object, BindingNotifyier>();
            _keyToItems = new EditableLookup<object, object>();
            _itemToKey = new Dictionary<object, object>();

            ListBoxSelectedItems_Initialized();
        }

        #region Interface to attached properties to set members

        public void SetItemsSource(IEnumerable itemsSource)
        {
            var oldValue = _itemsSource;
            _itemsSource = itemsSource;
            ItemsSource_Replaced(oldValue, itemsSource);
        }

        public void SetSelectedValues(IEnumerable selectedValues)
        {
            var oldValue = _selectedValues;
            _selectedValues = selectedValues;
            SelectedValues_CollectionReplaced(oldValue, selectedValues);
        }

        public void SetSelectedValuePath(PropertyPath selectedValuesPath)
        {
            var oldValue = _selectedValuePath;
            _selectedValuePath = selectedValuesPath;
            SelectedValuePatch_Changed(oldValue, selectedValuesPath);
        }

        public void SetCommand(ICommand command)
        {
            _command = command;
            if (_command == null) return;

            var type = _command.GetType();
            _cmdCast = null;

            var iRxCmdT = type
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.IsGenericOf(typeof(IRxCommand<>)));

            if (iRxCmdT == null) return;
            var iEnumT = iRxCmdT.GenericTypeArguments[0];

            // if the type argumnet is IEnumerable itself, good, otherwise we check if it implements IEnumerable<>
            if (!iEnumT.IsGenericOf(typeof(IEnumerable<>)))
                iEnumT = iEnumT
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.IsGenericOf(typeof(IEnumerable<>)));

            if (iEnumT == null) return;
            // ok, if we are here, the command is IRxCommand<IEnumerable<T>> so we reserve T

            var cmdTypeArg = iEnumT.GenericTypeArguments[0];
            var castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
            var castGenericMethod = castMethod.MakeGenericMethod(new Type[] { cmdTypeArg });
            _cmdCast = castGenericMethod.ToFunc<IEnumerable, object>();
        }

        #endregion

        #region Basic Actions

        /// <summary>
        /// update the lookups with the new item
        /// 1. create a notifier
        /// 2. add to item -> notifier dictionary
        /// 3. add to key -> items lookup
        /// 4. add to item -> key dictionary
        /// 5. attach to notifier changed event
        /// </summary>
        void _addItemToLookups(object item)
        {
            var notifier = _selectedValuePath.CreateNotifierOn(item);
            _itemToNotifier.Add(item, notifier);
            var key = notifier.Value;
            _keyToItems.Add(key, item);
            _itemToKey.Add(item, key);
            notifier.Changed += Notifier_Changed;
        }

        /// <summary>
        /// update the lookups with the new item
        /// 5. dettach notifier from event handler
        /// 4. remove from item -> key dictionary
        /// 3. remove from key-> items lookup
        /// 2. remove from item -> notifier dictionary
        /// 1. dispose notifier
        /// </summary>
        void _removeItemFromLookups(object item)
        {
            var key = _itemToKey[item];
            var notifier = _itemToNotifier[item];

            notifier.Changed -= Notifier_Changed;
            _itemToKey.Remove(item);
            _keyToItems.Remove(key, item);
            _itemToNotifier.Remove(item);
            notifier.Dispose();
        }

        /// <summary>
        /// remove from key->items lookup
        /// replace item -> key entry
        /// no need to change item->notifier - they are both unchanged
        /// add new key to key -> items lookup
        /// </summary>
        void _itemKeyChanged(object item, object oldKey, object newKey)
        {
            _keyToItems.Remove(oldKey, item);
            _itemToKey[item] = newKey;
            _keyToItems.Add(newKey, item);
        }


        /// <summary>
        /// Add all new items to lookup (inclufing notifiers)
        /// attach to INCC
        /// </summary>
        void _connectToNewItemsSource(IEnumerable itemsSource)
        {
            foreach (var item in itemsSource)
            {
                _addItemToLookups(item);
            }

            if (itemsSource is INotifyCollectionChanged)
            {
                (itemsSource as INotifyCollectionChanged).CollectionChanged += ItemsSource_CollectionChanged;
            }
        }

        /// <summary>
        /// dettach from INCC
        /// remove all items from lookups (including notifiers)
        /// </summary>
        void _disconnectFromOldItemsSource(IEnumerable itemsSource)
        {
            if (itemsSource is INotifyCollectionChanged)
            {
                (itemsSource as INotifyCollectionChanged).CollectionChanged -= ItemsSource_CollectionChanged;
            }

            foreach (var item in itemsSource)
            {
                _removeItemFromLookups(item);
            }
        }

        /// <summary>
        /// attach to INCC
        /// </summary>
        void _connectToNewSelectedValuesList(IEnumerable selectedValues)
        {
            if (selectedValues is INotifyCollectionChanged)
            {
                (selectedValues as INotifyCollectionChanged).CollectionChanged += SelectedValues_CollectionChanged;
            }
        }

        /// <summary>
        /// dettach from INCC
        /// </summary>
        void _disconnectFromOldSelectedValuesList(IEnumerable selectedValues)
        {
            if (selectedValues is INotifyCollectionChanged)
            {
                (selectedValues as INotifyCollectionChanged).CollectionChanged -= SelectedValues_CollectionChanged;
            }
        }

        /// <summary>
        /// Empty all lookups
        /// Refill them again considering the new value path
        /// </summary>
        /// <param name="newPath"></param>
        void _replaceSelectedValuePath(PropertyPath newPath)
        {
            foreach (var item in _itemToKey.Keys.ToArray())
            {
                _removeItemFromLookups(item);
            }
            foreach (var item in _itemToKey.Keys.ToArray())
            {
                _addItemToLookups(item);
            }
        }

        void _attachToListBox()
        {
            _listBox.SelectionChanged += ListBox_SelectionChanged;
            _listBox.Unloaded += ListBox_Unloaded;
        }

        void _dettachFromListBox()
        {
            _listBox.SelectionChanged -= ListBox_SelectionChanged;
            _listBox.Unloaded -= ListBox_Unloaded;
        }

        IEnumerable<object> _calcSelectedItemsFromSelectedValues()
        {
            if (_selectedValues == null) yield break;
            foreach (var key in _selectedValues.Cast<object>().Distinct())
            {
                if (_keyToItems.Contains(key))
                {
                    foreach (var item in _keyToItems[key])
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the listbox on the currently selected items
        /// </summary>
        void _applyOnListBox()
        {
            if ((_listBox == null) || (_listBox.SelectedItems == null)) return;

            var selectedItems = _calcSelectedItemsFromSelectedValues().ToArray();

            var diff = (_listBox.SelectedItems as ObservableCollection<object>).Diff(selectedItems.Cast<object>());
            if (diff.Added.Any() || diff.Removed.Any() || diff.Reset)
            {
                _dettachFromListBox();

                if (_listBox is FasterMultiSelectListBox mlb)
                {
                    mlb.SetSelectedItems(selectedItems);
                } else
                {
                    (_listBox.SelectedItems as ObservableCollection<object>).ApplyDiff(diff);
                }
                _attachToListBox();
            }

        }


        IEnumerable<object> _calcSelectedValuesFromListBoxSelectedItems()
        {
            if ((_listBox == null) || (_listBox.SelectedItems == null)) yield break;
            foreach (var item in _listBox.SelectedItems)
            {
                if (_itemToKey.ContainsKey(item))
                    yield return _itemToKey[item];
            }
        }

        /// <summary>
        /// Gets the currently selected items in the listbox and applies them on the command
        /// </summary>
        void _applyOnCommand()
        {
            if (_command == null) return;
            IEnumerable selectedValues = _calcSelectedValuesFromListBoxSelectedItems().ToArray();

            if (_cmdCast == null)
            {
                _command.Execute(selectedValues);
            }
            else
            {
                // special case where the command is IRxCommand<IEnumerable<T>> and we know T, so we cast
                var casted = _cmdCast(selectedValues);
                _command.Execute(casted);
            }
            

        }

        #endregion

        #region Event Handlers

        // event handlers
        // A1. ItemsSource replaced (new items to select from)
        private void ItemsSource_Replaced(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue != null)
            {
                _disconnectFromOldItemsSource(oldValue);
            }

            if (newValue != null)
            {
                _connectToNewItemsSource(newValue);
            }
            _applyOnListBox();
        }

        // A2. Items Source collection change event
        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems?.Cast<object>()?.ToArray()??Enumerable.Empty<object>())
                    {
                        _removeItemFromLookups(item);
                    }
                    foreach (var item in e.NewItems?.Cast<object>()?.ToArray() ?? Enumerable.Empty<object>())
                    {
                        _addItemToLookups(item);
                    }
                    _applyOnListBox();
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (var item in _itemToKey.Keys.ToArray())
                    {
                        _removeItemFromLookups(item);
                    }
                    _applyOnListBox();
                    break;
                default:
                    break;
            }
        }


        // A3. Item key changed
        private void Notifier_Changed(object sender, (object oldValue, object newValue) e)
        {
            var item = sender;
            var oldKey = e.oldValue;
            var newKey = e.newValue;

            _itemKeyChanged(item, oldKey, newKey);
            _applyOnListBox();
        }

        // B1. Selected Values Collection replaced
        private void SelectedValues_CollectionReplaced(IEnumerable oldValue, IEnumerable newValue)
        {
            _disconnectFromOldSelectedValuesList(oldValue);
            _connectToNewSelectedValuesList(newValue);
            _applyOnListBox();
        }

        // B2. Selected Values collection change event
        private void SelectedValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _applyOnListBox();
        }

        // C2. SelectedValuePath changed 
        private void SelectedValuePatch_Changed(PropertyPath oldValue, PropertyPath newValue)
        {
            _replaceSelectedValuePath(newValue);
            _applyOnListBox();
        }

        // D1. ListBox.SelectedItems - Initialize
        private void ListBoxSelectedItems_Initialized()
        {
            _attachToListBox();
        }

        // D2. ListBox.SelectedItems - Collection Changed Event
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _applyOnCommand();
        }

        // D3. ListBox - Unloaded
        private void ListBox_Unloaded(object sender, RoutedEventArgs e)
        {
            _dettachFromListBox();
        }



        #endregion


    }
}
