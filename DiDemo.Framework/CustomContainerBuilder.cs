using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace DiDemo.Framework {
    public class CustomContainerBuilder {
        private readonly List<DependencyRegistration> _dependencies = new List<DependencyRegistration>();
        private readonly List<InterceptorRegistration> _interceptors = new List<InterceptorRegistration>();
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();

        public CustomContainerBuilder(IServiceCollection serviceCollection) {
            _serviceCollection = serviceCollection;
        }

        public CustomContainerBuilder AddInterceptor<TInterceptor>(Func<IServiceProvider, TInterceptor> interceptorFunc)
        where TInterceptor : class, IInterceptor {
            _interceptors.Add(
                new InterceptorRegistration<TInterceptor>(
                    interceptorFunc
                )
            );
            return this;
        }

        public DependencyRegistration AddDependency<TService, TImplementation>(Scope scope = Scope.Scoped)
        where TService : class
        where TImplementation : class, TService {
            var dependency = new DependencyRegistration<TService, TImplementation>(scope);
            _dependencies.Add(dependency);
            return dependency;
        }

        public ServiceProvider BuildServiceProvider() {
            AddInterceptors(_serviceCollection);
            foreach (var dependency in _dependencies) {
                var wiring = dependency.CreateWiring();
                wiring.Invoke(_serviceCollection);
            }

            return _serviceCollection.BuildServiceProvider();
        }

        private void AddInterceptors(IServiceCollection serviceCollection) {
            foreach (var interceptor in _interceptors) {
                var wiring = interceptor.CreateWiring();
                wiring.Invoke(_serviceCollection);
            }
        }
    }

    public class DependencyRegistration<TService, TImplementation> : DependencyRegistration
    where TService : class
    where TImplementation : class, TService {
        private readonly HashSet<Type> _interceptorTypes = new HashSet<Type>();
        public IEnumerable<Type> InterceptorTypes => _interceptorTypes;
        public DependencyRegistration(Scope scope) : base(scope) { }

        public override Action<IServiceCollection> CreateWiring() {
            if (_interceptorTypes.Any()) {
                return InnerCreateInterceptedWiring();
            }

            return InnerCreateWiring();

        }

        public Action<IServiceCollection> InnerCreateInterceptedWiring() {
            var func = new Func<IServiceProvider, TService>(
                    sp => {
                        var implementation = sp.GetService<TImplementation>();
                        var interceptors = _interceptorTypes.Select(it => sp.GetService(it)as IInterceptor).ToArray();
                        var intercepted = new ProxyGenerator().CreateInterfaceProxyWithTarget(typeof(TService), implementation, interceptors);
                        return intercepted as TService;
                    }
                );
            return new Action<IServiceCollection>(
                sc => {
                    switch (Scope) {
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
                }
            );
        }
        public Action<IServiceCollection> InnerCreateWiring() {
            return new Action<IServiceCollection>(
                sc => {
                    switch (Scope) {
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
                }
            );
        }

        public override DependencyRegistration WithInterceptor<TInterceptor>() {
            _interceptorTypes.Add(typeof(TInterceptor));
            return this;
        }
    }
    public abstract class DependencyRegistration : ICreateWiring {
        public DependencyRegistration(Scope scope) {
            this.Scope = scope;

        }
        public Scope Scope { get; }
        public abstract Action<IServiceCollection> CreateWiring();
        public abstract DependencyRegistration WithInterceptor<TInterceptor>()where TInterceptor : class, IInterceptor;
    }

    public class InterceptorRegistration<TInterceptor> : InterceptorRegistration
    where TInterceptor : class, IInterceptor {
        private readonly Func<IServiceProvider, TInterceptor> _createInterceptorFunc;

        public InterceptorRegistration(Func<IServiceProvider, TInterceptor> createInterceptorFunc) {
            _createInterceptorFunc = createInterceptorFunc;
        }

        public override Action<IServiceCollection> CreateWiring() {
            return new Action<IServiceCollection>(sc => sc.AddSingleton<TInterceptor>(_createInterceptorFunc));
        }
    }
    public abstract class InterceptorRegistration : ICreateWiring {
        public abstract Action<IServiceCollection> CreateWiring();
    }

    public interface ICreateWiring {
        Action<IServiceCollection> CreateWiring();
    }

    public enum Scope {
        Singleton,
        Scoped,
        Transient
    }

}