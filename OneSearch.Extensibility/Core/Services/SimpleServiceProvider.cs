using System;
using System.Collections.Generic;
using System.Linq;

namespace OneSearch.Extensibility.Core.Services
{
    public class SimpleServiceProvider : IServiceProvider
    {
        private IList<SimpleServiceDescriptor> _descriptor;
        private Dictionary<Type, object> _instance;

        public SimpleServiceProvider(IList<SimpleServiceDescriptor> descriptor)
        {
            _descriptor = descriptor;
            _instance = new Dictionary<Type, object>();
        }

        public TService GetService<TService>() => (TService)GetService(typeof(TService));

        public object GetService(Type type)
        {
            var descriptor = _descriptor.FirstOrDefault(x => x.ServiceType == type);

            if (descriptor == null && type.IsGenericType)
            {
                var genType = type.GetGenericTypeDefinition();
                descriptor = _descriptor.FirstOrDefault(x => x.ServiceType == genType);

                var genericArg = type.GetGenericArguments()[0];
                var newType = descriptor.ImplementationType.MakeGenericType(genericArg);

                return GetSingletonService(new SimpleServiceDescriptor(descriptor.ServiceType, newType, ServiceLifeTime.Singleton));
            }


            switch (descriptor.LifeTime)
            {
                case ServiceLifeTime.Singleton: return GetSingletonService(descriptor);
                case ServiceLifeTime.Transient: return CreateInstance(descriptor);
                default: return null;
            }
        }

        private object GetSingletonService(SimpleServiceDescriptor descriptor)
        {
            if (_instance.TryGetValue(descriptor.ServiceType, out var instance))
            {
                return instance;
            }

            var second = CreateInstance(descriptor);
            _instance.Add(descriptor.ServiceType, second);

            return second;
        }

        private object CreateInstance(SimpleServiceDescriptor descriptor)
        {
            if(descriptor.Factory != null)
            {
                return descriptor.Factory(this);
            }

            return ConstructService(descriptor.ImplementationType);
        }

        private object ConstructService(Type type)
        {
            var constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
            var parameters = constructor.GetParameters();
            var arguments = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                arguments[i] = GetService(parameters[i].ParameterType);
            }

            return constructor.Invoke(arguments);
        }
    }
}
