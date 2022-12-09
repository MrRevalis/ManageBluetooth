using System;

using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Widget;

using ManageBluetooth.Custom;
using ManageBluetooth.Droid.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EntryWithLineColor), typeof(EntryWithLineColorRenderer))]
namespace ManageBluetooth.Droid.Renderers
{
    public class EntryWithLineColorRenderer : EntryRenderer
    {
        private EntryWithLineColor _entry;
        public EntryWithLineColorRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null
                || e.NewElement == null)
            {
                return;
            }

            this._entry = this.Element as EntryWithLineColor;
            this._entry.Focused += EntryFocused;
            this._entry.Unfocused += EntryUnfocued;

            /// this.SetTintColor();
            Control.BackgroundTintList = ColorStateList.ValueOf(_entry.LineColor.ToAndroid());
        }

        protected override void Dispose(bool disposing)
        {
            if (this._entry != null)
            {
                this._entry.Focused -= EntryFocused;
                this._entry.Unfocused -= EntryUnfocued;
            }

            base.Dispose(disposing);
        }

        private void SetTintColor()
        {
            IntPtr IntPtrtextViewClass = JNIEnv.FindClass(typeof(TextView));
            IntPtr mCursorDrawableResProperty = JNIEnv.GetFieldID(IntPtrtextViewClass, "mCursorDrawableRes", "I");

            JNIEnv.SetField(Control.Handle, mCursorDrawableResProperty, Resource.Drawable.CustomCursorColor);
        }

        private void EntryUnfocued(object sender, FocusEventArgs e)
        {
            Control.BackgroundTintList = ColorStateList.ValueOf(_entry.LineColor.ToAndroid());
        }

        private void EntryFocused(object sender, FocusEventArgs e)
        {
            Control.BackgroundTintList = ColorStateList.ValueOf(_entry.LineColorFocused.ToAndroid());
        }
    }
}