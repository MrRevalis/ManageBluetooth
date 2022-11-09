using System;
using System.Globalization;

using Xamarin.Forms;

namespace ManageBluetooth.Converters.Base
{
    public class ConverterBase : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return default(object);
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}