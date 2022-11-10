using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using ManageBluetooth.Interface;
using ManageBluetooth.Models.Constants;
using ManageBluetooth.Resources;
using ManageBluetooth.Services;
using ManageBluetooth.ViewModels.Base;

using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ManageBluetooth.ViewModels
{
    public class BluetoothPageViewModel : BaseViewModel
    {
        private readonly IBluetoothService _bluetoothService;

        private bool isBluetoothEnabled;
        public bool IsBluetoothEnabled
        {
            get => isBluetoothEnabled;
            set => SetProperty(ref isBluetoothEnabled, value);
        }

        private LocalizedString isBluetoothEnabledMessage;
        public LocalizedString IsBluetoothEnabledMessage
        {
            get => isBluetoothEnabledMessage;
            set => SetProperty(ref isBluetoothEnabledMessage, value);
        }

        private LocalizedString bluetoothState;
        public LocalizedString BluetoothState
        {
            get => bluetoothState;
            set => SetProperty(ref bluetoothState, value);
        }

        public ICommand ChangeBluetoothStatusCommand { get; set; }

        public ObservableCollection<string> ConnectedDevicesList { get; set; }

        public BluetoothPageViewModel(IBluetoothService bluetoothService)
        {
            this._bluetoothService = bluetoothService;

            this.SetUpMessaginCenter();

            IsBluetoothEnabled = this._bluetoothService.IsBluetoothEnabled();
            BluetoothState = this.CreateBluetoothStateLabel();
            IsBluetoothEnabledMessage = this.CreateBluetoothEnabledMessage();

            ChangeBluetoothStatusCommand = new Command(ChangeBluetoothState);

            ConnectedDevicesList = new ObservableCollection<string>();

            PopulateList();
        }

        private void ChangeBluetoothState()
        {
            if (IsBusy) return;

            IsBusy = true;
            this._bluetoothService.ChangeBluetoothState();
            IsBusy = false;
        }

        private void SetUpMessaginCenter()
        {
            MessagingCenter.Subscribe<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, (sender, arg) =>
            {
                this.IsBluetoothEnabled = arg;

                BluetoothState = this.CreateBluetoothStateLabel();
                IsBluetoothEnabledMessage = this.CreateBluetoothEnabledMessage();

                PopulateList();
            });
        }

        private LocalizedString CreateBluetoothEnabledMessage()
        {
            if (IsBluetoothEnabled)
            {
                return new LocalizedString(() => string.Format(AppResources.BluetoothEnabled, DeviceInfo.Name));
            }

            return new LocalizedString(() => string.Format(AppResources.BluetoothDisabled, DeviceInfo.Name));
        }

        private LocalizedString CreateBluetoothStateLabel()
        {
            if (IsBluetoothEnabled)
            {
                return new LocalizedString(() => AppResources.Enabled);
            }

            return new LocalizedString(() => AppResources.Disabled);
        }

        private async Task PopulateList()
        {
            if (isBluetoothEnabled)
            {
                var list = await this._bluetoothService.GetConnectedBluetoothDevices();
                foreach (var device in list)
                {
                    ConnectedDevicesList.Add(device);
                }
            }
        }
    }
}