using Android.Bluetooth;

namespace ManageBluetooth.Droid.Helpers
{
    public static class BluetoothDeviceHelper
    {
        public static string GetBluetoothDeviceName(BluetoothDevice device)
        {
            return !string.IsNullOrEmpty(device.Alias)
                ? device.Alias
                : !string.IsNullOrEmpty(device.Name)
                    ? device.Name
                    : device.Address;
        }
    }
}