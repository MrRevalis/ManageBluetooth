using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;

using Java.Util;

using ManageBluetooth.Droid.Converters;
using ManageBluetooth.Droid.Models.Constants;
using ManageBluetooth.Droid.Services;
using ManageBluetooth.Interface;
using ManageBluetooth.Models;
using ManageBluetooth.Models.Enum;

using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidBluetoothService))]
namespace ManageBluetooth.Droid.Services
{
    public class AndroidBluetoothService : IAndroidBluetoothService
    {
        private readonly BluetoothManager _bluetoothManager;
        private readonly BluetoothAdapter _bluetoothAdapter;

        private readonly ParcelUuid _defaultParcelUuid;

        private BluetoothSocket _socket;
        private Stream deviceInputStream;
        private Stream deviceOutputStream;

        public AndroidBluetoothService()
        {
            this._bluetoothManager = Android.App.Application.Context.GetSystemService(Context.BluetoothService) as BluetoothManager;
            this._bluetoothAdapter = this._bluetoothManager.Adapter;

            this._defaultParcelUuid = new ParcelUuid(UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
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

        public bool IsBluetoothEnabled()
        {
            return this._bluetoothAdapter.IsEnabled;
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

        public bool IsBluetoothScanning()
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
                throw new Exception(ErrorEnum.DeviceCannotBeFound.ToString());
            }

            if (this.IsBluetoothScanning())
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
                throw new Exception(ErrorEnum.DeviceCannotBeFound.ToString());
            }

            if (this.IsBluetoothScanning())
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
                    this.deviceInputStream = null;

                    this.deviceOutputStream.Close();
                    this.deviceOutputStream = null;

                    Thread.Sleep(1000);

                    //this._socket.Close();

                    //if (!this._socket.IsConnected)
                    //{
                    //    this._socket = null;
                    //}

                    var inputStream = this._socket.InputStream as InputStreamInvoker;
                    while (true)
                    {
                        try
                        {
                            var nextByte = inputStream.ReadByte();
                        }
                        catch (Java.IO.IOException)
                        {
                            this._socket.Close();
                            this._socket = null;

                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception(ErrorEnum.CannotConnectToTheDevice.ToString());
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
                    //if (device.BondState == Bond.None)
                    //{
                    //    device.CreateBond();
                    //}

                    if (!uuids.Contains(_defaultParcelUuid))
                    {
                        uuids.Append(_defaultParcelUuid);
                    }

                    foreach (var uuid in uuids)
                    {
                        try
                        {
                            if (this._socket != null
                                && this._socket.IsConnected)
                            {
                                return true;
                            }

                            this._socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString(uuid.ToString()));
                            // this._socket = device.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString(uuid.ToString()));
                            // this._socket = this.CreateSocket(device);

                            Thread.Sleep(1000);

                            await _socket.ConnectAsync();

                            this.deviceInputStream = this._socket.InputStream;
                            this.deviceOutputStream = this._socket.OutputStream;

                            return true;
                        }
                        catch (Exception)
                        {
                            this._socket.Close();
                        }
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

        public void ChangeBluetoothDeviceAlias(string id, string newAlias)
        {
            var device = this._bluetoothAdapter.GetRemoteDevice(id);

            if (device == null)
            {
                throw new Exception(ErrorEnum.DeviceCannotBeFound.ToString());
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
            catch (Exception)
            {
                throw new Exception(ErrorEnum.CannotChangeDeviceAlias.ToString());
            }
        }

        public void UnbondWithBluetoothDevice(string id)
        {
            var device = this._bluetoothAdapter.GetRemoteDevice(id);

            if (device == null)
            {
                throw new Exception(ErrorEnum.DeviceCannotBeFound.ToString());
            }

            var removeBondMethod = device.Class.GetMethod(Constants.BluetoothDeviceMethodNames.RemoveBond, (Java.Lang.Class[])null);
            removeBondMethod.Invoke(device, (Java.Lang.Object[])null);
        }

        private BluetoothSocket CreateSocket(BluetoothDevice device)
        {
            var m = device.Class.GetMethod(Constants.BluetoothDeviceMethodNames.CreateSocket, new Java.Lang.Class[] { Java.Lang.Integer.Type });

            return (BluetoothSocket)m.Invoke(device, 1);
        }
    }
}