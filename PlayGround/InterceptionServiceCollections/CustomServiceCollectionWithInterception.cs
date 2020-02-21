using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace PlayGround.InterceptionServiceCollections
{
    public class InterceptionServiceCollection
    {
        private readonly Dictionary<Type, Action<ServiceCollection>> _interceptors = new Dictionary<Type, Action<ServiceCollection>>();
        private readonly List<DependencyBuilder> _dependencies = new List<DependencyBuilder>();
        public DependencyBuilder AddDependency<TImplementation>(Scope scope = Scope.Scoped)
        {
            var dependencyBuilder = new DependencyBuilder
            {
                Dependency = new Dependency
                {
                    Scope = scope,
                    Implementation = typeof(TImplementation)
                }
            };
            _dependencies.Add(dependencyBuilder);
            return dependencyBuilder;
        }

        public void AddInterceptor<TInterceptor>(Func<TInterceptor> interceptorFunc) where TInterceptor: class, IInterceptor, new()
        {
            var interceptor = interceptorFunc.Invoke();
            _interceptors.Add(interceptor.GetType(), 
            new Action<ServiceCollection>((s) => 
                s.AddSingleton<TInterceptor>(
                    new Func<IServiceProvider, TInterceptor>((sp) => interceptor))
                ));
        }

        public ServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            foreach (var interceptor in _interceptors)
            {
                interceptor.Value.Invoke(serviceCollection);
            }

            foreach (var dependencyBuilder in _dependencies)
            {
                var dependency = dependencyBuilder.Dependency;
                switch (dependency.Scope)
                {
                    case Scope.Scoped:
                        
                        // serviceCollection.AddScoped(dependency.Implementation);
                        break;
                    case Scope.Singleton:
                        // serviceCollection.AddSingleton(dependency.Implementation);
                        break;
                    case Scope.Transient:
                        // serviceCollection.AddTransient(dependency.Implementation);
                        break;
                    default:
                        throw new ArgumentException($"Scope {dependency.Scope} not known.");
                }
            }
            return serviceCollection.BuildServiceProvider();
        }
    }

    public class DependencyBuilder
    {
        private readonly List<Type> _interceptors = new List<Type>();
        public IEnumerable<Type> Interceptors => _interceptors;
        public Dependency Dependency { get; set; }
       
        
        public DependencyBuilder WithInterceptor<TInterceptor>() where TInterceptor : class, IInterceptor{
            _interceptors.Add(typeof(TInterceptor));
            return this;
        }
    }

    public class Dependency
    {
        public Scope Scope { get; set; }
        public Type Implementation { get; set; }

    }


    public enum Scope
    {
        Singleton,
        Scoped,
        Transient
    }

}