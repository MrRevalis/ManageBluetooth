using System.Reflection;

using ManageBluetooth.Models;

using Plugin.BLE.Abstractions.Contracts;

namespace ManageBluetooth.Converters
{
    public static partial class IDeviceConverter
    {
        private const string AddressPropertyName = "Address";
        public static SimpleBluetoothDevice ConvertToSimpleBluetoothDevice(IDevice device)
        {
            return new SimpleBluetoothDevice
            {
                // DeviceId = device.Id,
                DeviceName = string.IsNullOrEmpty(device.Name) ? GetDeviceMacAddress(device) : device.Name,
                // DeviceClass = (device.NativeDevice as BluetoothDevice).BluetoothClass.DeviceClass,
                // DeviceState = device.State,
            };
        }

        private static string GetDeviceMacAddress(IDevice device)
        {
            PropertyInfo propInfo = device.NativeDevice
                .GetType()
                .GetProperty(AddressPropertyName);

            return propInfo.GetValue(device.NativeDevice, null) as string;
        }
    }
}