using System;
using System.Globalization;

using FontAwesome;

using ManageBluetooth.Converters.Base;
using ManageBluetooth.Models.Enum;

namespace ManageBluetooth.Converters
{
    public class BluetoothDeviceClassConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var deviceClass = (BluetoothDeviceTypeEnum)value;

            switch (deviceClass)
            {
                default:
                    return FontAwesomeIcons.Question;
            }
        }
    }
}