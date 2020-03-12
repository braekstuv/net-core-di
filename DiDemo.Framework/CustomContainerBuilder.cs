using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DiDemo.Framework
{
    public class CustomContainerBuilder
    {
        private readonly List<Dependency> _dependencies = new List<Dependency>();

        private readonly IServiceCollection _serviceCollection = new ServiceCollection();

        public CustomContainerBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public Dependency AddDependency<TService, TImplementation>(Scope scope = Scope.Scoped)
            where TService : class
            where TImplementation : class, TService
        {
            var dependency = new Dependency<TService, TImplementation>(scope);
            _dependencies.Add(dependency);
            return dependency;
        }

        public ServiceProvider BuildServiceProvider()
        {
            foreach (var dependency in _dependencies)
            {
                var createFunc = dependency.CreateFunc();
                createFunc.Invoke(_serviceCollection);
            }

            return _serviceCollection.BuildServiceProvider();
        }

    }

    public class Dependency<TService, TImplementation> : Dependency
    where TService : class
    where TImplementation : class, TService
    {
        public Dependency(Scope scope) : base(scope)
        {
        }

        public override Func<IServiceCollection, IServiceCollection> CreateFunc()
        {
            return InnerCreateFunc();
        }

        public Func<IServiceCollection, IServiceCollection> InnerCreateFunc()
        {
            return new Func<IServiceCollection, IServiceCollection>(
                sc =>
                {
                    switch (Scope)
                    {
                        case Scope.Scoped:
                            sc.AddScoped<TService, TImplementation>();
                            break;
                        case Scope.Singleton:
                            sc.AddSingleton<TService, TImplementation>();
                            break;
                        case Scope.Transient:
                            sc.AddTransient<TService, TImplementation>();
                            break;
                        default:
                            throw new ArgumentException($"Scope {Scope} not known.");
                    }
                    return sc;
                }
            );
        }

    }
    public abstract class Dependency
    {
        public Dependency(Scope scope)
        {
            this.Scope = scope;

        }
        public Scope Scope { get; }
        public abstract Func<IServiceCollection, IServiceCollection> CreateFunc();
    }

    public enum Scope
    {
        Singleton,
        Scoped,
        Transient
    }

}