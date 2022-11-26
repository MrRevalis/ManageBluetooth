using System.Collections.ObjectModel;
using System.Linq;
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
                default(ObservableCollection<SimpleBluetoothDevice>),
                propertyChanged: DevicesListChanged);

        private static void DevicesListChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var newList = newValue as ObservableCollection<SimpleBluetoothDevice>;
            var control = bindable as StackLayout;

            if (!newList.Any())
            {
                control.Margin = new Thickness(0, 10, 0, 10);
            }
            else
            {
                control.Margin = new Thickness(0, 0, 0, 0);
            }
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