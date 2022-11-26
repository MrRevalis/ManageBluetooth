using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;

using ManageBluetooth.Droid.Receiver;

using Xamarin.CommunityToolkit.Helpers;

namespace ManageBluetooth.Droid
{
    [Activity(Label = "ManageBluetooth", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.LayoutDirection | ConfigChanges.Locale)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string[] Permissions =
        {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.BluetoothPrivileged,
        };

        private readonly BluetoothDiscoveryActionReceiver _deviceDiscoverReceiver;
        private readonly BluetoothDeviceConnectionStateReceiver _deviceConnectionStateReceiver;

        public MainActivity()
        {
            this._deviceDiscoverReceiver = new BluetoothDiscoveryActionReceiver();
            this._deviceConnectionStateReceiver = new BluetoothDeviceConnectionStateReceiver();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            CheckPermissions();
            SetUpBluetoothSettings();

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            // newConfig.Locale.Language
            var locale = newConfig.Locales.Get(0);
            var newCulture = !string.IsNullOrEmpty(locale.Language) ? locale.Language : "en";
            LocalizationResourceManager.Current.CurrentCulture = new System.Globalization.CultureInfo(newCulture);
        }

        private void CheckPermissions()
        {
            bool minimumPermissionsGranted = true;

            foreach (string permission in Permissions)
            {
                if (CheckSelfPermission(permission) != Permission.Granted)
                {
                    minimumPermissionsGranted = false;
                }
            }

            if (!minimumPermissionsGranted)
            {
                RequestPermissions(Permissions, 0);
            }
        }

        private void SetUpBluetoothSettings()
        {
            var discoveryStarted = new IntentFilter(BluetoothAdapter.ActionDiscoveryStarted);
            var discoveryFinished = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
            RegisterReceiver(this._deviceDiscoverReceiver, discoveryStarted);
            RegisterReceiver(this._deviceDiscoverReceiver, discoveryFinished);

            var actionAclConnected = new IntentFilter(BluetoothDevice.ActionAclConnected);
            var actionAclDisconnected = new IntentFilter(BluetoothDevice.ActionAclDisconnected);
            var actionAclDisconnectRequested = new IntentFilter(BluetoothDevice.ActionAclDisconnectRequested);
            RegisterReceiver(this._deviceConnectionStateReceiver, actionAclConnected);
            RegisterReceiver(this._deviceConnectionStateReceiver, actionAclDisconnected);
            RegisterReceiver(this._deviceConnectionStateReceiver, actionAclDisconnectRequested);
        }
    }
}