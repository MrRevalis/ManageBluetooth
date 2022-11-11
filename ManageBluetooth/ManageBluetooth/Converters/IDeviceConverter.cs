using Android.Bluetooth;

using ManageBluetooth.Models;

using Plugin.BLE.Abstractions.Contracts;

namespace ManageBluetooth.Converters
{
    public static class IDeviceConverter
    {
        public static SimpleBluetoothDevice ConvertToSimpleBluetoothDevice(IDevice device)
        {
            return new SimpleBluetoothDevice
            {
                DeviceId = device.Id,
                DeviceName = device.Name,
                DeviceClass = (device.NativeDevice as BluetoothDevice).BluetoothClass.DeviceClass,
                DeviceState = device.State,
            };
        }
    }
}