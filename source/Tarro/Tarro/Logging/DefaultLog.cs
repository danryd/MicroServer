using System;
using System.Diagnostics;
using System.Text;

namespace Tarro.Logging
{
    internal class DefaultLog : ILog
    {
        private readonly TraceSource traceSource;
        private readonly EventLog eventLog;
        private readonly string loggerName;
        private static readonly string logSourceName = "Tarro";

        public DefaultLog(string name)
        {
            loggerName = name;
            traceSource = new TraceSource(logSourceName);
            traceSource.Switch.Level = SourceLevels.All;
            Trace.AutoFlush = true;

            eventLog = new EventLog(logSourceName);


            if (Environment.UserInteractive)
                traceSource.Listeners.Add(new ColoredConsoleTraceListener());
            else
                traceSource.Listeners.Add(new EventLogTraceListener(logSourceName));

        }

        private string ExceptionToString(Exception exception)
        {
            var sb = new StringBuilder();
            do
            {
                sb.AppendLine(exception.Message);
                sb.AppendLine("at:");
                sb.AppendLine(exception.StackTrace);
                exception = exception.InnerException;
            } while (exception != null);
            return sb.ToString();
        }

        private void Log(TraceEventType type, string format, Exception exception, params object[] parameters)
        {
            var sb = new StringBuilder();
            sb.Append("[" + loggerName + "]");
            sb.Append(" ");
            sb.AppendLine(String.Format(format, parameters));
            if (exception != null)
            {
                sb.AppendLine("Exception:");
                sb.AppendLine(ExceptionToString(exception));
            }
            traceSource.TraceEvent(type, 0, sb.ToString());

        }
        private void Log(TraceEventType type, string format, params object[] parameters)
        {
            Log(type, format, null, parameters);
        }
        public void Warn(string message, params object[] parameters)
        {

            Log(TraceEventType.Warning, message, parameters);
        }

        public void Warn(string message, Exception exception, params object[] parameters)
        {

            Log(TraceEventType.Warning, message, exception, parameters);
        }
        public void Error(string message, params object[] parameters)
        {
            Log(TraceEventType.Error, message, parameters);
        }

        public void Error(string message, Exception exception, params object[] parameters)
        {
            Log(TraceEventType.Error, message, exception, parameters);
        }
        public void Verbose(string message, params object[] parameters)
        {
            Log(TraceEventType.Verbose, message, parameters);
        }

        public void Verbose(string message, Exception exception, params object[] parameters)
        {
            Log(TraceEventType.Verbose, message, exception, parameters);
        }
        public void Info(string message, params object[] parameters)
        {
            Log(TraceEventType.Information, message, parameters);
        }

        public void Info(string message, Exception exception, params object[] parameters)
        {
            Log(TraceEventType.Warning, message, exception, parameters);
        }

    }
}