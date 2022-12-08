using ManageBluetooth.Views;

using Xamarin.Forms;

namespace ManageBluetooth
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(BluetoothPage), typeof(BluetoothPage));
            Routing.RegisterRoute($"{nameof(BluetoothPage)}/{nameof(BluetoothDevicePage)}", typeof(BluetoothDevicePage));
        }
    }
}