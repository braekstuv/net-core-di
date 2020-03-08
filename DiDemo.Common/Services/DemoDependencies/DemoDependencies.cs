using System;

namespace DiDemo.Common.Services.DemoDependencies
{
    public abstract class DependencyBase : IDisposable, IDependency
    {
        public Guid Id { get; } = Guid.NewGuid();

        public void Dispose()
        {
            Console.WriteLine($"Disposing {GetType().Name} with id: {Id}");
        }

        public string GetDescription(){
            return$"Hi! I'm a {GetType().Name}. My Id is {Id}";
        }
    }

    public class TransientDependency : DependencyBase, ITransientDependency
    {
    }

    public interface ITransientDependency : IDependency
    {
    }

    public class ScopedDependency : DependencyBase, IScopedDependency
    {
    }

    public interface IScopedDependency : IDependency
    {
    }

    public class SingletonDependency : DependencyBase, ISingletonDependency
    {
    }

    public interface ISingletonDependency: IDependency
    {
    }

    public interface IDependency
    {
        string GetDescription();
    }
}
