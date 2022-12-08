using System.Windows.Input;

using ManageBluetooth.Resources;
using ManageBluetooth.ViewModels.Base;

using Xamarin.CommunityToolkit.Helpers;

namespace ManageBluetooth.ViewModels
{
    public class BluetoothDevicePageViewModel : BaseViewModel
    {
        public LocalizedString ConnectedDevice { get; set; }
        public LocalizedString ChangeName { get; set; }
        public LocalizedString CancelBond { get; set; }

        public ICommand ChangeDeviceAliasCommand { get; set; }
        public ICommand CancelBondWithDeviceCommand { get; set; }

        public BluetoothDevicePageViewModel()
        {
            this.ConnectedDevice = new LocalizedString(() => AppResources.ConnectedDevice);
            this.ChangeName = new LocalizedString(() => AppResources.ChangeName);
            this.CancelBond = new LocalizedString(() => AppResources.CancelBond);
        }
    }
}
