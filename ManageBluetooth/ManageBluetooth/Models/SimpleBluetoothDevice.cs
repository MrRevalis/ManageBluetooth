using System.ComponentModel;
using System.Runtime.CompilerServices;

using ManageBluetooth.Models.Enum;

namespace ManageBluetooth.Models
{
    public class SimpleBluetoothDevice : INotifyPropertyChanged
    {
        public string DeviceId { get; set; }
        private string deviceName;
        public string DeviceName
        {
            get => deviceName;
            set
            {
                deviceName = value;
                OnPropertyChanged();
            }
        }

        private bool isBonded;
        public bool IsBonded
        {
            get => isBonded;
            set
            {
                isBonded = value;
                OnPropertyChanged();
            }
        }

        private BluetoothDeviceTypeEnum deviceClass;
        public BluetoothDeviceTypeEnum DeviceClass
        {
            get => deviceClass;
            set
            {
                deviceClass = value;
                OnPropertyChanged();
            }
        }

        private BluetoothDeviceConnectionStateEnum deviceState;
        public BluetoothDeviceConnectionStateEnum DeviceState
        {
            get => deviceState;
            set
            {
                if (deviceState == value) return;
                deviceState = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}