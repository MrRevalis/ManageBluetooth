using Xamarin.Forms;

namespace ManageBluetooth.Custom
{
    public class RoundedCustomFrame : Frame
    {
        public new CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static new readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(RoundedCustomFrame));

        public RoundedCustomFrame()
        {
            base.CornerRadius = 0;
        }
    }
}