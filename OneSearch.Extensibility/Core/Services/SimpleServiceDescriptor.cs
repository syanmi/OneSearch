using System;

namespace OneSearch.Extensibility.Core.Services
{
    public class SimpleServiceDescriptor
    {
        private Type _serviceType;
        private Type _implementationType;
        private Func<IServiceProvider, object> _factory;
        private ServiceLifeTime _lifeTime;

        public Type ServiceType => _serviceType;
        public Type ImplementationType => _implementationType;
        public Func<IServiceProvider, object> Factory => _factory;
        public ServiceLifeTime LifeTime => _lifeTime;

        public SimpleServiceDescriptor(Type serviceType, object service)
        {
            _serviceType = serviceType;
            _implementationType = serviceType;
            _factory = ((provider) => service);
            _lifeTime = ServiceLifeTime.Singleton;
        }

        public SimpleServiceDescriptor(Type serviceType, Type implementationType, ServiceLifeTime lifeTime)
        {
            _serviceType = serviceType;
            _implementationType = implementationType;
            _factory = null;
            _lifeTime = lifeTime;
        }

        public SimpleServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifeTime lifeTime)
        {
            _serviceType = serviceType;
            _implementationType = serviceType;
            _factory = factory;
            _lifeTime = lifeTime;
        }
    }
}
