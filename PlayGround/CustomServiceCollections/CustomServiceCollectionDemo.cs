using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using PlayGround.Scopes;

namespace PlayGround.CustomServiceCollections
{
    public class CustomServiceCollectionDemo
    {
        private static IServiceProvider _serviceProvider;
        public static void Run()
        {
            var serviceCollection = new CustomServiceCollection();
            serviceCollection.AddDependency<SingletonService>(Scope.Singleton);
            serviceCollection.AddDependency<ScopedService>(Scope.Scoped);
            serviceCollection.AddDependency<TransientService>(Scope.Transient);
          
            
            _serviceProvider = serviceCollection.BuildServiceProvider();

            RunScope(1);
            RunScope(2);

        }

        private static void RunScope(int scopeId)
        {
            Console.WriteLine();
            Console.WriteLine("===========================");
            Console.WriteLine($"Opening Scope {scopeId}...");
            Console.WriteLine("===========================");
            using(var scope = _serviceProvider.CreateScope()){
                var services = new List<ServiceBase>(){
                    scope.ServiceProvider.GetService<SingletonService>(),
                    scope.ServiceProvider.GetService<SingletonService>(),
                    scope.ServiceProvider.GetService<TransientService>(),
                    scope.ServiceProvider.GetService<TransientService>(),
                    scope.ServiceProvider.GetService<ScopedService>(),
                    scope.ServiceProvider.GetService<ScopedService>(),
                };
                
                services.ForEach(s => {
                    Console.WriteLine($"Hello, {s.GetType().Name}! What is your Id? (press any key)");
                    //Console.Read();
                    s.LogDescription();
                    Console.WriteLine();
                });
                Console.WriteLine($"Closing Scope {scopeId}...(press any key)");
                //Console.Read();
            }

        }
    }
}