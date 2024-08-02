using System;
using System.Collections.Generic;

namespace OneSearch.Extensibility.Core.Services
{
    public class SimpleServiceCollection : IServiceCollection
    {
        private IList<SimpleServiceDescriptor> _descriptor;

        public SimpleServiceCollection()
        {
            _descriptor = new List<SimpleServiceDescriptor>();
        }

        public void Add(Type serviceType, Type implementationType, ServiceLifeTime lifeTime)
        {
            _descriptor.Add(new SimpleServiceDescriptor(serviceType, implementationType, lifeTime));
        }

        public void Add<T>(Func<IServiceProvider, T> factory, ServiceLifeTime lifeTime) where T : class
        {
            _descriptor.Add(new SimpleServiceDescriptor(typeof(T), factory, lifeTime));
        }

        public IServiceProvider BuildServiceProvider()
        {
            return new SimpleServiceProvider(_descriptor);
        }
    }
}
