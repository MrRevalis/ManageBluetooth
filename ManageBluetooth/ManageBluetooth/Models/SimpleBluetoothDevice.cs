using System;

using Android.Bluetooth;

using Plugin.BLE.Abstractions;

namespace ManageBluetooth.Models
{
    public class SimpleBluetoothDevice
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DeviceClass DeviceClass { get; set; }
        public DeviceState DeviceState { get; set; }
    }
}