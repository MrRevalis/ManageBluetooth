﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.Bluetooth;

using ManageBluetooth.Converters;
using ManageBluetooth.Interface;
using ManageBluetooth.Models;
using ManageBluetooth.Models.Constants;

using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;

using Xamarin.Forms;

using ScanMode = Plugin.BLE.Abstractions.Contracts.ScanMode;

namespace ManageBluetooth.Services
{
    public class BluetoothService : IBluetoothService
    {
        private readonly IBluetoothLE _bluetooth;
        private readonly IAdapter _adapter;

        private readonly BluetoothManager _manager;

        private readonly int ScanTimeout = 15000;

        private List<IDevice> DiscoveredDeviceList { get; set; }
        private List<IDevice> ConnectedDevicesList { get; set; }

        public BluetoothService()
        {
            this._bluetooth = CrossBluetoothLE.Current;
            this._adapter = CrossBluetoothLE.Current.Adapter;

            this._manager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService);

            this.DiscoveredDeviceList = new List<IDevice>();
            this.ConnectedDevicesList = new List<IDevice>();

            this._bluetooth.StateChanged += BluetootStateChanged;
            this._adapter.DeviceDiscovered += BluetoothDeviceDiscovered;
            this._adapter.ScanTimeoutElapsed += BluetoothScanTimeoutElapsed;

            this._adapter.ScanTimeout = this.ScanTimeout;
            this._adapter.ScanMode = ScanMode.LowLatency;
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

        public async void ChangeBluetoothState()
        {
            if (IsBluetoothEnabled())
            {
                await this._adapter.StopScanningForDevicesAsync();
                this._manager.Adapter.Disable();

                this.DiscoveredDeviceList.Clear();
                this.ConnectedDevicesList.Clear();
            }
            else
            {
                this._manager.Adapter.Enable();
            }
        }

        public IEnumerable<SimpleBluetoothDevice> GetConnectedOrKnowBluetoothDevices()
        {
            var devices = new List<SimpleBluetoothDevice>();
            var connectedOrPairedDevices = this._adapter.GetSystemConnectedOrPairedDevices();

            foreach (var device in connectedOrPairedDevices)
            {
                this.ConnectedDevicesList.Add(device);
                devices.Add(IDeviceConverter.ConvertToSimpleBluetoothDevice(device));
            }

            return devices;
        }

        public async void StartScanningForBluetoothDevices()
        {
            await this._adapter.StartScanningForDevicesAsync();
        }

        public async void StopScanningForBluetoothDevices()
        {
            await this._adapter.StopScanningForDevicesAsync();
        }

        private async void BluetoothScanTimeoutElapsed(object sender, System.EventArgs e)
        {
            await this._adapter.StopScanningForDevicesAsync();

            MessagingCenter.Send(this, BluetoothCommandConstants.BluetoothScanTimeoutElapsed, false);
        }

        private void BluetoothDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if (!this.DiscoveredDeviceList.Contains(e.Device)
                && !string.IsNullOrEmpty(e.Device.Name))
            {
                this.DiscoveredDeviceList.Add(e.Device);
                var bluetoothDevice = IDeviceConverter.ConvertToSimpleBluetoothDevice(e.Device);

                MessagingCenter.Send(this, BluetoothCommandConstants.BluetootDeviceDiscovered, bluetoothDevice);
            }
        }

        private void BluetootStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            switch (e.NewState)
            {
                case BluetoothState.On:
                    MessagingCenter.Send(this, BluetoothCommandConstants.BluetootStateChanged, true);
                    break;
                case BluetoothState.Off:
                    MessagingCenter.Send(this, BluetoothCommandConstants.BluetootStateChanged, false);
                    break;
                default:
                    break;
            }
        }

        public bool IsBluetoothScanning()
        {
            return this._adapter.IsScanning;
        }

        public async Task ConnectWithUnknownDevice(Guid deviceGuid)
        {
            var device = this.DiscoveredDeviceList.FirstOrDefault(x => x.Id == deviceGuid);

            if (device == null)
            {
                return;
            }

            try
            {
                if (this.IsBluetoothScanning())
                {
                    this.StopScanningForBluetoothDevices();
                }
                var para = new ConnectParameters(true, false);

                await device.UpdateRssiAsync();

                await this._adapter.ConnectToDeviceAsync(device, para);
                var bluetoothDevice = IDeviceConverter.ConvertToSimpleBluetoothDevice(device);

                var qwe = "qwe;";
            }
            catch (DeviceConnectionException ex)
            {
                // Popup z bledem
            }
        }
    }
}