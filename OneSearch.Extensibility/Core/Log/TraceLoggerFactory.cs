
namespace OneSearch.Extensibility.Core.Log
{
    public class TraceLoggerFactory : ITraceLoggerFactory
    {
        public ITraceLogger CreateLogger(string categoryName)
        {
            return new SimpleTraceLogger(categoryName);
        }
    }
}
