using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Android.Bluetooth;
using Android.Content;
using Android.OS;

using Java.Util;

using ManageBluetooth.Droid.Converters;
using ManageBluetooth.Droid.Extensions;
using ManageBluetooth.Droid.Models.Constants;
using ManageBluetooth.Droid.Services;
using ManageBluetooth.Interface;
using ManageBluetooth.Models;
using ManageBluetooth.Models.Constants;
using ManageBluetooth.Models.Enum;

using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidBluetoothService))]
namespace ManageBluetooth.Droid.Services
{
    public class AndroidBluetoothService : IAndroidBluetoothService
    {
        private readonly BluetoothManager _bluetoothManager;
        private readonly BluetoothAdapter _bluetoothAdapter;

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

            return await this.ConnectWithDevice(device);
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
            if (this._socket != null)
            // && this._socket.IsConnected)
            {
                try
                {
                    this.deviceInputStream.Close();
                    this.deviceOutputStream.Close();
                    Thread.Sleep(1000);
                    this._socket.Close();
                    // this._socket.Dispose();
                    if (!this._socket.IsConnected)
                    {
                        this._socket = null;
                    }
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
                    if (device.BondState != Bond.Bonded)
                    {
                        device.CreateBond();
                    }

                    this.UpdateBluetoothDeviceConnectionState(device, BluetoothDeviceConnectionStateEnum.Connecting);

                    foreach (var uuid in uuids)
                    {
                        try
                        {
                            this._socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString(uuid.ToString()));
                            await _socket.ConnectAsync();

                            this.deviceInputStream = this._socket.InputStream;
                            this.deviceOutputStream = this._socket.OutputStream;

                            return true;
                        }
                        catch (Exception e)
                        {
                            this._socket.Close();
                        }
                    }

                    this.UpdateBluetoothDeviceConnectionState(device, BluetoothDeviceConnectionStateEnum.Error);
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

        public void ChangeBluetoothDeviceAlias(string id, string newAlias)
        {
            var device = this._bluetoothAdapter.GetRemoteDevice(id);

            if (device == null)
            {
                return;
            }

            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                {
                    device.SetAlias(newAlias);
                    return;
                }

                var changeAlias = device.Class.GetMethod(Constants.BluetoothDeviceMethodNames.SetAlias, Java.Lang.Class.FromType(typeof(Java.Lang.String)));
                changeAlias.Invoke(device, newAlias);
            }
            catch (Exception e)
            {

            }

        }

        public void UnbondWithBluetoothDevice(string id)
        {
            var device = this._bluetoothAdapter.GetRemoteDevice(id);

            if (device == null)
            {
                return;
            }

            var removeBondMethod = device.Class.GetMethod(Constants.BluetoothDeviceMethodNames.RemoveBond, (Java.Lang.Class[])null);
            removeBondMethod.Invoke(device, (Java.Lang.Object[])null);
        }

        public bool IsBluetoothEnabled()
        {
            return this._bluetoothAdapter.IsEnabled;
        }

        private void UpdateBluetoothDeviceConnectionState(BluetoothDevice device, BluetoothDeviceConnectionStateEnum connectionState)
        {
            MessagingCenter.Send(Application.Current, BluetoothCommandConstants.BluetoothDeviceConnectionStateChanged, new UpdateBluetoothConnectionStatusModel
            {
                DeviceId = device.Address,
                DeviceName = device.GetDeviceName(),
                DeviceState = connectionState,
            });
        }
    }
}