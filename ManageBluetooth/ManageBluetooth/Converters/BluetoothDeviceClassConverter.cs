using System;
using System.Globalization;

using Android.Bluetooth;

using FontAwesome;

using ManageBluetooth.Converters.Base;

namespace ManageBluetooth.Converters
{
    public class BluetoothDeviceClassConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var deviceClass = (DeviceClass)value;

            switch (deviceClass)
            {
                case DeviceClass.AudioVideoHeadphones:
                    return FontAwesomeIcons.Headphones;
                case DeviceClass.PhoneUncategorized:
                case DeviceClass.PhoneSmart:
                    return FontAwesomeIcons.Mobile;
                case DeviceClass.WearableWristWatch:
                    return FontAwesomeIcons.WatchSmart;
                case DeviceClass.ComputerLaptop:
                    return FontAwesomeIcons.Laptop;
                case DeviceClass.ComputerDesktop:
                    return FontAwesomeIcons.Desktop;
                default:
                    return FontAwesomeIcons.Question;
            }
        }
    }
}