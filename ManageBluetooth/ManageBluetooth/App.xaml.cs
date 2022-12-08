using ManageBluetooth.Resources;

using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;

namespace ManageBluetooth
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Startup.Init();

            LocalizationResourceManager.Current.Init(AppResources.ResourceManager);
            LocalizationResourceManager.Current.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => AppResources.Culture = LocalizationResourceManager.Current.CurrentCulture; ;

            //MainPage = new NavigationPage(new BluetoothPage());
            MainPage = new AppShell();
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