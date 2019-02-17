using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MvvmKit
{
    public abstract class ConverterExtension<T> : MarkupExtension, IValueConverter
    {
        public object Value { get; set; }

        public object Parameter { get; set; }

        private Type _getTargetPropertyType(IServiceProvider serviceProvider)
        {
            var ipvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var prop = ipvt.TargetProperty;

            if (prop is DependencyProperty)
            {
                return (prop as DependencyProperty).PropertyType;
            }
            else
            {
                return (prop as PropertyInfo).PropertyType;
            }
        }

        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            var type = _getTargetPropertyType(serviceProvider);
            // since we can also use this extension as value converter, we allow to simply return this instance and run its convert value method
            if (type == typeof(IValueConverter)) return this;

            return (this as IValueConverter).Convert(Value, type, Parameter, CultureInfo.CurrentCulture);
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((T)value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected abstract object Convert(T value, Type TargetType, object parameter, CultureInfo culture);
    }
}
