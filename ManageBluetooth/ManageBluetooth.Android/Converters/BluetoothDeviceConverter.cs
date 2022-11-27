using Android.App;
using Android.Bluetooth;
using Android.Content;

using Java.Lang;

using ManageBluetooth.Models;
using ManageBluetooth.Models.Enum;

namespace ManageBluetooth.Droid.Converters
{
    public static class BluetoothDeviceConverter
    {
        private static readonly BluetoothManager _bluetoothManager;

        static BluetoothDeviceConverter()
        {
            _bluetoothManager = Application.Context.GetSystemService(Context.BluetoothService) as BluetoothManager;
        }

        public static SimpleBluetoothDevice ConvertToSimpleBluetoothDevice(BluetoothDevice device)
        {
            return new SimpleBluetoothDevice
            {
                DeviceId = device.Address,
                DeviceName = string.IsNullOrEmpty(device.Name) ? device.Address : device.Name,
                IsBonded = device.BondState == Bond.Bonded ? true : false,
                DeviceClass = GetDeviceType(device.BluetoothClass.DeviceClass),
                DeviceState = GetDeviceConnectionState(device)
            };
        }

        private static BluetoothDeviceConnectionStateEnum GetDeviceConnectionState(BluetoothDevice device)
        {
            if (isConnected(device))
            {
                return BluetoothDeviceConnectionStateEnum.Connected;
            }
            return BluetoothDeviceConnectionStateEnum.Disconnected;
        }

        private static BluetoothDeviceTypeEnum GetDeviceType(DeviceClass deviceClass)
        {
            return BluetoothDeviceTypeEnum.Unknown;
        }

        public static bool isConnected(BluetoothDevice device)
        {
            try
            {
                var m = device.Class.GetMethod("isConnected", (Class[])null);
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