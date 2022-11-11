using System.Collections.Generic;

using Android.Bluetooth;

using ManageBluetooth.Interface;
using ManageBluetooth.Models.Constants;

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

using Xamarin.Forms;

namespace ManageBluetooth.Services
{
    public class BluetoothService : IBluetoothService
    {
        private readonly IBluetoothLE _bluetooth;
        private readonly IAdapter _adapter;

        private readonly BluetoothManager _manager;

        private List<string> DeviceList { get; set; }

        public bool BluetoothEnabled { get; set; }

        public BluetoothService()
        {
            this._bluetooth = CrossBluetoothLE.Current;
            this._adapter = CrossBluetoothLE.Current.Adapter;

            this._manager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService);

            this.DeviceList = new List<string>();

            this._bluetooth.StateChanged += BluetootStateChanged;
            this._adapter.DeviceDiscovered += BluetoothDeviceDiscovered;
            this._adapter.ScanTimeoutElapsed += BluetoothScanTimeoutElapsed;
        }

        private void BluetoothScanTimeoutElapsed(object sender, System.EventArgs e)
        {
            MessagingCenter.Send<BluetoothService, bool>(this, BluetoothCommandConstants.BluetoothScanTimeoutElapsed, false);

        }

        private void BluetoothDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            MessagingCenter.Send<BluetoothService, string>(this, BluetoothCommandConstants.BluetootDeviceDiscovered, e.Device.Name);
        }

        ~BluetoothService()
        {
            this._bluetooth.StateChanged -= BluetootStateChanged;
        }

        private void BluetootStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            switch (e.NewState)
            {
                case BluetoothState.On:
                    this.BluetoothEnabled = true;
                    MessagingCenter.Send<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, this.BluetoothEnabled);
                    break;
                case BluetoothState.Off:
                    this.BluetoothEnabled = false;
                    MessagingCenter.Send<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, this.BluetoothEnabled);
                    break;
                default:
                    this.BluetoothEnabled = false;
                    break;
            }
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
                devicesNames.Add(device.Name);
            }

            return devicesNames;
        }

        public async void StartScanningForBluetoothDevices()
        {
            this._adapter.ScanTimeout = 15000;
            this._adapter.ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode.LowLatency;
            await this._adapter.StartScanningForDevicesAsync();
        }

        public async void StopScanningForBluetoothDevices()
        {
            await this._adapter.StopScanningForDevicesAsync();
        }
    }
}