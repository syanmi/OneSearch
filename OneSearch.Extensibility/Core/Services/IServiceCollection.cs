using System;

namespace OneSearch.Extensibility.Core.Services
{
    public interface IServiceCollection
    {
        void Add(Type serviceType, Type implementationType, ServiceLifeTime lifeTime);

        void Add<T>(Func<IServiceProvider, T> factory, ServiceLifeTime lifeTime) where T : class;

        IServiceProvider BuildServiceProvider();
    }
}
