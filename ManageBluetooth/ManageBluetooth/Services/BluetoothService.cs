using System.Collections.Generic;
using System.Threading.Tasks;

using ManageBluetooth.Interface;
using ManageBluetooth.Models;

using Xamarin.Forms;

namespace ManageBluetooth.Services
{
    public class BluetoothService : IBluetoothService
    {
        private readonly IAndroidBluetoothService _androidBluetoothService;

        public BluetoothService()
        {
            this._androidBluetoothService = DependencyService.Get<IAndroidBluetoothService>();
        }

        public bool IsBluetoothEnabled()
        {
            return this._androidBluetoothService.IsBluetoothEnabled();
        }

        public void ChangeBluetoothState()
        {
            if (IsBluetoothEnabled())
            {
                this._androidBluetoothService.StopBluetoothScanning();
                this._androidBluetoothService.DisableBluetooth();
            }
            else
            {
                this._androidBluetoothService.EnableBluetooth();
            }
        }

        public IEnumerable<SimpleBluetoothDevice> GetBondedBluetoothDevices()
        {
            return this._androidBluetoothService.GetBondedDevices();
        }

        public void StartScanningForBluetoothDevices()
        {
            this._androidBluetoothService.StartBluetoothScanning();
        }

        public void StopScanningForBluetoothDevices()
        {
            this._androidBluetoothService.StopBluetoothScanning();
        }

        public bool IsBluetoothScanning()
        {
            return this._androidBluetoothService.BluetoothScanningStatus();
        }

        public async Task ConnectWithBluetoothDevice(SimpleBluetoothDevice device)
        {
            if (device.IsBonded)
            {
                await this._androidBluetoothService.ConnectWithDevice(device.DeviceId);
            }
            else
            {
                this._androidBluetoothService.BondWithDevice(device.DeviceId);
            }
        }

        public void DisconnectWithBluetoothDevice()
        {
            if (!this._androidBluetoothService.IsBluetoothEnabled())
            {
                return;
            }

            this._androidBluetoothService.DisconnectWithDevice();
        }

        public SimpleBluetoothDevice GetBluetoothDevice(string id)
        {
            if (!this._androidBluetoothService.IsBluetoothEnabled())
            {
                return null;
            }

            return this._androidBluetoothService.GetBluetoothDevice(id);
        }

        public void ChangeBluetoothDeviceAlias(string id, string newAlias)
        {
            this._androidBluetoothService.ChangeBluetoothDeviceAlias(id, newAlias);
        }

        public void UnbondWithBluetoothDevice(string id)
        {
            this._androidBluetoothService.UnbondWithBluetoothDevice(id);
        }
    }
}