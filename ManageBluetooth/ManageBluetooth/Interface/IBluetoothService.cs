using System.Collections.Generic;

namespace ManageBluetooth.Interface
{
    public interface IBluetoothService
    {
        bool IsBluetoothEnabled();
        void ChangeBluetoothState();
        IEnumerable<string> GetConnectedOrKnowBluetoothDevices();
        void StartScanningForBluetoothDevices();
        void StopScanningForBluetoothDevices();
    }
}
