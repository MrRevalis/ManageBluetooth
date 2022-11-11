using System.Collections.Generic;

using Android.Bluetooth;

using ManageBluetooth.Converters;
using ManageBluetooth.Interface;
using ManageBluetooth.Models;
using ManageBluetooth.Models.Constants;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

using Xamarin.Forms;

using ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode;

namespace ManageBluetooth.Services
{
    public class BluetoothService : IBluetoothService
    {
        private readonly IBluetoothLE _bluetooth;
        private readonly IAdapter _adapter;

        private readonly BluetoothManager _manager;

        private List<IDevice> DiscoveredDeviceList { get; set; }
        private List<IDevice> ConnectedDevices { get; set; }

        public BluetoothService()
        {
            this._bluetooth = CrossBluetoothLE.Current;
            this._adapter = CrossBluetoothLE.Current.Adapter;

            this._manager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService);

            this._adapter.ScanMode = ScanMode.LowLatency;

            this.DiscoveredDeviceList = new List<IDevice>();
            this.ConnectedDevices = new List<IDevice>();

            this._bluetooth.StateChanged += BluetootStateChanged;
            this._adapter.DeviceDiscovered += BluetoothDeviceDiscovered;
            this._adapter.ScanTimeoutElapsed += BluetoothScanTimeoutElapsed;
        }

        ~BluetoothService()
        {
            this._bluetooth.StateChanged -= BluetootStateChanged;
            this._adapter.DeviceDiscovered -= BluetoothDeviceDiscovered;
            this._adapter.ScanTimeoutElapsed -= BluetoothScanTimeoutElapsed;
        }

        public bool IsBluetoothEnabled()
        {
            return this._bluetooth.IsOn;
        }

        public void ChangeBluetoothState()
        {
            if (IsBluetoothEnabled())
            {
                _manager.Adapter.Disable();
            }
            else
            {
                _manager.Adapter.Enable();
            }
        }

        public IEnumerable<string> GetConnectedOrKnowBluetoothDevices()
        {
            var devicesNames = new List<string>();

            var systemDevices = _adapter.GetSystemConnectedOrPairedDevices();

            foreach (var device in systemDevices)
            {
                this.ConnectedDevices.Add(device);
                devicesNames.Add(device.Name);
            }

            return devicesNames;
        }

        public async void StartScanningForBluetoothDevices()
        {
            this._adapter.ScanTimeout = 15000;
            await this._adapter.StartScanningForDevicesAsync();
        }

        public async void StopScanningForBluetoothDevices()
        {
            await this._adapter.StopScanningForDevicesAsync();
        }

        private void BluetoothScanTimeoutElapsed(object sender, System.EventArgs e)
        {
            MessagingCenter.Send<BluetoothService, bool>(this, BluetoothCommandConstants.BluetoothScanTimeoutElapsed, false);

        }

        private void BluetoothDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if (!this.DiscoveredDeviceList.Contains(e.Device)
                && !string.IsNullOrEmpty(e.Device.Name))
            {
                this.DiscoveredDeviceList.Add(e.Device);
                var bluetoothDevice = IDeviceConverter.ConvertToSimpleBluetoothDevice(e.Device);

                MessagingCenter.Send<BluetoothService, SimpleBluetoothDevice>(this, BluetoothCommandConstants.BluetootDeviceDiscovered, bluetoothDevice);
            }
        }

        private void BluetootStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            switch (e.NewState)
            {
                case BluetoothState.On:
                    MessagingCenter.Send<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, true);
                    break;
                case BluetoothState.Off:
                    MessagingCenter.Send<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, false);
                    break;
                default:
                    break;
            }
        }
    }
}