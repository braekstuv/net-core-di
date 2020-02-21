using System;
using System.IO;
using System.Linq;
using Castle.DynamicProxy;
namespace PlayGround.Interception
{
    public class InterceptionDemo
    {
        public static void Run()
        {
            ICalculator calculator = new Calculator();
            var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(calculator, new MyLogger(Console.Out));
            proxy.Divide(5, 5);
            proxy.Divide(5, 0);
        }
    }

    public class Calculator : ICalculator
    {
        public double Divide(int x, int y)
        {
            return x / y;
        }
    }

    public class MyLogger : IInterceptor
    {
        private readonly TextWriter _output;
        public MyLogger(TextWriter output)
        {
            _output = output;
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

    

    public interface ICalculator
    {
        double Divide(int x, int y);
    }
}