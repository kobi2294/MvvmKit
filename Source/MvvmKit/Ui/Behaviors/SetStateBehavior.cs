using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace MvvmKit
{
    public class SetStateBehavior: Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
        }

        #region Binding Property

        public object Binding
        {
            get { return (object)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Binding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(object), typeof(SetStateBehavior), new PropertyMetadata(null, OnBindingChanged));

        private static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bhvr = d as SetStateBehavior;
            if (bhvr == null) return;

            bhvr.calcState(true);
        }

        #endregion

        #region Prefix Property

        public string Prefix
        {
            get { return (string)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Prefix.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrefixProperty =
            DependencyProperty.Register("Prefix", typeof(string), typeof(SetStateBehavior), new PropertyMetadata("", OnPrefixChanged));

        private static void OnPrefixChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bhvr = d as SetStateBehavior;
            if (bhvr == null) return;

            bhvr.calcState(true);
        }


        #endregion

        private void calcState(bool useTransitions)
        {
            if (AssociatedObject == null) return;

            if (!AssociatedObject.IsLoaded)
            {
                AssociatedObject.Loaded += _OnLoaded;
                return;
            }

            var bindingText = Binding?.ToString() ?? "";
            var prefix = Prefix ?? "";

            var state = prefix + bindingText;
            var res = ExtendedVisualStateManager.GoToElementState(AssociatedObject, state, useTransitions);
        }

        private void _OnLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= _OnLoaded;
            calcState(false);
        }
    }
}
