using System;

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
        BluetoothAdapter.ActionDiscoveryStarted,
        BluetoothAdapter.ActionDiscoveryFinished,
    })]
    public class BluetoothDiscoveryActionReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            if (action.Equals(BluetoothAdapter.ActionDiscoveryStarted, StringComparison.InvariantCulture))
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, BluetoothCommandConstants.BluetoothScanningStateChanged, true);
            }
            else if (action.Equals(BluetoothAdapter.ActionDiscoveryFinished, StringComparison.InvariantCulture))
            {
                MessagingCenter.Send(Xamarin.Forms.Application.Current, BluetoothCommandConstants.BluetoothScanningStateChanged, false);
            }
        }
    }
}