
namespace OneSearch.Extensibility.Core.Log
{
    public interface ITraceLoggerFactory
    {
        ITraceLogger CreateLogger(string categoryName);
    }
}
