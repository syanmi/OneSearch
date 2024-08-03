
namespace OneSearch.Extensibility.Core.Data
{
    public interface IReadOnlyDataSource
    {
        T GetSection<T>() where T : new();
    }
}
