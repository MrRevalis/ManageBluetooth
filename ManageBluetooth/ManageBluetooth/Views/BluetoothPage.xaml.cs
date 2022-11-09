using ManageBluetooth.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ManageBluetooth.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothPage : ContentPage
    {
        public BluetoothPage()
        {
            InitializeComponent();

            BindingContext = Startup.ServiceProvider.GetService<BluetoothPageViewModel>();
        }
    }
}