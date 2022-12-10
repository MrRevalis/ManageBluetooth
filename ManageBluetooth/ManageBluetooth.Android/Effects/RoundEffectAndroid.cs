using System;

using Android.Graphics;
using Android.Views;

using ManageBluetooth.Custom.Effects;
using ManageBluetooth.Droid.Effects;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Rect = Android.Graphics.Rect;

[assembly: ExportEffect(typeof(RoundEffectAndroid), nameof(RoundEffect))]
namespace ManageBluetooth.Droid.Effects
{
    public class RoundEffectAndroid : PlatformEffect
    {
        ViewOutlineProvider originalProvider;
        Android.Views.View effectTarget;

        protected override void OnAttached()
        {
            try
            {
                effectTarget = Control ?? Container;
                originalProvider = effectTarget.OutlineProvider;

                effectTarget.OutlineProvider = new CornerRadiusOutlineProvider(Element);
                effectTarget.ClipToOutline = true;
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnDetached()
        {
            if (effectTarget != null)
            {
                effectTarget.OutlineProvider = originalProvider;
                effectTarget.ClipToOutline = false;
            }
        }
    }

    class CornerRadiusOutlineProvider : ViewOutlineProvider
    {
        Element element;

        public CornerRadiusOutlineProvider(Element formsElement)
        {
            element = formsElement;
        }

        public override void GetOutline(Android.Views.View view, Outline outline)
        {
            var scale = view.Resources.DisplayMetrics.Density;

            var width = (double)element.GetValue(VisualElement.WidthProperty) * scale;
            var height = (double)element.GetValue(VisualElement.HeightProperty) * scale;

            var minDimension = (float)Math.Min(height, width);
            var radius = minDimension / 2f;

            Rect rect = new Rect(0, 0, (int)width, (int)height);

            outline.SetRoundRect(rect, radius);
        }
    }
}