using System.Collections.Generic;
using System.Web;
using System.Windows.Input;

using ManageBluetooth.Interface;
using ManageBluetooth.Models;
using ManageBluetooth.Resources;
using ManageBluetooth.ViewModels.Base;

using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;

namespace ManageBluetooth.ViewModels
{
    public class BluetoothDevicePageViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IBluetoothService _bluetoothService;
        private readonly IToastService _toastService;

        public LocalizedString ConnectedDevice { get; set; }
        public LocalizedString ChangeName { get; set; }
        public LocalizedString CancelBond { get; set; }

        private SimpleBluetoothDevice device;
        public SimpleBluetoothDevice Device
        {
            get => device;
            set => SetProperty(ref device, value);
        }

        public ICommand ChangeDeviceAliasCommand { get; set; }
        public ICommand CancelBondWithDeviceCommand { get; set; }

        public BluetoothDevicePageViewModel(IBluetoothService bluetoothService)
        {
            this._bluetoothService = bluetoothService;
            this._toastService = DependencyService.Get<IToastService>();

            this.ChangeDeviceAliasCommand = new Command(ChangeDeviceAlias);
            this.CancelBondWithDeviceCommand = new Command(CancelBondWithDevice);

            this.ConnectedDevice = new LocalizedString(() => AppResources.ConnectedDevice);
            this.ChangeName = new LocalizedString(() => AppResources.ChangeName);
            this.CancelBond = new LocalizedString(() => AppResources.CancelBond);
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            var deviceId = HttpUtility.UrlDecode(query[nameof(SimpleBluetoothDevice.DeviceId)]);

            this.Device = this._bluetoothService.GetBluetoothDevice(deviceId);

            if (this.Device == null)
            {
                this._toastService.ShortAlert("Brak urzadzenia");
                Shell.Current.GoToAsync("..");
            }
        }

        private void ChangeDeviceAlias()
        {

        }

        private void CancelBondWithDevice()
        {

        }
    }
}
