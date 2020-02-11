using System;

namespace PlayGround.Scopes
{
    public abstract class ServiceBase : IDisposable
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

    public class TransientService : ServiceBase
    {
    }

    public class ScopedService : ServiceBase
    {
    }

    public class SingletonService : ServiceBase
    {
    }
}