using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using ManageBluetooth.Custom.Collection;
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

        public ObservableCollection<SimpleBluetoothDevice> Devices { get; set; }
        public FilteredObservableCollection<SimpleBluetoothDevice> BondedDevicesList { get; set; }
        public FilteredObservableCollection<SimpleBluetoothDevice> AvailableDevicesList { get; set; }

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

            this.Devices = new ObservableCollection<SimpleBluetoothDevice>();
            this.BondedDevicesList = new FilteredObservableCollection<SimpleBluetoothDevice>(this.Devices, x => x.IsBonded);
            this.AvailableDevicesList = new FilteredObservableCollection<SimpleBluetoothDevice>(this.Devices, x => !x.IsBonded);

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
                this._toastService.LongAlert(AppResources.LinkingError);
                return;
            }

            if (device.DeviceState == BluetoothDeviceConnectionStateEnum.Connected)
            {
                this._bluetoothService.DisconnectWithBluetoothDevice();
            }
            else
            {
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

            this.Devices.Clear();
            this.Devices.AddRange(this._bluetoothService.GetBondedBluetoothDevices().OrderByDescending(x => (int)x.DeviceState));

            //this.BondedDevicesList.Clear();
            //this.BondedDevicesList.AddRange(this._bluetoothService.GetBondedBluetoothDevices().OrderBy(x => (int)x.DeviceState));
        }

        private void StopBluetoothScan()
        {
            if (this._bluetoothService.IsBluetoothScanning())
            {
                this._bluetoothService.StopScanningForBluetoothDevices();
            }

            this.Devices.Clear();
            //this.AvailableDevicesList.Clear();
            //this.BondedDevicesList.Clear();
        }

        private void AddDiscoveredDevice(SimpleBluetoothDevice device)
        {
            //if (this.AvailableDevicesList.Any(x => x.DeviceId.Equals(device.DeviceId)))
            //{
            //    return;
            //}

            //this.AvailableDevicesList.Add(device);

            if (!this.Devices.Any(x => x.DeviceId == device.DeviceId))
            {
                this.Devices.Add(device);
            }
        }


        private void UpdateBluetoothDeviceProperty(UpdateBluetoothConnectionStatusModel updateModel)
        {
            //var device = this.BondedDevicesList
            //    .FirstOrDefault(x => x.DeviceId == updateModel.DeviceId && x.DeviceState != updateModel.DeviceState);

            //if (device == null)
            //{
            //    return;
            //}

            //device.DeviceState = updateModel.DeviceState;
            //this.BondedDevicesList = new FilteredObservableCollection<SimpleBluetoothDevice>(this.BondedDevicesList);

            var device = this.Devices
                .FirstOrDefault(x => x.DeviceId == updateModel.DeviceId && x.DeviceState != updateModel.DeviceState);

            if (device != null)
            {
                device.DeviceState = updateModel.DeviceState;
            }
        }

        private void UpdateBluetoothDeviceProperty(UpdateBluetoothBondStatusModel updateModel)
        {
            var deviceIndex = this.Devices.FindIndex(x => x.DeviceId == updateModel.DeviceId);

            if (deviceIndex != -1)
            {
                var device = this.Devices[deviceIndex];

                if (device.IsBonded != updateModel.IsBonded)
                {
                    device.IsBonded = updateModel.IsBonded;

                    var newDevice = new SimpleBluetoothDevice
                    {
                        DeviceId = device.DeviceId,
                        DeviceClass = device.DeviceClass,
                        DeviceName = device.DeviceName,
                        DeviceState = device.DeviceState,
                        IsBonded = updateModel.IsBonded,
                    };

                    this.Devices.Replace(deviceIndex, device);
                    // this.Devices[deviceIndex] = newDevice;
                }
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

            MessagingCenter.Subscribe<Application, UpdateBluetoothConnectionStatusModel>(Application.Current, BluetoothCommandConstants.BluetoothDeviceConnectionStateChanged, (sender, arg) =>
            {
                switch (arg.DeviceState)
                {
                    case BluetoothDeviceConnectionStateEnum.Connected:
                        this.isConnectingWithDevice = false;
                        break;
                    case BluetoothDeviceConnectionStateEnum.Connecting:
                        this.isConnectingWithDevice = true;
                        break;
                    case BluetoothDeviceConnectionStateEnum.Disconnected:
                        this._toastService.ShortAlert(string.Format(AppResources.ClosedConnection, arg.DeviceName));
                        break;
                    default:
                        this.isConnectingWithDevice = false;
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