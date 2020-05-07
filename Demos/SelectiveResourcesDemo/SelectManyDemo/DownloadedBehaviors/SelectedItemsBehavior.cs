using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SelectiveResourcesDemo.SelectManyDemo
{
    public static class SelectedItems
    {
        private static readonly DependencyProperty SelectedItemsBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItemsBehavior",
                typeof(SelectedItemsBehavior),
                typeof(ListBox),
                null);

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.RegisterAttached(
                "Items",
                typeof(IList),
                typeof(SelectedItems),
                new PropertyMetadata(null, ItemsPropertyChanged));

        public static void SetItems(ListBox listBox, IList list) { listBox.SetValue(ItemsProperty, list); }
        public static IList GetItems(ListBox listBox) { return listBox.GetValue(ItemsProperty) as IList; }

        private static void ItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as ListBox;
            if (target != null)
            {
                AttachBehavior(target, e.NewValue as IList);
            }
        }

        private static void AttachBehavior(ListBox target, IList list)
        {
            var behavior = target.GetValue(SelectedItemsBehaviorProperty) as SelectedItemsBehavior;
            if (behavior == null)
            {
                behavior = new SelectedItemsBehavior(target, list);
                target.SetValue(SelectedItemsBehaviorProperty, behavior);
            }
        }
    }

    public class SelectedItemsBehavior
    {
        private readonly ListBox _listBox;
        private readonly IList _boundList;

        public SelectedItemsBehavior(ListBox listBox, IList boundList)
        {
            _boundList = boundList;
            _listBox = listBox;
            _listBox.Loaded += OnLoaded;
            _listBox.DataContextChanged += OnDataContextChanged;
            _listBox.SelectionChanged += OnSelectionChanged;

            // Try to attach to INotifyCollectionChanged.CollectionChanged event.
            var notifyCollectionChanged = boundList as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += OnCollectionChanged;
            }
        }

        void UpdateListBoxSelection()
        {
            // Temporarily detach from ListBox.SelectionChanged event
            _listBox.SelectionChanged -= OnSelectionChanged;

            // Synchronize selected ListBox items with bound list
            _listBox.SelectedItems.Clear();
            foreach (var item in _boundList)
            {
                // References in _boundList might not be the same as in _listBox.Items
                var i = _listBox.Items.IndexOf(item);
                if (i >= 0)
                {
                    _listBox.SelectedItems.Add(_listBox.Items[i]);
                }
            }

            // Re-attach to ListBox.SelectionChanged event
            _listBox.SelectionChanged += OnSelectionChanged;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Init ListBox selection
            UpdateListBoxSelection();
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Update ListBox selection
            UpdateListBoxSelection();
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Update ListBox selection
            UpdateListBoxSelection();
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Temporarily deattach from INotifyCollectionChanged.CollectionChanged event.
            var notifyCollectionChanged = _boundList as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
            }

            // Synchronize bound list with selected ListBox items
            _boundList.Clear();
            foreach (var item in _listBox.SelectedItems)
            {
                _boundList.Add(item);
            }

            // Re-attach to INotifyCollectionChanged.CollectionChanged event.
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged += OnCollectionChanged;
            }
        }
    }
}
