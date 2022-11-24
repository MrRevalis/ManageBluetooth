using System.Collections.ObjectModel;
using System.Windows.Input;

using ManageBluetooth.Extensions;
using ManageBluetooth.Interface;
using ManageBluetooth.Models;
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

        private ObservableCollection<SimpleBluetoothDevice> connectedDevicesList;
        public ObservableCollection<SimpleBluetoothDevice> ConnectedDevicesList
        {
            get => connectedDevicesList;
            set => SetProperty(ref connectedDevicesList, value);
        }

        private ObservableCollection<SimpleBluetoothDevice> availableDevicesList;
        public ObservableCollection<SimpleBluetoothDevice> AvailableDevicesList
        {
            get => availableDevicesList;
            set => SetProperty(ref availableDevicesList, value);
        }

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

        private bool isBluetoothScanning;
        public bool IsBluetoothScanning
        {
            get => isBluetoothScanning;
            set
            {
                if (value == isBluetoothScanning)
                {
                    return;
                }
                SetProperty(ref isBluetoothScanning, value);
                UpdateStartStopScanLabel();
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

        private LocalizedString startStopScan;
        public LocalizedString StartStopScan
        {
            get => startStopScan;
            set => SetProperty(ref startStopScan, value);
        }

        public ICommand ChangeBluetoothStatusCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }
        public ICommand StartStopScanningCommand { get; set; }
        public ICommand ConnectWithUnknownDeviceCommand { get; set; }

        public BluetoothPageViewModel(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;

            ConnectedDevicesList = new ObservableCollection<SimpleBluetoothDevice>();
            AvailableDevicesList = new ObservableCollection<SimpleBluetoothDevice>();

            IsBluetoothEnabled = _bluetoothService.IsBluetoothEnabled();

            ChangeBluetoothStatusCommand = new Command(ChangeBluetoothState);
            StartStopScanningCommand = new Command(StartStopScanning);
            ConnectWithUnknownDeviceCommand = new Command<SimpleBluetoothDevice>((device) => ConnectWithUnknownDevice(device));

            UpdateBluetoothProperties();
            StartStopBluetoothScanning();
            SetUpMessaginCenter();
        }

        private async void ConnectWithUnknownDevice(SimpleBluetoothDevice device)
        {
            if (!IsBluetoothEnabled
                || device == null)
            {
                return;
            }

            await this._bluetoothService.ConnectWithUnknownDevice(device.DeviceId);

        }

        private void StartStopScanning()
        {
            if (isBluetoothScanning)
            {
                _bluetoothService.StopScanningForBluetoothDevices();
                IsBluetoothScanning = false;
            }
            else
            {
                _bluetoothService.StartScanningForBluetoothDevices();
                IsBluetoothScanning = true;
            }
        }

        private void UpdateBluetoothProperties()
        {
            if (IsBluetoothEnabled)
            {
                IsBluetoothEnabledMessage = new LocalizedString(() => string.Format(AppResources.BluetoothEnabled, DeviceInfo.Name));
                BluetoothState = new LocalizedString(() => AppResources.Enabled);
            }
            else
            {
                IsBluetoothEnabledMessage = new LocalizedString(() => AppResources.BluetoothDisabled);
                BluetoothState = new LocalizedString(() => AppResources.Disabled);
            }
        }

        private void UpdateStartStopScanLabel()
        {
            if (IsBluetoothScanning)
            {
                StartStopScan = new LocalizedString(() => AppResources.Stop);
            }
            else
            {
                StartStopScan = new LocalizedString(() => AppResources.Scan);
            }
        }

        private void StartStopBluetoothScanning()
        {
            if (IsBluetoothEnabled)
            {
                this.StartBluetoothScan();
            }
            else
            {
                this.StopBluetoothScan();
            }
        }

        private void ChangeBluetoothState()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            _bluetoothService.ChangeBluetoothState();
            IsBusy = false;
        }

        private void SetUpMessaginCenter()
        {
            MessagingCenter.Subscribe<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, (sender, arg) =>
            {
                IsBluetoothEnabled = arg;
                StartStopBluetoothScanning();
            });

            MessagingCenter.Subscribe<BluetoothService, SimpleBluetoothDevice>(this, BluetoothCommandConstants.BluetootDeviceDiscovered, (sender, arg) =>
            {
                AvailableDevicesList.Add(arg);
                AvailableDevicesList = new ObservableCollection<SimpleBluetoothDevice>(AvailableDevicesList);
            });

            MessagingCenter.Subscribe<BluetoothService, bool>(this, BluetoothCommandConstants.BluetoothScanTimeoutElapsed, (sender, arg) =>
            {
                IsBluetoothScanning = arg;
            });
        }

        private void StartBluetoothScan()
        {
            if (this._bluetoothService.IsBluetoothScanning()
                || this.IsBluetoothScanning)
            {
                return;
            }

            var connectedOrKnowDevices = this._bluetoothService.GetConnectedOrKnowBluetoothDevices();
            ConnectedDevicesList.AddRange(connectedOrKnowDevices);

            this._bluetoothService.StartScanningForBluetoothDevices();
            this.IsBluetoothScanning = true;

            ConnectedDevicesList = new ObservableCollection<SimpleBluetoothDevice>(ConnectedDevicesList);
        }

        private void StopBluetoothScan()
        {
            if (this._bluetoothService.IsBluetoothScanning()
                || this.isBluetoothScanning)
            {
                this.IsBluetoothScanning = false;
                _bluetoothService.StopScanningForBluetoothDevices();
            }

            AvailableDevicesList.Clear();
            ConnectedDevicesList.Clear();
        }
    }
}