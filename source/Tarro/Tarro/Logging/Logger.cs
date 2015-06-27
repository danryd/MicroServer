using System;

namespace Tarro.Logging
{

    public class LogFactory
    {

        public static ILog GetLogger(string name)
        {
            return new DefaultLog(name);
        }

        public static ILog GetLogger(Type type)
        {
            return GetLogger(type.Name);
        }
        public static ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }
    }


    public interface ILog
    {
        void Warn(string message, params object[] parameters);
        void Warn(string message, Exception exception, params object[] parameters);
        void Error(string message, params object[] parameters);
        void Error(string message, Exception exception, params object[] parameters);
        void Verbose(string message, params object[] parameters);
        void Verbose(string message, Exception exception, params object[] parameters);
        void Info(string message, params object[] parameters);
        void Info(string message, Exception exception, params object[] parameters);
    }


}

