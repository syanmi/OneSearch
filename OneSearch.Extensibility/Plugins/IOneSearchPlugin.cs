
namespace OneSearch.Extensibility.Plugins
{
    public interface IOneSearchPlugin
    {
        string Name { get; }

        void Initialize();

        void Execute();

        void Shutdown();
    }
}
