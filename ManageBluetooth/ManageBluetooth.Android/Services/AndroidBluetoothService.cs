﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Android.Bluetooth;
using Android.Content;


using Java.Util;

using ManageBluetooth.Droid.Converters;
using ManageBluetooth.Droid.Services;
using ManageBluetooth.Interface;
using ManageBluetooth.Models;
using ManageBluetooth.Models.Constants;

using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidBluetoothService))]
namespace ManageBluetooth.Droid.Services
{
    public class AndroidBluetoothService : IAndroidBluetoothService
    {
        private readonly BluetoothManager _bluetoothManager;
        private readonly BluetoothAdapter _bluetoothAdapter;

        private const string _defaultUuidForSpp = "00001101-0000-1000-8000-00805f9b34fb";
        private BluetoothSocket _socket;
        private Stream deviceInputStream;
        private Stream deviceOutputStream;

        public AndroidBluetoothService()
        {
            this._bluetoothManager = Android.App.Application.Context.GetSystemService(Context.BluetoothService) as BluetoothManager;
            this._bluetoothAdapter = this._bluetoothManager.Adapter;
        }

        public IEnumerable<SimpleBluetoothDevice> GetBondedDevices()
        {
            var bondedDevices = this._bluetoothAdapter.BondedDevices;

            if (bondedDevices == null
                || !bondedDevices.Any())
            {
                return Enumerable.Empty<SimpleBluetoothDevice>();
            }

            return bondedDevices.Select(device => BluetoothDeviceConverter.ConvertToSimpleBluetoothDevice(device));
        }

        public void DisableBluetooth()
        {
            if (this._bluetoothAdapter.IsEnabled)
            {
                this._bluetoothAdapter.Disable();
            }
        }

        public void EnableBluetooth()
        {
            if (!this._bluetoothAdapter.IsEnabled)
            {
                this._bluetoothAdapter.Enable();
            }
        }

        public void StartBluetoothScanning()
        {
            if (!this._bluetoothAdapter.IsDiscovering)
            {
                this._bluetoothAdapter.StartDiscovery();
            }
        }

        public void StopBluetoothScanning()
        {
            if (this._bluetoothAdapter.IsDiscovering)
            {
                this._bluetoothAdapter.CancelDiscovery();
            }
        }

        public bool BluetoothScanningStatus()
        {
            return this._bluetoothAdapter.IsEnabled
                ? this._bluetoothAdapter.IsDiscovering
                : false;
        }

        public async Task<bool> ConnectWithDevice(string id)
        {
            var device = this._bluetoothAdapter.GetRemoteDevice(id);

            if (device == null)
            {
                throw new Exception("Brak urzadzenia");
            }

            if (this.BluetoothScanningStatus())
            {
                this.StopBluetoothScanning();
            }

            await this.ConnectWithDevice(device);

            return _socket != null;
        }


        public void BondWithDevice(string id)
        {
            var device = this._bluetoothAdapter.GetRemoteDevice(id);

            if (device == null)
            {
                throw new Exception("Brak urzadzenia");
            }

            if (this.BluetoothScanningStatus())
            {
                this.StopBluetoothScanning();
            }

            device.CreateBond();
        }


        public void DisconnectWithDevice()
        {
            if (this._socket != null
                && this._socket.IsConnected)
            {
                try
                {
                    this.deviceInputStream.Close();
                    this.deviceOutputStream.Close();
                    Thread.Sleep(1000);
                    this._socket.Close();

                    this._socket = null;
                }
                catch (Exception e)
                {

                }
            }
        }

        private async Task<bool> ConnectWithDevice(BluetoothDevice device)
        {
            if (device.FetchUuidsWithSdp())
            {
                var uuids = device.GetUuids();

                if (uuids != null
                    && uuids.Any())
                {
                    foreach (var uuid in uuids)
                    {
                        try
                        {
                            this._socket = device.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString(uuid.ToString()));
                            await _socket.ConnectAsync();
                            this.deviceInputStream = this._socket.InputStream;
                            this.deviceOutputStream = this._socket.OutputStream;

                            return true;
                        }
                        catch (Exception e)
                        {
                            this._socket.Close();
                        }

                        var statusModel = new UpdateBluetoothConnectionStatusModel
                        {
                            DeviceId = device.Address,
                            DeviceState = Models.Enum.BluetoothDeviceConnectionStateEnum.Error
                        };

                        MessagingCenter.Send(Application.Current, BluetoothCommandConstants.BluetoothDeviceConnectionStateChanged, statusModel);
                    }
                }
            }

            return false;
        }

        public SimpleBluetoothDevice GetBluetoothDevice(string id)
        {
            var device = this._bluetoothAdapter.GetRemoteDevice(id);

            if (device == null)
            {
                return null;
            }

            return BluetoothDeviceConverter.ConvertToSimpleBluetoothDevice(device);
        }
    }
}