using System.IO;
using System.Linq;
using Castle.DynamicProxy;
using PlayGround.CustomServiceCollections;
using PlayGround.Interception;

namespace PlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            //ScopesDemo.Run();
            //CustomServiceCollectionDemo.Run();
            InterceptionDemo.Run();
        }
    }
}
