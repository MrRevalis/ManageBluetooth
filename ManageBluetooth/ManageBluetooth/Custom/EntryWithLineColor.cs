using Xamarin.Forms;

namespace ManageBluetooth.Custom
{
    public class EntryWithLineColor : Entry
    {
        public Color LineColor
        {
            get => (Color)GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }

        public static readonly BindableProperty LineColorProperty =
            BindableProperty.Create(nameof(LineColor), typeof(Color), typeof(EntryWithLineColor));

        public Color LineColorFocused
        {
            get => (Color)GetValue(LineColorFocusedProperty);
            set => SetValue(LineColorFocusedProperty, value);
        }

        public static readonly BindableProperty LineColorFocusedProperty =
            BindableProperty.Create(nameof(LineColorFocused), typeof(Color), typeof(EntryWithLineColor));
    }
}