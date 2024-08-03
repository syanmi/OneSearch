
namespace OneSearch.Extensibility.Core.Data
{
    interface IDataSourceFactory
    {
        IDataSource Create(string name);
    }
}
