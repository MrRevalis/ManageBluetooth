using System.Collections.Generic;
using System.Threading.Tasks;

using ManageBluetooth.Extensions;
using ManageBluetooth.Helpers;
using ManageBluetooth.Interface;
using ManageBluetooth.Models;
using ManageBluetooth.Models.Constants;
using ManageBluetooth.Models.Enum;

using Xamarin.Forms;

namespace ManageBluetooth.Services
{
    public class BluetoothService : IBluetoothService
    {
        private readonly IAndroidBluetoothService _androidBluetoothService;

        private const int MaxConnectionTries = 3;

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
            return this._androidBluetoothService.IsBluetoothScanning();
        }

        public async Task ConnectWithBluetoothDevice(SimpleBluetoothDevice device)
        {
            this.UpdateBluetoothDeviceConnectionState(device, BluetoothDeviceConnectionStateEnum.Connecting);
            for (int i = 0; i < MaxConnectionTries; i++)
            {
                var result = await this._androidBluetoothService.ConnectWithDevice(device.DeviceId)
                    .ExecuteAsyncOperation();

                if (result)
                {
                    return;
                }
            }

            this.UpdateBluetoothDeviceConnectionState(device, BluetoothDeviceConnectionStateEnum.Error);
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
            ExceptionHelper.CatchException(() => this._androidBluetoothService.ChangeBluetoothDeviceAlias(id, newAlias));
        }

        public void UnbondWithBluetoothDevice(string id)
        {
            ExceptionHelper.CatchException(() => this._androidBluetoothService.UnbondWithBluetoothDevice(id));
        }

        private void UpdateBluetoothDeviceConnectionState(SimpleBluetoothDevice device, BluetoothDeviceConnectionStateEnum connectionState)
        {
            MessagingCenter.Send(Application.Current, BluetoothCommandConstants.BluetoothDeviceConnectionStateChanged, new UpdateBluetoothConnectionStatusModel
            {
                DeviceId = device.DeviceId,
                DeviceName = device.DeviceName,
                DeviceState = connectionState,
            });
        }
    }
}