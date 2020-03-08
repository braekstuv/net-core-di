using System.Linq;
using DiDemo.Common.Services.DemoDependencies;

namespace DiDemo.Common.Services
{
    public class MyCoolService
    {
        private readonly MyOtherCoolService _myOtherCoolService;
        private readonly ISingletonDependency _singleton;
        private readonly ITransientDependency _transient;
        private readonly IScopedDependency _scoped;

        public MyCoolService(
            MyOtherCoolService myOtherCoolService,
            ISingletonDependency singleton,
            ITransientDependency transient,
            IScopedDependency scoped)
        {
            _myOtherCoolService = myOtherCoolService;
            _singleton = singleton;
            _transient = transient;
            _scoped = scoped;
        }

        public string[] GetDescriptions(){
            return new string[]{
                $"{GetType().Name}{GetHashCode()}",
                _singleton.GetDescription(),
                _scoped.GetDescription(),
                _transient.GetDescription(),
            }.Concat(_myOtherCoolService.GetDescriptions()).ToArray();
        }
    }

    public class MyOtherCoolService
    {
        private readonly ISingletonDependency _singleton;
        private readonly ITransientDependency _transient;
        private readonly IScopedDependency _scoped;

        public MyOtherCoolService(
            ISingletonDependency singleton,
            ITransientDependency transient,
            IScopedDependency scoped)
        {
            _singleton = singleton;
            _transient = transient;
            _scoped = scoped;
        }

        public string[] GetDescriptions()
        {
            return new string[]{
                $"{GetType().Name}{GetHashCode()}",
                _singleton.GetDescription(),
                _scoped.GetDescription(),
                _transient.GetDescription(),
            };
        }
    }
}