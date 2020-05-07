using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MvvmKit
{
    public static class SelectionHelper
    {
        #region ItemsSource property

        public static IEnumerable GetItemsSource(ListBox obj)
        {
            return (IEnumerable)obj.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(ListBox obj, IEnumerable value)
        {
            obj.SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(SelectionHelper),
                new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lb = d as ListBox;
            GetBehavior(lb).SetItemsSource(e.NewValue as IEnumerable);
        }

        #endregion

        #region SelectedValues property

        public static IEnumerable GetSelectedValues(ListBox obj)
        {
            return (IEnumerable)obj.GetValue(SelectedValuesProperty);
        }

        public static void SetSelectedValues(ListBox obj, IEnumerable value)
        {
            obj.SetValue(SelectedValuesProperty, value);
        }

        public static readonly DependencyProperty SelectedValuesProperty =
            DependencyProperty.RegisterAttached(
                "SelectedValues",
                typeof(IEnumerable),
                typeof(SelectionHelper),
                new PropertyMetadata(null, OnSelectedValuesChanged));

        private static void OnSelectedValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lb = d as ListBox;
            GetBehavior(lb).SetSelectedValues(e.NewValue as IEnumerable);
        }

        #endregion

        #region Command property

        public static ICommand GetCommand(ListBox obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(ListBox obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(SelectionHelper),
                new PropertyMetadata(null, OnCommandChanged));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lb = d as ListBox;
            GetBehavior(lb).SetCommand(e.NewValue as ICommand);
        }

        #endregion

        #region SelectedValuePath property

        public static PropertyPath GetSelectedValuePath(ListBox obj)
        {
            return (PropertyPath)obj.GetValue(SelectedValuePathProperty);
        }

        public static void SetSelectedValuePath(ListBox obj, PropertyPath value)
        {
            obj.SetValue(SelectedValuePathProperty, value);
        }

        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.RegisterAttached(
                "SelectedValuePath",
                typeof(PropertyPath),
                typeof(SelectionHelper),
                new PropertyMetadata(null, OnSelectedValuePathChanged));

        private static void OnSelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lb = d as ListBox;
            GetBehavior(lb).SetSelectedValuePath(e.NewValue as PropertyPath);
        }

        #endregion


        private static SelectionHelperBehavior GetBehavior(ListBox obj)
        {
            var res = (SelectionHelperBehavior)obj.GetValue(BehaviorProperty);
            if (res == null)
            {
                res = new SelectionHelperBehavior(obj);
                SetBehavior(obj, res);
            }
            return res;
        }

        private static void SetBehavior(ListBox obj, SelectionHelperBehavior value)
        {
            obj.SetValue(BehaviorProperty, value);
        }

        // Using a DependencyProperty as the backing store for Behavior.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.RegisterAttached("_Behavior", 
                typeof(SelectionHelperBehavior), 
                typeof(SelectionHelper), new PropertyMetadata(null));


    }
}
