using System;
using System.Collections.Generic;

using Android.Bluetooth;

using Plugin.BLE.Abstractions;

namespace ManageBluetooth.Models
{
    public class SimpleBluetoothDevice : IEquatable<SimpleBluetoothDevice>
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DeviceClass DeviceClass { get; set; }
        public DeviceState DeviceState { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SimpleBluetoothDevice);
        }

        public bool Equals(SimpleBluetoothDevice other)
        {
            return other is not null &&
                   DeviceId.Equals(other.DeviceId) &&
                   DeviceName == other.DeviceName &&
                   DeviceClass == other.DeviceClass &&
                   DeviceState == other.DeviceState;
        }

        public override int GetHashCode()
        {
            int hashCode = -2139169097;
            hashCode = hashCode * -1521134295 + DeviceId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DeviceName);
            hashCode = hashCode * -1521134295 + DeviceClass.GetHashCode();
            hashCode = hashCode * -1521134295 + DeviceState.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(SimpleBluetoothDevice left, SimpleBluetoothDevice right)
        {
            return EqualityComparer<SimpleBluetoothDevice>.Default.Equals(left, right);
        }

        public static bool operator !=(SimpleBluetoothDevice left, SimpleBluetoothDevice right)
        {
            return !(left == right);
        }
    }
}