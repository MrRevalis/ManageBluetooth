﻿using System.Collections.Generic;
using System.Threading.Tasks;

using ManageBluetooth.Models;

namespace ManageBluetooth.Interface
{
    public interface IAndroidBluetoothService
    {
        IEnumerable<SimpleBluetoothDevice> GetBondedDevices();
        void EnableBluetooth();
        void DisableBluetooth();
        void StartBluetoothScanning();
        void StopBluetoothScanning();
        bool BluetoothScanningStatus();
        Task<bool> ConnectWithDevice(string id);
    }
}