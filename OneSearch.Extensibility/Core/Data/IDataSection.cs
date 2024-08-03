
namespace OneSearch.Extensibility.Core.Data
{
    public interface IDataSection<T>
    {
        T Value { get; }

        void Load();

        void Save();
    }
}
