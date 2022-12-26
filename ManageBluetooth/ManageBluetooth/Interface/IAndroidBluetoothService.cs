using System.Collections.Generic;
using System.Threading.Tasks;

using ManageBluetooth.Models;

namespace ManageBluetooth.Interface
{
    public interface IAndroidBluetoothService
    {
        bool IsBluetoothEnabled();
        IEnumerable<SimpleBluetoothDevice> GetBondedDevices();
        void EnableBluetooth();
        void DisableBluetooth();
        void StartBluetoothScanning();
        void StopBluetoothScanning();
        bool IsBluetoothScanning();
        Task<bool> ConnectWithDevice(string id);
        void DisconnectWithDevice();
        void BondWithDevice(string id);
        SimpleBluetoothDevice GetBluetoothDevice(string id);
        void ChangeBluetoothDeviceAlias(string id, string newAlias);
        void UnbondWithBluetoothDevice(string id);
    }
}
