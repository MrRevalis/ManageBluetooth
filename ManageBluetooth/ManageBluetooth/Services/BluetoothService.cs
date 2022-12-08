using System.Collections.Generic;
using System.Threading.Tasks;

using ManageBluetooth.Interface;
using ManageBluetooth.Models;
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
        private readonly IAndroidBluetoothService _androidBluetoothService;
        private readonly IToastService _toastService;

        public BluetoothService()
        {
            this._bluetooth = CrossBluetoothLE.Current;
            this._adapter = CrossBluetoothLE.Current.Adapter;
            this._androidBluetoothService = DependencyService.Get<IAndroidBluetoothService>();
            this._toastService = DependencyService.Get<IToastService>();

            this._bluetooth.StateChanged += BluetootStateChanged;
        }

        ~BluetoothService()
        {
            this._bluetooth.StateChanged -= BluetootStateChanged;
        }

        public bool IsBluetoothEnabled()
        {
            return this._bluetooth.IsOn;
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
            //if (!result)
            //{
            //    device.DeviceState = Models.Enum.BluetoothDeviceConnectionStateEnum.Error;
            //}
        }

        public void DisconnectWithBluetoothDevice()
        {
            this._androidBluetoothService.DisconnectWithDevice();
        }

        public SimpleBluetoothDevice GetBluetoothDevice(string id)
        {
            if (!IsBluetoothEnabled())
            {
                return null;
            }

            return this._androidBluetoothService.GetBluetoothDevice(id);
        }
    }
}