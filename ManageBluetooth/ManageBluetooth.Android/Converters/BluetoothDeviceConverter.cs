using Android.App;
using Android.Bluetooth;
using Android.Content;

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
            var deviceState = _bluetoothManager.GetConnectionState(device, ProfileType.Gatt);

            switch (deviceState)
            {
                case ProfileState.Disconnected: return BluetoothDeviceConnectionStateEnum.Disconnected;
                case ProfileState.Connecting: return BluetoothDeviceConnectionStateEnum.Connecting;
                case ProfileState.Connected: return BluetoothDeviceConnectionStateEnum.Connected;
                case ProfileState.Disconnecting: return BluetoothDeviceConnectionStateEnum.Disconnecting;
                default:
                    return BluetoothDeviceConnectionStateEnum.Disconnected;
            }
        }

        private static BluetoothDeviceTypeEnum GetDeviceType(DeviceClass deviceClass)
        {
            return BluetoothDeviceTypeEnum.Unknown;
        }
    }
}