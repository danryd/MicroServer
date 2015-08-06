using System.Diagnostics;

namespace Tarro.Logging
{
    public class LogSinks {
        private static readonly string eventLogName = "Application";
        private static readonly string logSourceName = "Tarro";
        public static EventLog EventLog()
        {
            var eventLog = new EventLog(eventLogName);
            eventLog.Source = logSourceName;
            return eventLog;
        }

        public static TraceSource TraceSource()
        {
            return new TraceSource(logSourceName);
        }
    }
}