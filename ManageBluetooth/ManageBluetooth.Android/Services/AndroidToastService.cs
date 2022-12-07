using Android.App;
using Android.Widget;

using ManageBluetooth.Droid.Services;
using ManageBluetooth.Interface;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidToastService))]
namespace ManageBluetooth.Droid.Services
{
    internal class AndroidToastService : IToastService
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}