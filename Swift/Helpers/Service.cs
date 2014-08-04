using System;
using Splat;

namespace Swift.Helpers {
    public static class Service {
        public static T Get<T>() {
            return Locator.Current.GetService<T>();
        }

        public static void Register(Func<object> factory, Type serviceType) {
            Locator.CurrentMutable.Register(factory, serviceType);
        }

        public static void RegisterLazy(Func<object> factory, Type serviceType, string contract = null) {
            Locator.CurrentMutable.RegisterLazySingleton(factory, serviceType, contract);
        }

        public static void RegisterConstant(object value, Type serviceType) {
            Locator.CurrentMutable.RegisterConstant(value, serviceType);
        }
    }
}