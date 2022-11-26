using ManageBluetooth.Models.Enum;

namespace ManageBluetooth.Models
{
    public class SimpleBluetoothDevice
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public bool IsBonded { get; set; }
        public BluetoothDeviceTypeEnum DeviceClass { get; set; }
        public BluetoothDeviceConnectionStateEnum DeviceState { get; set; }
    }
}