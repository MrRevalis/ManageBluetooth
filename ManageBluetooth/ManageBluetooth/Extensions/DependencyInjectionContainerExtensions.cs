using ManageBluetooth.Interface;
using ManageBluetooth.Services;
using ManageBluetooth.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace ManageBluetooth.Extensions
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IBluetoothService, BluetoothService>();

            return services;
        }

        public static IServiceCollection ConfigureViewModels(this IServiceCollection services)
        {
            services.AddTransient<BluetoothPageViewModel>();
            services.AddTransient<BluetoothDevicePageViewModel>();

            return services;
        }
    }
}