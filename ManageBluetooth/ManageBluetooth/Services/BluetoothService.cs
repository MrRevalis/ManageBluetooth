using System.Collections.Generic;
using System.Threading.Tasks;

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

        public bool BluetoothEnabled { get; set; }

        public BluetoothService()
        {
            this._bluetooth = CrossBluetoothLE.Current;
            this._adapter = CrossBluetoothLE.Current.Adapter;

            this._manager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService);

            this._bluetooth.StateChanged += BluetootStateChanged;
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
                    break;
                default:
                    this.BluetoothEnabled = false;
                    break;
            }

            MessagingCenter.Send<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, this.BluetoothEnabled);
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

        public async Task<IEnumerable<string>> GetConnectedBluetoothDevices()
        {
            var devicesNames = new List<string>();

            var systemDevices = _adapter.GetSystemConnectedOrPairedDevices();

            foreach (var device in systemDevices)
            {
                devicesNames.Add(device.Name);
            }

            return devicesNames;
        }
    }
}