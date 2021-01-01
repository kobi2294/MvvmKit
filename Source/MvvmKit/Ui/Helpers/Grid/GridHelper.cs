using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmKit
{
    public static class GridHelper
    {
        #region UniformColumns Property

        public static int GetUniformColumns(Grid obj)
        {
            return (int)obj.GetValue(UniformColumnsProperty);
        }

        public static void SetUniformColumns(Grid obj, int value)
        {
            obj.SetValue(UniformColumnsProperty, value);
        }

        // Using a DependencyProperty as the backing store for UniformColumns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UniformColumnsProperty =
            DependencyProperty.RegisterAttached("UniformColumns", typeof(int), typeof(GridHelper), new PropertyMetadata(1, OnUniformColumnsChanged));

        private static void OnUniformColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newVal = (int)e.NewValue;
            var grid = (Grid)d;

            _invalidateColumns(grid, newVal);
        }

        #endregion

        #region UniformRows Property

        public static int GetUniformRows(Grid obj)
        {
            return (int)obj.GetValue(UniformRowsProperty);
        }

        public static void SetUniformRows(Grid obj, int value)
        {
            obj.SetValue(UniformRowsProperty, value);
        }

        // Using a DependencyProperty as the backing store for UniformRows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UniformRowsProperty =
            DependencyProperty.RegisterAttached("UniformRows", typeof(int), typeof(GridHelper), new PropertyMetadata(1, OnUniformRowsChanged));

        private static void OnUniformRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newVal = (int)e.NewValue;
            var grid = (Grid)d;

            _invalidateRows(grid, newVal);
        }

        #endregion


        private static void _invalidateColumns(Grid grid, int count)
        {
            var all = grid.ColumnDefinitions;

            // first remove all non-star definitions
            for (int i = all.Count-1; i >= 0; i--)
            {
                var def = all[i];
                if (!def.Width.IsStar)
                    all.RemoveAt(i);
            }

            if (all.Count > count)
                all.RemoveRange(count, all.Count - count);

            while (all.Count < count)
                all.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        private static void _invalidateRows(Grid grid, int count)
        {
            var all = grid.RowDefinitions;

            // first remove all non-star definitions
            for (int i = all.Count - 1; i >= 0; i--)
            {
                var def = all[i];
                if (!def.Height.IsStar)
                    all.RemoveAt(i);
            }

            if (all.Count > count)
                all.RemoveRange(count, all.Count - count);

            while (all.Count < count)
                all.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }


    }
}
