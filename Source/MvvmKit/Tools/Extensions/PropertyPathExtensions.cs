using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MvvmKit
{
    internal class BindingEvaluator : DependencyObject
    {
        internal event EventHandler<(object oldValue, object newValue)> Changed;

        internal object Target
        {
            get { return (object)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(object), typeof(BindingEvaluator), new PropertyMetadata(null, OnTargetChanged));

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var be = d as BindingEvaluator;

            be.invoke(e.OldValue, e.NewValue);
        }

        private void invoke(object oldValue, object newValue)
        {
            Changed?.Invoke(this, (oldValue, newValue));
        }
    }

    public class BindingNotifyier: BaseDisposable
    {
        public event EventHandler<(object oldValue, object newValue)> Changed;
        public object Value { get; private set; }

        private BindingEvaluator _eval;
        private Binding _binding;
        private object _source;

        public BindingNotifyier(PropertyPath path, object source)
        {
            _source = source;
            _binding = new Binding();
            _binding.FallbackValue = null;
            _binding.Source = source;
            _binding.Path = path;
            _binding.Mode = BindingMode.OneWay;
            _eval = new BindingEvaluator();
            _eval.Changed += OnTargetChanged;
            BindingOperations.SetBinding(_eval, BindingEvaluator.TargetProperty, _binding);
            Value = _eval.Target;
        }

        private void OnTargetChanged(object sender, (object oldValue, object newValue) e)
        {
            Value = e.newValue;
            Changed?.Invoke(_source, e);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            Changed = null;
            BindingOperations.ClearBinding(_eval, BindingEvaluator.TargetProperty);
            _eval.Changed -= OnTargetChanged;
            _eval = null;
            _binding = null;
            _source = null;
            Value = null;
        }
    }

    public static class PropertyPathExtensions
    {
        public static T Evaluate<T>(this PropertyPath path, object source)
        {
            T res = default(T);

            try
            {
                var binding = new Binding();
                binding.FallbackValue = default(T);
                binding.Source = source;
                binding.Path = path;
                binding.Mode = BindingMode.OneTime;

                var eval = new BindingEvaluator();
                BindingOperations.SetBinding(eval, BindingEvaluator.TargetProperty, binding);

                res = (T)eval.Target;
            }
            catch { }

            return res;
        }

        public static object Evaluate(this PropertyPath path, object source)
        {
            object res = null;
            try
            {
                var binding = new Binding();
                binding.FallbackValue = null;
                binding.Source = source;
                binding.Path = path;
                binding.Mode = BindingMode.OneTime;

                var eval = new BindingEvaluator();
                BindingOperations.SetBinding(eval, BindingEvaluator.TargetProperty, binding);

                res = eval.Target;
            }
            catch { }

            return res;


        }

        public static BindingNotifyier CreateNotifierOn(this PropertyPath path, object source)
        {
            var notifier = new BindingNotifyier(path, source);
            return notifier;
        }
    }
}
