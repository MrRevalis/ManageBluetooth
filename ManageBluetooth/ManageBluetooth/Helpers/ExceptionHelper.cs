using System;

using ManageBluetooth.Interface;
using ManageBluetooth.Models.Enum;
using ManageBluetooth.Resources;

using Xamarin.Forms;

namespace ManageBluetooth.Helpers
{
    public static class ExceptionHelper
    {
        public static void CatchException(this Action task)
        {
            try
            {
                task.Invoke();
            }
            catch (Exception e)
            {
                ShowExceptionToast(e.Message);
            }
        }

        public static void ShowExceptionToast(string error)
        {
            var toastService = DependencyService.Get<IToastService>();

            if (Enum.TryParse(error, out ErrorEnum result))
            {
                switch (result)
                {
                    case ErrorEnum.UnknownError:
                        toastService.ShortAlert(AppResources.UnknownError);
                        break;
                    case ErrorEnum.DeviceCannotBeFound:
                        toastService.ShortAlert(AppResources.DeviceCannotBeFound);
                        break;
                    case ErrorEnum.CannotConnectToTheDevice:
                        toastService.ShortAlert(AppResources.CannotConnectToTheDevice);
                        break;
                    case ErrorEnum.CannotChangeDeviceAlias:
                        toastService.ShortAlert(AppResources.CannotChangeDeviceAlias);
                        break;
                    case ErrorEnum.ErrorDisconnecting:
                        toastService.ShortAlert(AppResources.ErrorDisconnecting);
                        break;
                }
            }
            else
            {
                toastService.ShortAlert(AppResources.UnknownError);
            }
        }
    }
}