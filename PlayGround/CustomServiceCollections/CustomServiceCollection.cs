using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace PlayGround.CustomServiceCollections
{
    public class CustomServiceCollection{
        private readonly List<Dependency> _dependencies = new List<Dependency>();
        public void AddDependency<TImplementation>(Scope scope = Scope.Scoped){
            _dependencies.Add(new Dependency{
                Scope = scope,
                Implementation = typeof(TImplementation)
            });
        }

        public ServiceProvider BuildServiceProvider(){
            var serviceCollection = new ServiceCollection();
            foreach (var dependency in _dependencies)
            {
                switch (dependency.Scope)
                {
                    case Scope.Scoped:
                        serviceCollection.AddScoped(dependency.Implementation);
                        break;
                    case Scope.Singleton:
                        serviceCollection.AddSingleton(dependency.Implementation);
                        break;
                    case Scope.Transient:
                        serviceCollection.AddTransient(dependency.Implementation);
                        break;
                    default:
                        throw new ArgumentException($"Scope {dependency.Scope} not known.");
                }
            }
            return serviceCollection.BuildServiceProvider();
        }      
    }

    public class Dependency{
        public Scope Scope {get;set;}
        public Type Implementation {get;set;}
    }


    public enum Scope{
        Singleton,
        Scoped,
        Transient
    }

}