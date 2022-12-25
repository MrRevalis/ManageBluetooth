using Android.App;
using Android.Bluetooth;
using Android.Content;

using ManageBluetooth.Models;
using ManageBluetooth.Models.Constants;
using ManageBluetooth.Models.Enum;

using Xamarin.Forms;

namespace ManageBluetooth.Droid.Receivers
{
    [BroadcastReceiver]
    [IntentFilter(new[]
    {
        BluetoothDevice.ActionBondStateChanged,
    })]
    public class BluetoothDeviceBondedStateReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;
            if (action.Equals(BluetoothDevice.ActionBondStateChanged))
            {
                var device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;
                var previousState = intent.GetIntExtra(BluetoothDevice.ExtraPreviousBondState, BluetoothDevice.Error);

                if (device == null)
                {
                    return;
                }

                var updateModel = new UpdateBluetoothBondStatusModel
                {
                    DeviceId = device.Address,
                    IsBonded = device.BondState == Bond.Bonded ? true : false,
                };

                MessagingCenter.Send(Xamarin.Forms.Application.Current, BluetoothCommandConstants.BluetoothDeviceBondStateChanged, updateModel);

                if (device.BondState == Bond.Bonded)
                {
                    var model = new UpdateBluetoothConnectionStatusModel
                    {
                        DeviceId = device.Address,
                        DeviceState = BluetoothDeviceConnectionStateEnum.Connected
                    };

                    // MessagingCenter.Send(Xamarin.Forms.Application.Current, BluetoothCommandConstants.ConnectBluetoothDevice, device.Address);
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, BluetoothCommandConstants.BluetoothDeviceConnectionStateChanged, model);
                }
            }
        }
    }
}