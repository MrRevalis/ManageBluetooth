using System.Collections.Generic;
using System.Threading.Tasks;

using ManageBluetooth.Models;

namespace ManageBluetooth.Interface
{
    public interface IBluetoothService
    {
        bool IsBluetoothEnabled();
        void ChangeBluetoothState();
        IEnumerable<SimpleBluetoothDevice> GetBondedBluetoothDevices();
        void StartScanningForBluetoothDevices();
        void StopScanningForBluetoothDevices();
        bool IsBluetoothScanning();
        Task ConnectWithBluetoothDevice(SimpleBluetoothDevice device);
        void DisconnectWithBluetoothDevice();

        SimpleBluetoothDevice GetBluetoothDevice(string id);
        void ChangeBluetoothDeviceAlias(string id, string newAlias);
        void UnbondWithBluetoothDevice(string id);
    }
}