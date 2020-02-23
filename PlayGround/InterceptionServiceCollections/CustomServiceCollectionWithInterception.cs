using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace PlayGround.InterceptionServiceCollections
{
    public class CustomServiceCollection
    {
        private readonly List<Dependency> _dependencies = new List<Dependency>();
        private readonly Dictionary<Type, Action<ServiceCollection>> _interceptorFuncs = new Dictionary<Type, Action<ServiceCollection>>();
        public CustomServiceCollection AddInterceptor<TInterceptor>(Func<TInterceptor> interceptorFunc)
        where TInterceptor : class, IInterceptor
        {
            var interceptor = interceptorFunc.Invoke();
            _interceptorFuncs.Add(typeof(TInterceptor), sp =>
                sp.AddSingleton<TInterceptor>(
                    new Func<IServiceProvider, TInterceptor>((sp) => interceptor))
                );
            return this;
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
            var innerServiceCollection = new ServiceCollection();
            AddInterceptors(innerServiceCollection);
            foreach (var dependency in _dependencies)
            {
                var createFunc = dependency.CreateFunc();
                createFunc.Invoke(innerServiceCollection);
            }

            return innerServiceCollection.BuildServiceProvider();
        }

        private void AddInterceptors(ServiceCollection serviceCollection)
        {
            foreach (var interceptorFunc in _interceptorFuncs)
            {
                interceptorFunc.Value.Invoke(serviceCollection);
            }
        }
    }

    public class Dependency<TService, TImplementation> : Dependency
    where TService : class
    where TImplementation : class, TService
    {
        private readonly HashSet<Type> _interceptorTypes = new HashSet<Type>();
        public IEnumerable<Type> InterceptorTypes => _interceptorTypes;
        public Dependency(Scope scope) : base(scope)
        {
        }

        public override Func<ServiceCollection, ServiceCollection> CreateFunc()
        {
            if (_interceptorTypes.Any())
            {
                return InnerCreateInterceptedFunc();
            }

            return InnerCreateFunc();

        }

        public Func<ServiceCollection, ServiceCollection> InnerCreateInterceptedFunc()
        {
            var func = new Func<IServiceProvider, TService>(
                sp =>
                {
                    var implementation = sp.GetService<TImplementation>();
                    var interceptors = _interceptorTypes.Select(it => sp.GetService(it) as IInterceptor).ToArray();
                    var intercepted = new ProxyGenerator().CreateInterfaceProxyWithTarget(typeof(TService), implementation, interceptors);
                    return intercepted as TService;
                }
            );
            return new Func<ServiceCollection, ServiceCollection>(
                sc =>
                {
                    switch (Scope)
                    {
                        case Scope.Scoped:
                            sc.AddScoped<TImplementation>();
                            sc.AddScoped<TService>(func);
                            break;
                        case Scope.Singleton:
                            sc.AddSingleton<TImplementation>();
                            sc.AddSingleton<TService>(func);
                            break;
                        case Scope.Transient:
                            sc.AddTransient<TImplementation>();
                            sc.AddTransient<TService>(func);
                            break;
                        default:
                            throw new ArgumentException($"Scope {Scope} not known.");
                    }
                    return sc;
                }
            );
        }
        public Func<ServiceCollection, ServiceCollection> InnerCreateFunc()
        {
            return new Func<ServiceCollection, ServiceCollection>(
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

        public override Dependency WithInterceptor<TInterceptor>()
        {
            _interceptorTypes.Add(typeof(TInterceptor));
            return this;
        }
    }
    public abstract class Dependency
    {
        public Dependency(Scope scope)
        {
            this.Scope = scope;

        }
        public Scope Scope { get; }
        public abstract Func<ServiceCollection, ServiceCollection> CreateFunc();
        public abstract Dependency WithInterceptor<TInterceptor>() where TInterceptor : class, IInterceptor;
    }

    public enum Scope
    {
        Singleton,
        Scoped,
        Transient
    }

}