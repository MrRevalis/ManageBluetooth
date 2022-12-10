using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using ManageBluetooth.Extensions;
using ManageBluetooth.Interface;
using ManageBluetooth.Models;
using ManageBluetooth.Models.Constants;
using ManageBluetooth.Models.Enum;
using ManageBluetooth.Resources;
using ManageBluetooth.ViewModels.Base;

using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ManageBluetooth.ViewModels
{
    public class BluetoothPageViewModel : BaseViewModel
    {
        private readonly IBluetoothService _bluetoothService;
        private readonly IToastService _toastService;

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
        public ICommand ConnectWithBluetoothDeviceCommand { get; set; }
        public ICommand BondWithBluetoothDeviceCommand { get; set; }

        private bool isConnectingWithDevice;

        public BluetoothPageViewModel(IBluetoothService bluetoothService)
        {
            this._bluetoothService = bluetoothService;
            this._toastService = DependencyService.Get<IToastService>();

            this.BondedDevicesList = new ObservableCollection<SimpleBluetoothDevice>();
            this.AvailableDevicesList = new ObservableCollection<SimpleBluetoothDevice>();

            this.ChangeBluetoothStatusCommand = new Command(ChangeBluetoothState);
            this.StartStopScanningCommand = new Command(StartStopScanning);
            this.ConnectWithBluetoothDeviceCommand = new Command<SimpleBluetoothDevice>((device) => ConnectWithBluetoothDevice(device));
            this.BondWithBluetoothDeviceCommand = new Command<SimpleBluetoothDevice>((device) => BondWithBluetoothDevice(device));
            this.PageAppearingCommand = new Command(PageAppearing);

            this.SetUpMessaginCenter();
        }

        private void BondWithBluetoothDevice(SimpleBluetoothDevice device)
        {
            this._bluetoothService.ConnectWithBluetoothDevice(device);
        }

        private void ConnectWithBluetoothDevice(SimpleBluetoothDevice device)
        {
            if (!this._bluetoothService.IsBluetoothEnabled()
                || this.isConnectingWithDevice
                || device == null)
            {
                return;
            }

            if (device.DeviceState == BluetoothDeviceConnectionStateEnum.Connected)
            {
                device.DeviceState = BluetoothDeviceConnectionStateEnum.Disconnecting;
                this._bluetoothService.DisconnectWithBluetoothDevice();
            }
            else
            {
                this.isConnectingWithDevice = true;
                device.DeviceState = BluetoothDeviceConnectionStateEnum.Connecting;

                this._bluetoothService.ConnectWithBluetoothDevice(device);
            }
        }

        private void PageAppearing()
        {
            IsBusy = true;
            this.IsBluetoothEnabled = _bluetoothService.IsBluetoothEnabled();
            this.UpdateBluetoothProperties();
            this.StartStopBluetoothScanning();
            IsBusy = false;
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
            this.UpdateBluetoothProperties();

            IsBusy = false;
        }

        private void StartBluetoothScan()
        {
            if (this._bluetoothService.IsBluetoothScanning())
            {
                return;
            }

            this._bluetoothService.StartScanningForBluetoothDevices();

            this.BondedDevicesList.Clear();
            this.BondedDevicesList.AddRange(this._bluetoothService.GetBondedBluetoothDevices().OrderBy(x => (int)x.DeviceState));
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
            if (this.AvailableDevicesList.Any(x => x.DeviceId.Equals(device.DeviceId)))
            {
                return;
            }

            this.AvailableDevicesList.Add(device);
        }


        private void UpdateBluetoothDeviceProperty(UpdateBluetoothConnectionStatusModel updateModel)
        {
            var device = this.BondedDevicesList
                .FirstOrDefault(x => x.DeviceId == updateModel.DeviceId && x.DeviceState != updateModel.DeviceState);

            if (device == null)
            {
                return;
            }

            device.DeviceState = updateModel.DeviceState;
            this.BondedDevicesList = new ObservableCollection<SimpleBluetoothDevice>(this.BondedDevicesList);
        }

        private void UpdateBluetoothDeviceProperty(UpdateBluetoothBondStatusModel updateModel)
        {
            if (updateModel.IsBonded)
            {
                var device = this.AvailableDevicesList.FirstOrDefault(x => x.DeviceId == updateModel.DeviceId);
                if (device == null)
                {
                    return;
                }

                device.IsBonded = updateModel.IsBonded;
                this.AvailableDevicesList.Remove(device);
                this.BondedDevicesList.Add(device);
            }
            else
            {
                var device = this.BondedDevicesList.FirstOrDefault(x => x.DeviceId == updateModel.DeviceId);
                if (device == null)
                {
                    return;
                }

                device.IsBonded = updateModel.IsBonded;
                this.BondedDevicesList.Remove(device);
                this.AvailableDevicesList.Add(device);
            }
        }


        private void SetUpMessaginCenter()
        {
            MessagingCenter.Subscribe<Application, bool>(Application.Current, BluetoothCommandConstants.BluetootStateChanged, (sender, arg) =>
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
                if (this.IsBluetoothScanning != arg)
                {
                    this.IsBluetoothScanning = arg;
                    this.UpdateStartStopScanLabel();
                }
            });

            MessagingCenter.Subscribe<Application, string>(Application.Current, BluetoothCommandConstants.ConnectBluetoothDevice, (sender, arg) =>
            {
                this.ConnectWithBluetoothDevice(this.BondedDevicesList.FirstOrDefault(x => x.DeviceId == arg));
            });

            MessagingCenter.Subscribe<Application, UpdateBluetoothConnectionStatusModel>(Application.Current, BluetoothCommandConstants.BluetoothDeviceConnectionStateChanged, (sender, arg) =>
            {
                if (arg.DeviceState == BluetoothDeviceConnectionStateEnum.Connected)
                {
                    this.isConnectingWithDevice = false;
                }
                else if (arg.DeviceState == BluetoothDeviceConnectionStateEnum.Connecting)
                {
                    this.isConnectingWithDevice = true;
                }

                switch (arg.DeviceState)
                {
                    case BluetoothDeviceConnectionStateEnum.Connected:
                        this.isConnectingWithDevice = true;
                        break;
                    case BluetoothDeviceConnectionStateEnum.Connecting:
                        this.isConnectingWithDevice = false;
                        break;
                    case BluetoothDeviceConnectionStateEnum.Disconnected:
                        this._toastService.ShortAlert(string.Format(AppResources.BluetoothEnabled, arg.DeviceName));
                        break;
                }

                this.UpdateBluetoothDeviceProperty(arg);
            });

            MessagingCenter.Subscribe<Application, UpdateBluetoothBondStatusModel>(Application.Current, BluetoothCommandConstants.BluetoothDeviceBondStateChanged, (sender, arg) =>
            {
                this.UpdateBluetoothDeviceProperty(arg);
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