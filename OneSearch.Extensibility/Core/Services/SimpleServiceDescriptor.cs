using System;

namespace OneSearch.Extensibility.Core.Services
{
    public class SimpleServiceDescriptor
    {
        private Type _type;
        private Func<IServiceProvider, object> _factory;
        private ServiceLifeTime _lifeTime;

        public Type Type => _type;
        public Func<IServiceProvider, object> Factory => _factory;
        public ServiceLifeTime LifeTime => _lifeTime;

        public SimpleServiceDescriptor(Type type, Func<IServiceProvider, object> factory, ServiceLifeTime lifeTime)
        {
            _type = type;
            _factory = factory;
            _lifeTime = lifeTime;
        }
    }
}
