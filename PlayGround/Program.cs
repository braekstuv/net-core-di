using System.IO;
using System.Linq;
using Castle.DynamicProxy;
using PlayGround.CustomServiceCollections;
namespace PlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            //ScopesDemo.Run();
            CustomServiceCollectionDemo.Run();
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
            _output.Write("Calling method {0} with parameters {1}... ",
            invocation.Method.Name,
            string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));

            invocation.Proceed();

            _output.WriteLine("Done: result was {0}.", invocation.ReturnValue);
        }
    }
}
