using Android.App;
using Android.Bluetooth;
using Android.Content;

using ManageBluetooth.Droid.Extensions;
using ManageBluetooth.Models;
using ManageBluetooth.Models.Constants;
using ManageBluetooth.Models.Enum;

using Xamarin.Forms;

namespace ManageBluetooth.Droid.Receivers
{
    [BroadcastReceiver]
    [IntentFilter(new[]
    {
        BluetoothDevice.ActionAclConnected,
        BluetoothDevice.ActionAclDisconnected,
        BluetoothDevice.ActionAclDisconnectRequested,
    })]
    public class BluetoothDeviceConnectionStateReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;
            var device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;

            if (device == null)
            {
                return;
            }

            var updateModel = new UpdateBluetoothConnectionStatusModel
            {
                DeviceId = device.Address,
                DeviceName = device.GetDeviceName(),
            };

            switch (action)
            {
                case BluetoothDevice.ActionAclConnected:
                    updateModel.DeviceState = BluetoothDeviceConnectionStateEnum.Connected;
                    break;
                case BluetoothDevice.ActionAclDisconnectRequested:
                    updateModel.DeviceState = BluetoothDeviceConnectionStateEnum.Disconnecting;
                    break;
                case BluetoothDevice.ActionAclDisconnected:
                    updateModel.DeviceState = BluetoothDeviceConnectionStateEnum.Disconnected;
                    break;

            }

            MessagingCenter.Send(Xamarin.Forms.Application.Current, BluetoothCommandConstants.BluetoothDeviceConnectionStateChanged, updateModel);
        }
    }
}