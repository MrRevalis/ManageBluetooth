using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task ConnectWithUnknownDevice(Guid deviceGuid);
    }
}
