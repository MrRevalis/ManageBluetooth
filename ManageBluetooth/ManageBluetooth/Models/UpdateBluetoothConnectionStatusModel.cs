using ManageBluetooth.Models.Enum;

namespace ManageBluetooth.Models
{
    public class UpdateBluetoothConnectionStatusModel
    {
        public string DeviceId { get; set; }
        public BluetoothDeviceConnectionStateEnum DeviceState { get; set; }
    }
}
