
namespace OneSearch.Extensibility.Core.Data
{
    public interface IDataSource
    {
        T GetSection<T>() where T : new();
    }
}
