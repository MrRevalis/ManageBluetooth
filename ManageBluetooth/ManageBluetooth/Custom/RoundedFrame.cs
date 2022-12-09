using Xamarin.Forms;

namespace ManageBluetooth.Custom
{
    public class RoundedFrame : Frame
    {
        public new CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static new readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(RoundedFrame));

        public RoundedFrame()
        {
            base.CornerRadius = 0;
        }
    }
}