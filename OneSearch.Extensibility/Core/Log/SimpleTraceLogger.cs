using System;

namespace OneSearch.Extensibility.Core.Log
{
    public class SimpleTraceLogger<T> : SimpleTraceLogger, ITraceLogger<T>
    {
        public SimpleTraceLogger() : base(typeof(T).ToString())
        {
        }
    }

    public class SimpleTraceLogger : TraceLoggerBase
    {
        public SimpleTraceLogger(string category) : base(category)
        {
        }

        protected override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
