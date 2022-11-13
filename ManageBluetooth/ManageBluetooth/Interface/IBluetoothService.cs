using System.Collections.Generic;

using ManageBluetooth.Models;

namespace ManageBluetooth.Interface
{
    public interface IBluetoothService
    {
        bool IsBluetoothEnabled();
        void ChangeBluetoothState();
        IEnumerable<SimpleBluetoothDevice> GetConnectedOrKnowBluetoothDevices();
        void StartScanningForBluetoothDevices();
        void StopScanningForBluetoothDevices();
        bool IsBluetoothScanning();
    }
}
