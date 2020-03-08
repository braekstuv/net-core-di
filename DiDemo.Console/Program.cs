using System;
using System.Threading;
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
            
            //Underlying dependencies will not be disposed.
            serviceCollection.AddSingleton<MyCoolService>()
            .AddSingleton<MyOtherCoolService>();

            //Underlying dependencies will be disposed.
            // serviceCollection.AddScoped<MyCoolService>()
            // .AddScoped<MyOtherCoolService>();

            serviceCollection.AddSingleton<SingletonDependency>()
            .AddScoped<ScopedDependency>()
            .AddTransient<TransientDependency>();

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var task1 = RunScope(serviceProvider);
                Thread.Sleep(10);
                var task2 = RunScope(serviceProvider);
                var tasks = new[]{
                    task1,
                    task2
                };
                await Task.WhenAll(tasks);
                System.Console.WriteLine("Disposing serviceProvider");
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
                System.Console.WriteLine("Disposing scope");
            }
            return Task.CompletedTask;
        }
    }
}
