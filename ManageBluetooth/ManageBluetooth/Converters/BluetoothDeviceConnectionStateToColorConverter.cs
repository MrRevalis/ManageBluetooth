using System;
using System.Globalization;

using ManageBluetooth.Converters.Base;
using ManageBluetooth.Helpers;
using ManageBluetooth.Models.Enum;

using Xamarin.Forms;

namespace ManageBluetooth.Converters
{
    public class BluetoothDeviceConnectionStateToColorConverter : ConverterBase
    {
        private const string LightCyan = "LightCyan";
        private const string DarkCyan = "DarkCyan";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var connectionState = (BluetoothDeviceConnectionStateEnum)value;

            switch (connectionState)
            {
                case
                    BluetoothDeviceConnectionStateEnum.Connected:
                    return ResourceHelpers.GetResource<Color>(DarkCyan);

                case BluetoothDeviceConnectionStateEnum.Disconnected:
                    return Color.Black;

                case BluetoothDeviceConnectionStateEnum.Connecting:
                    return ResourceHelpers.GetResource<Color>(LightCyan);

                case BluetoothDeviceConnectionStateEnum.Disconnecting:
                    return ResourceHelpers.GetResource<Color>(LightCyan);

                default:
                    return Color.Black;
            }
        }
    }
}