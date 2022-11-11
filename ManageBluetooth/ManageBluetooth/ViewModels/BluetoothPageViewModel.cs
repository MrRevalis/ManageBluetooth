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
        public ObservableCollection<string> ConnectedDevicesList { get; set; }
        public ObservableCollection<SimpleBluetoothDevice> AvailableDevices { get; set; }

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

        private LocalizedString startStopScan;
        public LocalizedString StartStopScan
        {
            get => startStopScan;
            set => SetProperty(ref startStopScan, value);
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
                // zmienic na inna metode, zeby wszystko nie było robione eeee
                UpdateBluetoothProperties();
            }
        }

        public ICommand ChangeBluetoothStatusCommand { get; set; }
        public ICommand PageAppearingCommand { get; set; }

        public ICommand StartStopScanningCommand { get; set; }

        public BluetoothPageViewModel(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;

            ConnectedDevicesList = new ObservableCollection<string>();
            AvailableDevices = new ObservableCollection<SimpleBluetoothDevice>();

            IsBluetoothEnabled = _bluetoothService.IsBluetoothEnabled();

            ChangeBluetoothStatusCommand = new Command(ChangeBluetoothState);
            PageAppearingCommand = new Command(PageAppearing);
            StartStopScanningCommand = new Command(StartStopScanning);

            UpdateBluetoothProperties();
            PopulateList();
            SetUpMessaginCenter();
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
                IsBluetoothScanning = true;
                _bluetoothService.StartScanningForBluetoothDevices();
            }
        }

        private void PageAppearing()
        {
            // IsBluetoothEnabled = this._bluetoothService.IsBluetoothEnabled();
            // IsBluetoothScanning = false;
        }

        private void UpdateBluetoothProperties()
        {
            if (IsBluetoothEnabled)
            {
                IsBluetoothEnabledMessage = new LocalizedString(() => string.Format(AppResources.BluetoothEnabled, DeviceInfo.Name));
                BluetoothState = new LocalizedString(() => AppResources.Enabled);

                if (IsBluetoothScanning)
                {
                    StartStopScan = new LocalizedString(() => AppResources.Stop);
                }
                else
                {
                    StartStopScan = new LocalizedString(() => AppResources.Scan);
                }
            }
            else
            {
                IsBluetoothEnabledMessage = new LocalizedString(() => AppResources.BluetoothDisabled);
                BluetoothState = new LocalizedString(() => AppResources.Disabled);
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
                PopulateList();
            });

            MessagingCenter.Subscribe<BluetoothService, SimpleBluetoothDevice>(this, BluetoothCommandConstants.BluetootDeviceDiscovered, (sender, arg) =>
            {
                AvailableDevices.Add(arg);
            });

            MessagingCenter.Subscribe<BluetoothService, bool>(this, BluetoothCommandConstants.BluetoothScanTimeoutElapsed, (sender, arg) =>
            {
                IsBluetoothScanning = arg;
            });
        }

        private void PopulateList()
        {
            if (IsBluetoothEnabled)
            {
                var connectedOrKnowDevices = this._bluetoothService.GetConnectedOrKnowBluetoothDevices();
                IsBluetoothScanning = true;
                _bluetoothService.StartScanningForBluetoothDevices();
                ConnectedDevicesList.AddRange(connectedOrKnowDevices);
            }
            else
            {
                _bluetoothService.StopScanningForBluetoothDevices();
                IsBluetoothScanning = false;
                ConnectedDevicesList.Clear();
                AvailableDevices.Clear();
            }
        }
    }
}