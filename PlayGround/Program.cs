using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace PlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            //ScopesDemo.Run();

            
        }
    }

    public class AOPServiceCollection : ServiceCollection{

        private List<object> _interceptors = new List<object>();
        public ServiceCollection AddInterceptor<T>() where T: class{
            this.AddSingleton<T>();            
            return this;
        }

        override
    }

    public class Logger<T>{

    }
}
