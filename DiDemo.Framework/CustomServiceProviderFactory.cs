using System;
using Microsoft.Extensions.DependencyInjection;

namespace DiDemo.Framework
{
    public class CustomServiceProviderFactory : IServiceProviderFactory<CustomContainerBuilder>
    {
        public CustomContainerBuilder CreateBuilder(IServiceCollection services)
        {
            return new CustomContainerBuilder(services);

        }

        public IServiceProvider CreateServiceProvider(CustomContainerBuilder containerBuilder)
        {
            return containerBuilder.BuildServiceProvider();
        }
    }
}
