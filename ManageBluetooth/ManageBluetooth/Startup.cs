using System;

using ManageBluetooth.Extensions;

using Microsoft.Extensions.DependencyInjection;

namespace ManageBluetooth
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static IServiceProvider Init()
        {
            var serviceProvider = new ServiceCollection()
                .ConfigureServices()
                .ConfigureViewModels()
                .BuildServiceProvider();

            ServiceProvider = serviceProvider;

            return serviceProvider;
        }
    }
}
