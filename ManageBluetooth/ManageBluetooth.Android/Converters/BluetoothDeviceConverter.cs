using Android.Bluetooth;

using ManageBluetooth.Droid.Extensions;
using ManageBluetooth.Models;

namespace ManageBluetooth.Droid.Converters
{
    public static class BluetoothDeviceConverter
    {
        public static SimpleBluetoothDevice ConvertToSimpleBluetoothDevice(BluetoothDevice device)
        {
            return new SimpleBluetoothDevice
            {
                DeviceId = device.Address,
                DeviceName = device.GetDeviceName(),
                IsBonded = device.BondState == Bond.Bonded ? true : false,
                DeviceClass = device.GetDeviceType(),
                DeviceState = device.GetDeviceConnectionState()
            };
        }
    }
}