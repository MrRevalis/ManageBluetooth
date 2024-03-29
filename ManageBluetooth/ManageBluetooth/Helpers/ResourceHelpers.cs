﻿using Xamarin.Forms;

namespace ManageBluetooth.Helpers
{
    public static class ResourceHelpers
    {
        public static T GetResource<T>(string resourceName)
        {
            try
            {
                var success = Application.Current.Resources.TryGetValue(resourceName, out var outValue);
                if (success
                    && outValue is T)
                {
                    return (T)outValue;
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                return default(T);
            }
        }
    }
}
