﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace MvvmKit
{
    public static class SelectionHelper
    {

        #region SelectedValues property

        public static IEnumerable GetSelectedValues(FasterMultiSelectListBox obj)
        {
            return (IEnumerable)obj.GetValue(SelectedValuesProperty);
        }

        public static void SetSelectedValues(FasterMultiSelectListBox obj, IEnumerable value)
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
            var lb = d as FasterMultiSelectListBox;
            _getBehavior(lb).SetSelectedValues(e.NewValue as IEnumerable);
        }

        #endregion

        #region Command property

        public static ICommand GetCommand(FasterMultiSelectListBox obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(FasterMultiSelectListBox obj, ICommand value)
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
            var lb = d as FasterMultiSelectListBox;
            _getBehavior(lb).SetCommand(e.NewValue as ICommand);
        }

        #endregion

        #region _itemsSource property

        private static IEnumerable _getItemsSource(FasterMultiSelectListBox obj)
        {
            return (IEnumerable)obj.GetValue(_itemsSourceProperty);
        }

        private static void _setItemsSource(FasterMultiSelectListBox obj, IEnumerable value)
        {
            obj.SetValue(_itemsSourceProperty, value);
        }

        private static readonly DependencyProperty _itemsSourceProperty =
            DependencyProperty.RegisterAttached(
                "_itemsSource",
                typeof(IEnumerable),
                typeof(SelectionHelper),
                new PropertyMetadata(null, _onItemsSourceChanged));

        private static void _onItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lb = d as FasterMultiSelectListBox;
            _getBehavior(lb).SetItemsSource(e.NewValue as IEnumerable);
        }

        #endregion

        #region _selectedValuePath property

        private static PropertyPath _getSelectedValuePath(FasterMultiSelectListBox obj)
        {
            return (PropertyPath)obj.GetValue(_selectedValuePathProperty);
        }

        private static void _setSelectedValuePath(FasterMultiSelectListBox obj, PropertyPath value)
        {
            obj.SetValue(_selectedValuePathProperty, value);
        }

        private static readonly DependencyProperty _selectedValuePathProperty =
            DependencyProperty.RegisterAttached(
                "_selectedValuePath",
                typeof(PropertyPath),
                typeof(SelectionHelper),
                new PropertyMetadata(null, _onSelectedValuePathChanged));

        private static void _onSelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lb = d as FasterMultiSelectListBox;
            _getBehavior(lb).SetSelectedValuePath(e.NewValue as PropertyPath);
        }

        #endregion

        #region _behavior property

        private static SelectionHelperBehavior _getBehavior(FasterMultiSelectListBox obj)
        {
            var res = (SelectionHelperBehavior)obj.GetValue(_behaviorProperty);
            if (res == null)
            {
                res = new SelectionHelperBehavior(obj);
                _setBehavior(obj, res);

                var b1 = new Binding()
                {
                    Path = new PropertyPath(ItemsControl.ItemsSourceProperty),
                    Mode = BindingMode.OneWay,
                    Source = obj
                };

                obj.SetBinding(_itemsSourceProperty, b1);

                var b2 = new Binding()
                {
                    Path = new PropertyPath(Selector.SelectedValuePathProperty),
                    Mode = BindingMode.OneWay,
                    Source = obj
                };

                obj.SetBinding(_selectedValuePathProperty, b2);

                res.SetOnCleanup(() =>
                {
                    obj.ClearValue(_itemsSourceProperty);
                    obj.ClearValue(_selectedValuePathProperty);
                    obj.ClearValue(_behaviorProperty);
                });

            }
            return res;
        }

        private static void _setBehavior(FasterMultiSelectListBox obj, SelectionHelperBehavior value)
        {
            obj.SetValue(_behaviorProperty, value);
        }

        // Using a DependencyProperty as the backing store for Behavior.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty _behaviorProperty =
            DependencyProperty.RegisterAttached("_behavior",
                typeof(SelectionHelperBehavior),
                typeof(SelectionHelper), new PropertyMetadata(null));
        #endregion

    }
}
