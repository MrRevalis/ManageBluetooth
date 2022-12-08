using ManageBluetooth.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ManageBluetooth.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothDevicePage : ContentPage
    {
        public BluetoothDevicePage()
        {
            InitializeComponent();

            BindingContext = Startup.ServiceProvider.GetService<BluetoothDevicePageViewModel>();
        }
    }
}