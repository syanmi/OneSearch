
namespace OneSearch.Extensibility.Core.Log
{
    public abstract class TraceLoggerBase : ITraceLogger
    {
        private string _category;
        private TraceLogLevel _level;

        public TraceLoggerBase()
        {
        }

        public TraceLoggerBase(string category)
        {
            _category = category;
            _level = TraceLogLevel.Debug;
        }

        public void Log(TraceLogLevel level, string message)
        {
            if(level < _level)
            {
                return;
            }

            WriteLine($"{_category} : {message}");
        }

        protected abstract void WriteLine(string message);
    }
}
