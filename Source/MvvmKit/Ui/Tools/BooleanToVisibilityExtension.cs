using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmKit
{
    public class BooleanToVisibilityExtension : ConverterExtension<bool>
    {
        public bool IsInverted { get; set; }

        public BooleanToVisibilityExtension()
        {
        }

        public BooleanToVisibilityExtension(bool isInverted)
        {
            IsInverted = isInverted;
        }

        protected override object Convert(bool value, Type TargetType, object parameter, CultureInfo culture)
        {
            if (IsInverted) value = !value;

            return value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
