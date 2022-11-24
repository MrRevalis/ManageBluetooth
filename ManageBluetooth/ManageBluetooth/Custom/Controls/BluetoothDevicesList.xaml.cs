using System.Collections.ObjectModel;
using System.Windows.Input;

using ManageBluetooth.Models;

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
                default(ObservableCollection<SimpleBluetoothDevice>));

        public ObservableCollection<SimpleBluetoothDevice> Devices
        {
            get => GetValue(DevicesProperty) as ObservableCollection<SimpleBluetoothDevice>;
            set => SetValue(DevicesProperty, value);
        }

        public static readonly BindableProperty ConnectWithDeviceCommandProperty =
            BindableProperty.Create(
                nameof(ConnectWithDeviceCommand),
                typeof(ICommand),
                typeof(BluetoothDevicesList),
                default(ICommand));

        public ICommand ConnectWithDeviceCommand
        {
            get => GetValue(ConnectWithDeviceCommandProperty) as ICommand;
            set => SetValue(ConnectWithDeviceCommandProperty, value);
        }

        public BluetoothDevicesList()
        {
            InitializeComponent();
        }
    }
}