using Android.Content;
using Android.Graphics.Drawables;

using ManageBluetooth.Custom;
using ManageBluetooth.Droid.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using FrameRenderer = Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer;

[assembly: ExportRenderer(typeof(RoundedCustomFrame), typeof(CustomFrameRenderer))]
namespace ManageBluetooth.Droid.Renderers
{
    public class CustomFrameRenderer : FrameRenderer
    {
        public CustomFrameRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || e.NewElement == null)
            {
                return;
            }

            UpdateCornerRadius();
        }

        private void UpdateCornerRadius()
        {
            RoundedCustomFrame roundedCustomFrame = this.Element as RoundedCustomFrame;
            if (roundedCustomFrame != null)
            {
                var cornerRadius = roundedCustomFrame.CornerRadius;
                if (cornerRadius == null)
                {
                    return;
                }

                var topLeft = Context.ToPixels(roundedCustomFrame.CornerRadius.TopLeft);
                var topRight = Context.ToPixels(roundedCustomFrame.CornerRadius.TopRight);
                var bottomLeft = Context.ToPixels(roundedCustomFrame.CornerRadius.BottomLeft);
                var bottomRight = Context.ToPixels(roundedCustomFrame.CornerRadius.BottomRight);

                var cornerRadii = new float[]
                {
                    topLeft,topLeft,
                    topRight,topRight,
                    bottomLeft,bottomLeft,
                    bottomRight,bottomRight,
                };

                (Control.Background as GradientDrawable).SetCornerRadii(cornerRadii);
            }
        }
    }
}