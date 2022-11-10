using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using ManageBluetooth.Extensions;
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
        public ObservableCollection<string> ConnectedDevicesList { get; set; }


        private bool isBluetoothEnabled;
        public bool IsBluetoothEnabled
        {
            get => isBluetoothEnabled;
            set
            {
                if (value == isBluetoothEnabled)
                {
                    return;
                }
                SetProperty(ref isBluetoothEnabled, value);
                UpdateBluetoothProperties();
            }
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
        public ICommand PageAppearingCommand { get; set; }

        public BluetoothPageViewModel(IBluetoothService bluetoothService)
        {
            this._bluetoothService = bluetoothService;

            ConnectedDevicesList = new ObservableCollection<string>();
            IsBluetoothEnabled = this._bluetoothService.IsBluetoothEnabled();

            ChangeBluetoothStatusCommand = new Command(ChangeBluetoothState);
            PageAppearingCommand = new Command(PageAppearing);

            this.SetUpMessaginCenter();
        }

        private void PageAppearing()
        {
            // IsBluetoothEnabled = this._bluetoothService.IsBluetoothEnabled();
        }

        private void UpdateBluetoothProperties()
        {
            this.CreateBluetoothStateLabel();
            this.CreateBluetoothEnabledMessage();
            this.PopulateList();
        }

        private void ChangeBluetoothState()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            this._bluetoothService.ChangeBluetoothState();
            IsBusy = false;
        }

        private void SetUpMessaginCenter()
        {
            MessagingCenter.Subscribe<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, (sender, arg) =>
            {
                this.IsBluetoothEnabled = arg;
            });
        }

        private void CreateBluetoothEnabledMessage()
        {
            if (this._bluetoothService.IsBluetoothEnabled())
            {
                this.IsBluetoothEnabledMessage = new LocalizedString(() => string.Format(AppResources.BluetoothEnabled, DeviceInfo.Name));
            }
            else
            {
                this.IsBluetoothEnabledMessage = new LocalizedString(() => string.Format(AppResources.BluetoothDisabled, DeviceInfo.Name));
            }
        }

        private void CreateBluetoothStateLabel()
        {
            if (this._bluetoothService.IsBluetoothEnabled())
            {
                this.BluetoothState = new LocalizedString(() => AppResources.Enabled);
            }
            else
            {
                this.BluetoothState = new LocalizedString(() => AppResources.Disabled);
            }
        }

        private async Task PopulateList()
        {
            if (this._bluetoothService.IsBluetoothEnabled())
            {
                var connectedOrKnowDevices = await this._bluetoothService.GetConnectedOrKnowBluetoothDevices();
                ConnectedDevicesList.AddRange(connectedOrKnowDevices);
            }
            else
            {
                ConnectedDevicesList.Clear();
            }
        }
    }
}