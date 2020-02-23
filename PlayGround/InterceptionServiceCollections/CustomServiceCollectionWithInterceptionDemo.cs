using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using PlayGround.Interception;
using PlayGround.Scopes;

namespace PlayGround.InterceptionServiceCollections
{
    public class CustomServiceCollectionWithInterceptionDemo
    {
        private static IServiceProvider _serviceProvider;
        public static void Run()
        {
            var serviceCollection = new CustomServiceCollection();
            serviceCollection.AddInterceptor(() => new MyLogger(Console.Out));
            serviceCollection.AddDependency<ISingletonService,SingletonService>(Scope.Singleton)
                .WithInterceptor<MyLogger>();
            serviceCollection.AddDependency<IScopedService,ScopedService>(Scope.Scoped)
                .WithInterceptor<MyLogger>();
            serviceCollection.AddDependency<ITransientService,TransientService>(Scope.Transient)
                .WithInterceptor<MyLogger>();
          
            
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
                var services = new List<IService>(){
                    scope.ServiceProvider.GetService<ISingletonService>(),
                    scope.ServiceProvider.GetService<ISingletonService>(),
                    scope.ServiceProvider.GetService<ITransientService>(),
                    scope.ServiceProvider.GetService<ITransientService>(),
                    scope.ServiceProvider.GetService<IScopedService>(),
                    scope.ServiceProvider.GetService<IScopedService>(),
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