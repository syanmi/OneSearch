using OneSearch.Extensibility.Core.Services;

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
    }
}
