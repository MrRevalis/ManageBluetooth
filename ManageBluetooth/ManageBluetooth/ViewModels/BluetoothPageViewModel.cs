using System.Collections.ObjectModel;
using System.Linq;
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
        public ObservableCollection<SimpleBluetoothDevice> BondedDevicesList
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
                this.UpdateStartStopScanLabel();
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
            this._bluetoothService = bluetoothService;

            this.BondedDevicesList = new ObservableCollection<SimpleBluetoothDevice>();
            this.AvailableDevicesList = new ObservableCollection<SimpleBluetoothDevice>();

            this.ChangeBluetoothStatusCommand = new Command(ChangeBluetoothState);
            this.StartStopScanningCommand = new Command(StartStopScanning);
            // ConnectWithUnknownDeviceCommand = new Command<SimpleBluetoothDevice>((device) => ConnectWithUnknownDevice(device));
            this.PageAppearingCommand = new Command(PageAppearing);

            this.IsBluetoothEnabled = _bluetoothService.IsBluetoothEnabled();
            this.SetUpMessaginCenter();
            this.UpdateBluetoothProperties();
            this.StartStopBluetoothScanning();
        }

        private void PageAppearing()
        {
            // StartStopBluetoothScanning();
        }

        private void StartStopScanning()
        {
            if (this._bluetoothService.IsBluetoothScanning())
            {
                this._bluetoothService.StopScanningForBluetoothDevices();
            }
            else
            {
                this._bluetoothService.StartScanningForBluetoothDevices();
            }
        }

        private void StartStopBluetoothScanning()
        {
            if (this._bluetoothService.IsBluetoothEnabled())
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
            this._bluetoothService.ChangeBluetoothState();
            IsBusy = false;
        }

        private void StartBluetoothScan()
        {
            if (this._bluetoothService.IsBluetoothScanning())
            {
                return;
            }
            this._bluetoothService.StartScanningForBluetoothDevices();

            this.BondedDevicesList.AddRange(this._bluetoothService.GetBondedBluetoothDevices());
            this.BondedDevicesList = new ObservableCollection<SimpleBluetoothDevice>(BondedDevicesList);
        }

        private void StopBluetoothScan()
        {
            if (this._bluetoothService.IsBluetoothScanning())
            {
                this._bluetoothService.StopScanningForBluetoothDevices();
            }

            this.AvailableDevicesList.Clear();
            this.BondedDevicesList.Clear();
        }

        private void AddDiscoveredDevice(SimpleBluetoothDevice device)
        {
            if (this.availableDevicesList.Any(x => x.DeviceId.Equals(device.DeviceId)))
            {
                return;
            }

            this.AvailableDevicesList.Add(device);
            this.AvailableDevicesList = new ObservableCollection<SimpleBluetoothDevice>(AvailableDevicesList);
        }


        private void SetUpMessaginCenter()
        {
            MessagingCenter.Subscribe<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, (sender, arg) =>
            {
                this.IsBluetoothEnabled = arg;
                StartStopBluetoothScanning();
            });

            MessagingCenter.Subscribe<Application, SimpleBluetoothDevice>(Application.Current, BluetoothCommandConstants.BluetootDeviceDiscovered, (sender, arg) =>
            {
                this.AddDiscoveredDevice(arg);
            });

            MessagingCenter.Subscribe<Application, bool>(Application.Current, BluetoothCommandConstants.BluetoothScanningStateChanged, (sender, arg) =>
            {
                this.IsBluetoothScanning = arg;
            });
        }

        private void UpdateBluetoothProperties()
        {
            if (this.IsBluetoothEnabled)
            {
                this.IsBluetoothEnabledMessage = new LocalizedString(() => string.Format(AppResources.BluetoothEnabled, DeviceInfo.Name));
                this.BluetoothState = new LocalizedString(() => AppResources.Enabled);
            }
            else
            {
                this.IsBluetoothEnabledMessage = new LocalizedString(() => AppResources.BluetoothDisabled);
                this.BluetoothState = new LocalizedString(() => AppResources.Disabled);
            }
        }

        private void UpdateStartStopScanLabel()
        {
            if (this.IsBluetoothScanning)
            {
                this.StartStopScan = new LocalizedString(() => AppResources.Stop);
            }
            else
            {
                this.StartStopScan = new LocalizedString(() => AppResources.Scan);
            }
        }
    }
}