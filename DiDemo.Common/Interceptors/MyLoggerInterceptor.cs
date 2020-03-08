using System;
using System.IO;
using System.Linq;
using Castle.DynamicProxy;

namespace DiDemo.Common.Interceptors
{
    public class MyLoggerInterceptor : IInterceptor, IDisposable
    {
        private readonly TextWriter _output;
        public MyLoggerInterceptor(TextWriter output)
        {
            _output = output;
        }

        public void Dispose()
        {
            _output.WriteLine($"Disposing {GetType().Name}");
            _output.Dispose();
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                _output.WriteLine("Calling {0}.{1} with parameters {2}... ",
                    invocation.TargetType.Name,
                    invocation.Method.Name,
                    string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));
                invocation.Proceed();

                _output.WriteLine("Done: result was {0}.", invocation.ReturnValue);
            }
            catch (Exception)
            {
                _output.WriteLine("Exception thrown during call.");
                throw;
            }
        }
    }
}

