using System;

namespace OneSearch.Extensibility.Core.Services
{
    public static class IServiceCollectionExtensions
    {
        public static void AddSingleton<T>(this IServiceCollection collection) where T : class
            => collection.Add(typeof(T), typeof(T), ServiceLifeTime.Singleton);
        public static void AddSingleton<T1, T2>(this IServiceCollection collection) where T1 : class where T2 : class
            => collection.Add(typeof(T1), typeof(T2), ServiceLifeTime.Singleton);
        public static void AddSingleton<T>(this IServiceCollection collection, Func<IServiceProvider, T> factory) where T : class
            => collection.Add(factory, ServiceLifeTime.Singleton);


        public static void AddTransient<T>(this IServiceCollection collection) where T : class
            => collection.Add(typeof(T), typeof(T), ServiceLifeTime.Transient);
        public static void AddTransient<T1, T2>(this IServiceCollection collection) where T1 : class where T2 : class
            => collection.Add(typeof(T1), typeof(T2), ServiceLifeTime.Transient);
        public static void AddTransient<T>(this IServiceCollection collection, Func<IServiceProvider, T> factory) where T : class
            => collection.Add(factory, ServiceLifeTime.Transient);
    }
}
