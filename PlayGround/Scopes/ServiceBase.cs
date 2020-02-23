using System;

namespace PlayGround.Scopes
{
    public abstract class ServiceBase : IDisposable, IService
    {
        public Guid Id { get; } = Guid.NewGuid();

        public void Dispose()
        {
            Console.WriteLine($"Disposing {GetType().Name} with id: {Id}");
        }

        public virtual void LogDescription(){
            Console.WriteLine($"My Id is {Id}");
        }
    }

    public class TransientService : ServiceBase, ITransientService
    {
    }

    public interface ITransientService : IService
    {
    }

    public class ScopedService : ServiceBase, IScopedService
    {
    }

    public interface IScopedService : IService
    {
    }

    public class SingletonService : ServiceBase, ISingletonService
    {
    }

    public interface ISingletonService: IService
    {
    }

    public interface IService
    {
        void LogDescription();
    }
}