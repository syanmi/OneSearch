
namespace OneSearch.Extensibility.Core.Log
{
    public interface ITraceLogger
    {
        void Log(TraceLogLevel level, string message);
    }
    public interface ITraceLogger<out T> : ITraceLogger
    {
    }
}
