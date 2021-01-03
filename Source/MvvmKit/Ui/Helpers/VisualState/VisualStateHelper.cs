using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmKit
{
    public static class VisualStateHelper
    {

        #region Binding Property

        public static object GetBinding(FrameworkElement obj)
        {
            return (object)obj.GetValue(BindingProperty);
        }

        public static void SetBinding(FrameworkElement obj, object value)
        {
            obj.SetValue(BindingProperty, value);
        }

        // Using a DependencyProperty as the backing store for Binding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.RegisterAttached("Binding", typeof(object), typeof(VisualStateHelper), new PropertyMetadata(null, OnBindingChanged));

        private static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null) calcState(element, true);
        }

        #endregion


        #region Prefix Property

        public static string GetPrefix(FrameworkElement obj)
        {
            return (string)obj.GetValue(PrefixProperty);
        }

        public static void SetPrefix(FrameworkElement obj, string value)
        {
            obj.SetValue(PrefixProperty, value);
        }

        public static readonly DependencyProperty PrefixProperty =
            DependencyProperty.RegisterAttached("Prefix", typeof(string), typeof(VisualStateHelper), new PropertyMetadata("", OnPrefixChanged));

        private static void OnPrefixChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            if (element != null) calcState(element, true);
        }

        #endregion


        private static void calcState(FrameworkElement element, bool useTransitions)
        {
            if (element == null) return;

            if (!element.IsLoaded)
            {
                element.Loaded += _OnLoaded;
                return;
            }

            var bindingText = GetBinding(element)?.ToString() ?? "";
            var prefix = GetPrefix(element) ?? "";

            var state = prefix + bindingText;
            var res = ExtendedVisualStateManager.GoToElementState(element, state, useTransitions);

        }

        private static void _OnLoaded(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem == null) return;

            elem.Loaded -= _OnLoaded;
            calcState(elem, false);
        }
    }
}
