using System;
using Castle.DynamicProxy;
using DiDemo.Common.Interceptors;

namespace DiDemo.InterceptionDemo.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ICalculator calculator = new Calculator();
            // calculator.Divide(5,5);
            // calculator.Divide(5,0);

            var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(calculator, new MyLoggerInterceptor(Console.Out));

            proxy.Divide(5, 5);
            proxy.Divide(5, 0);
        }


        public class Calculator : ICalculator
        {
            public double Divide(int x, int y)
            {
                return x / y;
            }
        }
        public interface ICalculator
        {
            double Divide(int x, int y);
        }
    }
}
