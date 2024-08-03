
namespace OneSearch.Extensibility.Core.Data
{
    public class DataSection<TSource, TSection> : IDataSection<TSection> where TSource : IDataSource where TSection : new()
    {
        private TSource _source;
        private TSection _section;
        public TSection Value => _section;

        public DataSection(TSource source)
        {
            _source = source;
            _section = _source.GetSection<TSection>();
        }

        public void Load()
        {
            _section = _source.GetSection<TSection>();
        }

        public void Save()
        {
            _source.SetSection(_section);
            _source.Save();
        }
    }
}
