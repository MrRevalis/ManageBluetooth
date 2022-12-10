using System;
using System.Globalization;

using ManageBluetooth.Converters.Base;
using ManageBluetooth.Models.Enum;

using MaterialDesign;

namespace ManageBluetooth.Converters
{
    public class BluetoothDeviceClassConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var deviceClass = (BluetoothDeviceTypeEnum)value;

            switch (deviceClass)
            {
                case BluetoothDeviceTypeEnum.Headphones:
                    return MaterialDesignIcons.Headphones;

                case BluetoothDeviceTypeEnum.TV:
                    return MaterialDesignIcons.Tv;

                case BluetoothDeviceTypeEnum.Laptop:
                    return MaterialDesignIcons.Laptop;

                case BluetoothDeviceTypeEnum.Computer:
                    return MaterialDesignIcons.Computer;

                default:
                    return MaterialDesignIcons.DevicesOther;
            }
        }
    }
}