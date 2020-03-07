using System;
using System.Linq;
using System.Threading.Tasks;
using DiDemo.Common.Services;
using DiDemo.Common.Services.DemoDependencies;
using Microsoft.Extensions.DependencyInjection;

namespace DiDemo.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<MyCoolService>()
            .AddSingleton<MyOtherCoolService>();

            serviceCollection.AddSingleton<SingletonDependency>()
            .AddScoped<ScopedDependency>()
            .AddTransient<TransientDependency>();

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var tasks = new[]{
                    RunScope(serviceProvider),
                    RunScope(serviceProvider)
                };
                await Task.WhenAll(tasks);
            };

            // var serviceProvider = serviceCollection.BuildServiceProvider(true);
            // var serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions{
            //     ValidateOnBuild = true,
            //     ValidateScopes = true,
            // });
        }

        private static Task RunScope(ServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var myCoolService = scope.ServiceProvider.GetService<MyCoolService>();
                Console.WriteLine(string.Join('\n', myCoolService.GetDescriptions()));
            }
            return Task.CompletedTask;
        }
    }
}
