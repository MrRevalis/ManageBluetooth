using System;
using System.Globalization;
using System.Linq;

using ManageBluetooth.Converters.Base;
using ManageBluetooth.Custom.Controls;
using ManageBluetooth.Models;

namespace ManageBluetooth.Converters
{
    public class SeparatorVisibilityConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var devicesList = (parameter as BluetoothDevicesList).Devices;

            if (value == null)
                return false;

            var simpleBluetoothDevice = value as SimpleBluetoothDevice;

            return devicesList.Last() != simpleBluetoothDevice;
        }
    }
}