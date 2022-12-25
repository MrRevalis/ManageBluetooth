using Android.Bluetooth;

using Java.Lang;

using ManageBluetooth.Droid.Models.Constants;
using ManageBluetooth.Models.Enum;

namespace ManageBluetooth.Droid.Extensions
{
    public static class BluetoothDeviceExtensions
    {
        public static string GetDeviceName(this BluetoothDevice device)
        {
            return !string.IsNullOrEmpty(device.Alias)
                ? device.Alias
                : !string.IsNullOrEmpty(device.Name)
                    ? device.Name
                    : device.Address;
        }

        public static BluetoothDeviceConnectionStateEnum GetDeviceConnectionState(this BluetoothDevice device)
        {
            if (device.GetConnectionState())
            {
                return BluetoothDeviceConnectionStateEnum.Connected;
            }
            return BluetoothDeviceConnectionStateEnum.Disconnected;
        }

        public static BluetoothDeviceTypeEnum GetDeviceType(this BluetoothDevice device)
        {
            switch (device.BluetoothClass.DeviceClass)
            {
                case DeviceClass.AudioVideoWearableHeadset:
                case DeviceClass.AudioVideoHeadphones:
                    return BluetoothDeviceTypeEnum.Headphones;
                case DeviceClass.AudioVideoVideoDisplayAndLoudspeaker:
                    return BluetoothDeviceTypeEnum.TV;
                case DeviceClass.ComputerLaptop:
                    return BluetoothDeviceTypeEnum.Laptop;
                case DeviceClass.ComputerDesktop:
                    return BluetoothDeviceTypeEnum.Computer;
                default:
                    return BluetoothDeviceTypeEnum.Unknown;
            }
        }

        public static bool GetConnectionState(this BluetoothDevice device)
        {
            try
            {
                var m = device.Class.GetMethod(Constants.BluetoothDeviceMethodNames.IsConnected, (Class[])null);
                var connected = (bool)m.Invoke(device, (Object[])null);
                return connected;
            }
            catch (Exception e)
            {
                throw new IllegalStateException(e);
            }
        }
    }
}