using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using ManageBluetooth.Custom.Collection;
using ManageBluetooth.Extensions;
using ManageBluetooth.Helpers;
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
        public ICommand PageDisappearingCommand { get; set; }
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

            this.PageAppearingCommand = new Command(PageAppearing);
            this.PageDisappearingCommand = new Command(PageDisappearing);

            this.ChangeBluetoothStatusCommand = new Command(ChangeBluetoothState);
            this.StartStopScanningCommand = new Command(StartStopScanning);
            this.ConnectWithBluetoothDeviceCommand = new Command<SimpleBluetoothDevice>((device) => ConnectWithBluetoothDevice(device));
            this.BondWithBluetoothDeviceCommand = new Command<SimpleBluetoothDevice>((device) => BondWithBluetoothDevice(device));

            this.IsBluetoothEnabled = _bluetoothService.IsBluetoothEnabled();
            this.UpdateBluetoothProperties();
            this.UpdateStartStopScanLabel();
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
                ExceptionHelper.CatchException(() => this._bluetoothService.DisconnectWithBluetoothDevice());

            }
            else
            {
                this._bluetoothService.ConnectWithBluetoothDevice(device);
            }
        }

        private void PageAppearing()
        {
            this.SubscribeMessagingCenter();
            this.StartStopBluetoothScanning();
        }

        private void PageDisappearing()
        {
            UnsubscribeMessagingCenter();
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
            if (!CanExecute)
            {
                return;
            }

            CanExecute = false;

            this._bluetoothService.ChangeBluetoothState();
            this.UpdateBluetoothProperties();

            CanExecute = true;
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
        }

        private void StopBluetoothScan()
        {
            if (this._bluetoothService.IsBluetoothScanning())
            {
                this._bluetoothService.StopScanningForBluetoothDevices();
            }

            this.Devices.Clear();
        }

        private void AddDiscoveredDevice(SimpleBluetoothDevice device)
        {
            if (!this.Devices.Any(x => x.DeviceId == device.DeviceId))
            {
                this.Devices.Add(device);
            }
        }


        private void UpdateBluetoothDeviceConnectionState(UpdateBluetoothConnectionStatusModel updateModel)
        {
            switch (updateModel.DeviceState)
            {
                case BluetoothDeviceConnectionStateEnum.Connected:
                    this.isConnectingWithDevice = false;
                    break;
                case BluetoothDeviceConnectionStateEnum.Connecting:
                    this.isConnectingWithDevice = true;
                    break;
                case BluetoothDeviceConnectionStateEnum.Disconnected:
                    this.isConnectingWithDevice = false;
                    this._toastService.ShortAlert(string.Format(AppResources.ClosedConnection, updateModel.DeviceName));
                    break;
                default:
                    this.isConnectingWithDevice = false;
                    break;
            }

            var deviceIndex = this.Devices.FindIndex(x => x.DeviceId == updateModel.DeviceId);
            if (deviceIndex != Constants.NotFoundIndex
                && this.Devices[deviceIndex]?.DeviceState != updateModel.DeviceState)
            {
                var deviceModel = this.Devices[deviceIndex];
                deviceModel.DeviceState = updateModel.DeviceState;

                //var deviceModel = this.Devices
                //    .FirstOrDefault(x => x.DeviceId == updateModel.DeviceId);
                //deviceModel.DeviceState = updateModel.DeviceState;

                if (deviceModel.DeviceState == BluetoothDeviceConnectionStateEnum.Connected
                    && deviceIndex != Constants.FirstIndex)
                {
                    this.Devices.Move(deviceIndex, Constants.FirstIndex);
                }
            }
        }

        private void UpdateBluetoothDeviceBondState(UpdateBluetoothBondStatusModel updateModel)
        {
            var deviceIndex = this.Devices.FindIndex(x => x.DeviceId == updateModel.DeviceId);

            if (deviceIndex != Constants.NotFoundIndex)
            {
                var device = this.Devices[deviceIndex];

                //if (device.IsBonded != updateModel.IsBonded)
                //{
                //    device.IsBonded = updateModel.IsBonded;

                //    var newDevice = new SimpleBluetoothDevice(device);
                //    this.Devices[deviceIndex] = newDevice;
                //}

                if (device.IsBonded != updateModel.IsBonded)
                {
                    if (updateModel.IsBonded)
                    {
                        device.IsBonded = updateModel.IsBonded;

                        var newDevice = new SimpleBluetoothDevice(device);
                        this.Devices[deviceIndex] = newDevice;
                    }
                    else
                    {
                        this.Devices.RemoveAt(deviceIndex);
                    }
                }
            }
        }

        private void SubscribeMessagingCenter()
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
                this.UpdateBluetoothDeviceConnectionState(arg);
            });

            MessagingCenter.Subscribe<Application, UpdateBluetoothBondStatusModel>(Application.Current, BluetoothCommandConstants.BluetoothDeviceBondStateChanged, (sender, arg) =>
            {
                this.UpdateBluetoothDeviceBondState(arg);
            });
        }

        private static void UnsubscribeMessagingCenter()
        {
            MessagingCenter.Unsubscribe<Application, bool>(Application.Current, BluetoothCommandConstants.BluetootStateChanged);

            MessagingCenter.Unsubscribe<Application, SimpleBluetoothDevice>(Application.Current, BluetoothCommandConstants.BluetootDeviceDiscovered);

            MessagingCenter.Unsubscribe<Application, bool>(Application.Current, BluetoothCommandConstants.BluetoothScanningStateChanged);

            MessagingCenter.Unsubscribe<Application, UpdateBluetoothConnectionStatusModel>(Application.Current, BluetoothCommandConstants.BluetoothDeviceConnectionStateChanged);

            MessagingCenter.Unsubscribe<Application, UpdateBluetoothBondStatusModel>(Application.Current, BluetoothCommandConstants.BluetoothDeviceBondStateChanged);
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
                this.isConnectingWithDevice = false;
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