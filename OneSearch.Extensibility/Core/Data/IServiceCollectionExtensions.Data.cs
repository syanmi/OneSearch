using OneSearch.Extensibility.Core.Services;
using System;

namespace OneSearch.Extensibility.Core.Data
{
    public static class IServiceCollectionExtensionsData
    {
        public static void MapDataSrouce<TSource, TSection>(this IServiceCollection services) where TSource : IReadOnlyDataSource where TSection : class, new()
        {
            services.Add((provider) =>
            {
                var source = provider.GetService<TSource>();
                return source.GetSection<TSection>();
            }, ServiceLifeTime.Singleton);
        }

        public static void AddDataSection<TSource, TSection>(this IServiceCollection services) where TSource : IDataSource where TSection : new()
        {

            Type Tip = typeof(IDataSection<TSection>);

            Func<OneSearch.Extensibility.Core.Services.IServiceProvider, IDataSection<TSection>> factory = (provider) =>
            {
                var source = provider.GetService<TSource>();
                return new DataSection<TSource, TSection>(source);
            };

            services.AddSingleton<IDataSection<TSection>>(factory);
  
        }
    }
}
