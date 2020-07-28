using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmKit
{
    public static class ScrollSync
    {
        private static Dictionary<ScrollViewer, string> _scrollViewers = new Dictionary<ScrollViewer, string>();

        private static Dictionary<string, double> _horizontalScrollOffsets = new Dictionary<string, double>();

        private static Dictionary<string, double> _verticalScrollOffsets = new Dictionary<string, double>();


        public static string GetGroup(ScrollViewer obj)
        {
            return (string)obj.GetValue(GroupProperty);
        }

        public static void SetGroup(ScrollViewer obj, string value)
        {
            obj.SetValue(GroupProperty, value);
        }

        // Using a DependencyProperty as the backing store for Group.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.RegisterAttached("Group", 
                typeof(string), typeof(ScrollSync), 
                new PropertyMetadata(null, OnGroupChanged));

        private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer == null) return;

            var oldValue = (string)e.OldValue;
            var newValue = (string)e.NewValue;

            if (oldValue.HasAnyText())
            {
                scrollViewer.ScrollChanged -= OnScrollChanged;
                scrollViewer.Unloaded -= OnScrollUnloaded;
                _scrollViewers.Remove(scrollViewer);
            }

            if (newValue.HasAnyText())
            {
                if (_horizontalScrollOffsets.ContainsKey(newValue))
                {
                    scrollViewer.ScrollToHorizontalOffset(_horizontalScrollOffsets[newValue]);
                } else
                {
                    _horizontalScrollOffsets.Add(newValue, scrollViewer.HorizontalOffset);
                }

                if (_verticalScrollOffsets.ContainsKey(newValue))
                {
                    scrollViewer.ScrollToHorizontalOffset(_verticalScrollOffsets[newValue]);
                }
                else
                {
                    _verticalScrollOffsets.Add(newValue, scrollViewer.HorizontalOffset);
                }

                _scrollViewers.Add(scrollViewer, newValue);
                scrollViewer.ScrollChanged += OnScrollChanged;
                scrollViewer.Unloaded += OnScrollUnloaded;
            }


        }

        private static void OnScrollUnloaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;

            SetGroup(scrollViewer, null);
        }

        private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if ((e.VerticalChange == 0) && (e.HorizontalChange == 0)) return;

            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;

            var group = _scrollViewers[scrollViewer];
            _verticalScrollOffsets[group] = scrollViewer.VerticalOffset;
            _horizontalScrollOffsets[group] = scrollViewer.HorizontalOffset;

            var affectedScrollViewers = _scrollViewers
                .Where(p => (p.Value == group) && (p.Key != scrollViewer))
                .Select(p => p.Key);

            foreach (var sv in affectedScrollViewers)
            {
                if (sv.VerticalOffset != scrollViewer.VerticalOffset)
                {
                    sv.ScrollToVerticalOffset(scrollViewer.VerticalOffset);
                }
                if (sv.HorizontalOffset != scrollViewer.HorizontalOffset)
                {
                    sv.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset);
                }
            }
        }
    }
}
