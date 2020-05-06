using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MvvmKit
{
    public class BindingEvaluator : DependencyObject
    {
        public object Target
        {
            get { return (object)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(object), typeof(BindingEvaluator));

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
    }
}
