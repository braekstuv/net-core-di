using System;
using System.Threading;
using System.Threading.Tasks;
using DiDemo.Common.Services;
using DiDemo.Common.Services.DemoDependencies;
using Microsoft.Extensions.DependencyInjection;

namespace DiDemo.CaptiveDependenciesDemo.ConsoleApp {
    class Program {
        static async Task Main(string[] args) {
            var serviceCollection = new ServiceCollection();

            //Underlying dependencies will be disposed when the scope is disposed.
            serviceCollection.AddScoped<MyCoolService>()
                .AddScoped<MyOtherCoolService>();

            //Underlying dependencies will be disposed when the serviceprovider is disposed.
            // serviceCollection.AddSingleton<MyCoolService>()
            // .AddSingleton<MyOtherCoolService>();

            serviceCollection.AddSingleton<SingletonDependency>()
                .AddScoped<ScopedDependency>()
                .AddTransient<TransientDependency>();

            //Line below will not throw an error
            using(var serviceProvider = serviceCollection.BuildServiceProvider())
            //Line below will throw an error if there are captive dependencies
            // using (var serviceProvider = serviceCollection.BuildServiceProvider(true))
            //Line below will throw an error
            // using (var serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions{
            //     ValidateScopes = true, //throws error if captive dependencies detected
            //     ValidateOnBuild = true //throws error if there are missing dependencies (open generics excluded)
            // }))
            {
                var task1 = RunScope(serviceProvider);
                Thread.Sleep(10);
                var task2 = RunScope(serviceProvider);
                var tasks = new [] {
                    task1,
                    task2
                };
                await Task.WhenAll(tasks);
                System.Console.WriteLine("Disposing serviceProvider");
            };
        }

        private static Task RunScope(ServiceProvider serviceProvider) {
            using(var scope = serviceProvider.CreateScope()) {
                var myCoolService = scope.ServiceProvider.GetService<MyCoolService>();
                Console.WriteLine(string.Join('\n', myCoolService.GetDescriptions()));
                System.Console.WriteLine("Disposing scope");
            }
            return Task.CompletedTask;
        }
    }
}