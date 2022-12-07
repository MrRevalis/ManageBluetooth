using System.Collections.ObjectModel;
using System.Windows.Input;

using ManageBluetooth.Extensions;
using ManageBluetooth.Models;
using ManageBluetooth.Resources;

using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ManageBluetooth.Custom.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothDevicesList : StackLayout
    {
        public static readonly BindableProperty DevicesProperty =
            BindableProperty.Create(
                nameof(Devices),
                typeof(ObservableCollection<SimpleBluetoothDevice>),
                typeof(BluetoothDevicesList),
                propertyChanged: DevicesListChanged);

        private static void DevicesListChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as BluetoothDevicesList;

            var devices = (newValue as ObservableCollection<SimpleBluetoothDevice>);
            devices.SortByDescending(x => x.DeviceState);

            BindableLayout.SetItemsSource(control.@this, devices);
        }

        public ObservableCollection<SimpleBluetoothDevice> Devices
        {
            get => GetValue(DevicesProperty) as ObservableCollection<SimpleBluetoothDevice>;
            set => SetValue(DevicesProperty, value);
        }

        public static readonly BindableProperty ConnectWithDeviceCommandProperty =
            BindableProperty.Create(
                nameof(ConnectWithDeviceCommand),
                typeof(ICommand),
                typeof(BluetoothDevicesList));

        public ICommand ConnectWithDeviceCommand
        {
            get => GetValue(ConnectWithDeviceCommandProperty) as ICommand;
            set => SetValue(ConnectWithDeviceCommandProperty, value);
        }

        public LocalizedString Connecting { get; set; }
        public LocalizedString Connected { get; set; }
        public LocalizedString Disconnecting { get; set; }
        public LocalizedString LinkingError { get; set; }
        public BluetoothDevicesList()
        {
            InitializeComponent();

            this.Connecting = new(() => AppResources.Connecting);
            this.Connected = new(() => AppResources.Connected);
            this.Disconnecting = new(() => AppResources.Disconnecting);
            this.LinkingError = new(() => AppResources.LinkingError);
        }
    }
}