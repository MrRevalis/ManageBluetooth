namespace ManageBluetooth.Models
{
    public class UpdateBluetoothBondStatusModel
    {
        public string DeviceId { get; set; }
        public bool IsBonded { get; set; }
    }
}
