using DiffPlex.DiffBuilder.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class ChangeTypeToBrushConverter : IValueConverter
    {
        public Brush Deleted { get; set; }

        public Brush Inserted { get; set; }

        public Brush Modified { get; set; }

        public Brush Imaginary { get; set; }

        public Brush Unchanged { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChangeType ct = ChangeType.Unchanged;
            if (value is ChangeType)
            {
                ct = (ChangeType)value;
            } else if ((value is string valueString) && Enum.TryParse<ChangeType>(valueString, out ct))
            {                
            } else
            {
                return null;
            }

            switch (ct)
            {
                case ChangeType.Unchanged:
                    return Unchanged;
                case ChangeType.Deleted:
                    return Deleted;
                case ChangeType.Inserted:
                    return Inserted;
                case ChangeType.Imaginary:
                    return Imaginary;
                case ChangeType.Modified:
                    return Modified;
                default:
                    break;
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
