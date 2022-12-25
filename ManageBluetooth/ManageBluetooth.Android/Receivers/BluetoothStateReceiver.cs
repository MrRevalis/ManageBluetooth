using Android.App;
using Android.Bluetooth;
using Android.Content;

using ManageBluetooth.Models.Constants;

using Xamarin.Forms;

namespace ManageBluetooth.Droid.Receivers
{
    [BroadcastReceiver]
    [IntentFilter(new[]
    {
        BluetoothAdapter.ActionStateChanged,
    })]
    public class BluetoothStateReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var bluetoothState = (State)intent.GetIntExtra(BluetoothAdapter.ExtraState, BluetoothAdapter.Error);

            switch (bluetoothState)
            {
                case State.Off:
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, BluetoothCommandConstants.BluetootStateChanged, false);
                    break;
                case State.On:
                    MessagingCenter.Send(Xamarin.Forms.Application.Current, BluetoothCommandConstants.BluetootStateChanged, true);
                    break;
                case State.TurningOn:
                    break;
                case State.TurningOff:
                    break;
                default:
                    break;
            }
        }
    }
}