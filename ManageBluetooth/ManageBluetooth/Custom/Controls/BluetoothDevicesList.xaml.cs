using System.Collections.ObjectModel;

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

        public BluetoothDevicesList()
        {
            InitializeComponent();
        }
    }
}