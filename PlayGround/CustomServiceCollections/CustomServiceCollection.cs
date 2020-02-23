using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace PlayGround.CustomServiceCollections
{
    public class CustomServiceCollection{
        private readonly List<Dependency> _dependencies = new List<Dependency>();
        public void AddDependency<TImplementation>(Scope scope = Scope.Scoped)
            where TImplementation: class
        {
            _dependencies.Add(new Dependency<TImplementation>(scope));
        }

        public ServiceProvider BuildServiceProvider(){
            var serviceCollection = new ServiceCollection();
            foreach (var dependency in _dependencies)
            {
                var factory = dependency.CreateFunc();
                factory.Invoke(serviceCollection);
            }
            return serviceCollection.BuildServiceProvider();
        }      
    }

    public class Dependency<TImplementation> : Dependency
    where TImplementation: class
    {
        public Dependency(Scope scope) : base(scope)
        {
        }

        public override Func<ServiceCollection, ServiceCollection> CreateFunc(){
            return new Func<ServiceCollection, ServiceCollection>(
                sc => {
                    switch(Scope){
                        case Scope.Scoped:
                            sc.AddScoped<TImplementation>();
                            break;
                        case Scope.Singleton:
                            sc.AddSingleton<TImplementation>();
                            break;
                        case Scope.Transient:
                            sc.AddTransient<TImplementation>();
                            break;
                        default:
                            throw new ArgumentException($"Scope {Scope} not known.");
                    }
                    return sc;
                }
            );
        }
    }

    public abstract class Dependency{
        public Scope Scope {get;}
        public Dependency(Scope scope)
        {
            Scope = scope;
        }
        public abstract Func<ServiceCollection, ServiceCollection> CreateFunc();
    }


    public enum Scope{
        Singleton,
        Scoped,
        Transient
    }

}