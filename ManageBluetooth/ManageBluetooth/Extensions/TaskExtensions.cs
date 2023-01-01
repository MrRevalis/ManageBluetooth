using System;
using System.Threading.Tasks;

using ManageBluetooth.Helpers;

namespace ManageBluetooth.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<T> ExecuteAsyncOperation<T>(this Task<T> task)
        {
            try
            {
                return await task;
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowExceptionToast(e.Message);

                return default;
            }
        }
    }
}