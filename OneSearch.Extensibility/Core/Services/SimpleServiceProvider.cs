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
            Type serviceType;
            Type implementType;
            Func<IServiceProvider, object> factory;
            ServiceLifeTime lifeTime;
            if (type.IsGenericType)
            {
                var genType = type.GetGenericTypeDefinition();
                var descriptor = _descriptor.FirstOrDefault(x => x.ServiceType == genType);

                var genericArg = type.GetGenericArguments()[0];
                var newType = descriptor.ImplementationType.MakeGenericType(genericArg);

                serviceType = type;
                implementType = descriptor.ImplementationType.MakeGenericType(genericArg);
                factory = descriptor.Factory;
                lifeTime = descriptor.LifeTime;
            }
            else
            {
                var descriptor = _descriptor.FirstOrDefault(x => x.ServiceType == type);
                serviceType = descriptor.ServiceType;
                implementType = descriptor.ImplementationType;
                factory = descriptor.Factory;
                lifeTime = descriptor.LifeTime;
            }


            switch (lifeTime)
            {
                case ServiceLifeTime.Singleton: return GetSingletonService(serviceType, implementType, factory);
                case ServiceLifeTime.Transient: return CreateInstance(implementType, factory);
                default: return null;
            }
        }

        private object GetSingletonService(Type serviceType, Type implementType, Func<IServiceProvider, object> factory)
        {
            if (_instance.TryGetValue(serviceType, out var instance))
            {
                return instance;
            }

            var second = CreateInstance(implementType, factory);
            _instance.Add(serviceType, second);

            return second;
        }

        private object CreateInstance(Type implementType, Func<IServiceProvider, object> factory)
        {
            if(factory != null)
            {
                return factory(this);
            }

            return ConstructService(implementType);
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
