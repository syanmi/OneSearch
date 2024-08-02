using System;

namespace OneSearch.Extensibility.Core.Services
{
    public interface IServiceCollection
    {
        void Add<T>(Func<IServiceProvider, T> factory, ServiceLifeTime lifeTime) where T : class;

        IServiceProvider BuildServiceProvider();
    }
}
