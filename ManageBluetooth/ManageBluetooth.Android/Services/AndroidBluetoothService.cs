using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.Bluetooth;
using Android.Content;

using Java.Util;

using ManageBluetooth.Droid.Converters;
using ManageBluetooth.Droid.Services;
using ManageBluetooth.Interface;
using ManageBluetooth.Models;

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

            if (device.BondState == Bond.Bonded)
            {
                var deviceUuid = device.GetUuids().FirstOrDefault() ?? new Android.OS.ParcelUuid(UUID.FromString(_defaultUuidForSpp));

                _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString(deviceUuid.ToString()));
                await _socket.ConnectAsync();
            }
            else if (device.BondState == Bond.None)
            {
                // Dla testu, chyba trzeba najpierw polaczyć
                //device.SetPairingConfirmation(true);
                //device.SetPin(System.Text.Encoding.Default.GetBytes("1234"));
                //device.CreateBond();

                var deviceUuid = device.GetUuids().FirstOrDefault() ?? new Android.OS.ParcelUuid(UUID.FromString(_defaultUuidForSpp));

                _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString(deviceUuid.ToString()));
                await _socket.ConnectAsync();
            }

            return _socket != null;
        }
    }
}