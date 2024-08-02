using System;

namespace OneSearch.Extensibility.Core.Services
{
    static class IServiceCollectionExtensions
    {
        public static void AddSingleton<T>(this IServiceCollection collection) where T : class
            => collection.Add<T>(null, ServiceLifeTime.Singleton);

        public static void AddSingleton<T>(this IServiceCollection collection, Func<IServiceProvider, T> factory) where T : class
            => collection.Add(factory, ServiceLifeTime.Singleton);

        public static void AddTransient<T>(this IServiceCollection collection) where T : class
            => collection.Add<T>(null, ServiceLifeTime.Transient);

        public static void AddTransient<T>(this IServiceCollection collection, Func<IServiceProvider, T> factory) where T : class
            => collection.Add(factory, ServiceLifeTime.Transient);
    }
}
