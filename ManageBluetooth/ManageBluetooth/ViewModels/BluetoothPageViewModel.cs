using System.Windows.Input;

using ManageBluetooth.Interface;
using ManageBluetooth.Models.Constants;
using ManageBluetooth.Services;
using ManageBluetooth.ViewModels.Base;

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
        public bool IsBluetoothScanning { get; set; }

        public ICommand ChangeBluetoothStatus { get; set; }

        public BluetoothPageViewModel(IBluetoothService bluetoothService)
        {
            this._bluetoothService = bluetoothService;

            this.SetUpMessaginCenter();

            IsBluetoothEnabled = this._bluetoothService.IsBluetoothEnabled();
        }

        private void SetUpMessaginCenter()
        {
            MessagingCenter.Subscribe<BluetoothService, bool>(this, BluetoothCommandConstants.BluetootStateChanged, (sender, arg) =>
            {
                this.IsBluetoothEnabled = arg;
            });
        }
    }
}