using ManageBluetooth.Views;

using Xamarin.Forms;

namespace ManageBluetooth
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            Startup.Init();

            MainPage = new NavigationPage(new BluetoothPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}