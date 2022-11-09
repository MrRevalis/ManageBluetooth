using System;
using System.Globalization;

using ManageBluetooth.Converters.Base;
using ManageBluetooth.Resources;

namespace ManageBluetooth.Converters
{
    internal class BluetoothEnabledToBackgroundColor : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isBluetoothEnabled = (bool?)value ?? false;

            if (isBluetoothEnabled)
            {
                return AppResources.Enabled;
            }

            return AppResources.Disabled;
        }
    }
}
