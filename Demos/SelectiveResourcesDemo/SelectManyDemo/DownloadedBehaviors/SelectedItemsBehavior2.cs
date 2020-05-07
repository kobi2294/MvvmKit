using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SelectiveResourcesDemo.SelectManyDemo
{
    public static class SelectedItemsBahavior2
    {
        public static readonly DependencyProperty SelectedItemsProperty =
              DependencyProperty.RegisterAttached
              ("SelectedItems", typeof(IList), typeof(SelectedItemsBahavior2),
              new PropertyMetadata(default(IList), OnAttach));

        public static void SetSelectedItems(DependencyObject d, IList value)
        {
            d.SetValue(SelectedItemsProperty, value);
        }
        public static IList GetSelectedItems(DependencyObject d)
        {
            return (IList)d.GetValue(SelectedItemsProperty);
        }
        private static void OnAttach(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var multiSelector = d as MultiSelector;
            if (multiSelector != null)
            {
                multiSelector.SelectionChanged += OnSelectionChanged;
                return;
            }
            var listBox = d as ListBox;
            if (listBox != null)
            {
                listBox.SelectionChanged += OnSelectionChanged;
            }
        }
        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = GetSelectedItems((DependencyObject)sender);
            foreach (var item in e.RemovedItems) list.Remove(item);
            foreach (var item in e.AddedItems) list.Add(item);
        }
    }
}
