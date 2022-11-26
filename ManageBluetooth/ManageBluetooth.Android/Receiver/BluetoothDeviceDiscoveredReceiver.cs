using System;

using Android.App;
using Android.Bluetooth;
using Android.Content;

using ManageBluetooth.Droid.Converters;
using ManageBluetooth.Models.Constants;

using Xamarin.Forms;

namespace ManageBluetooth.Droid.Receiver
{
    [BroadcastReceiver]
    [IntentFilter(new[] { BluetoothDevice.ActionFound })]
    public class BluetoothDeviceDiscoveredReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;

            if (action.Equals(BluetoothDevice.ActionFound, StringComparison.InvariantCulture))
            {
                var device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;

                if (device == null)
                {
                    return;
                }

                var bluetoothDevice = BluetoothDeviceConverter.ConvertToSimpleBluetoothDevice(device);

                MessagingCenter.Send(Xamarin.Forms.Application.Current, BluetoothCommandConstants.BluetootDeviceDiscovered, bluetoothDevice);
            }
        }
    }
}