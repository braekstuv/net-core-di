using System.Linq;
using DiDemo.Common.Services.DemoDependencies;

namespace DiDemo.Common.Services {
    public class MyCoolService {
        private readonly MyOtherCoolService _myOtherCoolService;
        private readonly SingletonDependency _singleton;
        private readonly TransientDependency _transient;
        private readonly ScopedDependency _scoped;

        public MyCoolService(
            MyOtherCoolService myOtherCoolService,
            SingletonDependency singleton,
            TransientDependency transient,
            ScopedDependency scoped) {
            _myOtherCoolService = myOtherCoolService;
            _singleton = singleton;
            _transient = transient;
            _scoped = scoped;
        }

        public string[] GetDescriptions() {
            return new string[] {
                $"{GetType().Name}{GetHashCode()}",
                _singleton.GetDescription(),
                _scoped.GetDescription(),
                _transient.GetDescription(),
            }.Concat(_myOtherCoolService.GetDescriptions()).ToArray();
        }
    }

    public class MyOtherCoolService {
        private readonly SingletonDependency _singleton;
        private readonly TransientDependency _transient;
        private readonly ScopedDependency _scoped;

        public MyOtherCoolService(
            SingletonDependency singleton,
            TransientDependency transient,
            ScopedDependency scoped) {
            _singleton = singleton;
            _transient = transient;
            _scoped = scoped;
        }

        public string[] GetDescriptions() {
            return new string[] {
                $"{GetType().Name}{GetHashCode()}",
                _singleton.GetDescription(),
                _scoped.GetDescription(),
                _transient.GetDescription(),
            };
        }
    }
}