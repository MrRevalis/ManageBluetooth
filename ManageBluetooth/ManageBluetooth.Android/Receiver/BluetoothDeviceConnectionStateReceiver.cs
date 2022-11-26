using Android.App;
using Android.Bluetooth;
using Android.Content;

namespace ManageBluetooth.Droid.Receiver
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
        }
    }
}