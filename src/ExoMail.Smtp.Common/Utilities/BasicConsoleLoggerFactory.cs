using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace ExoMail.Smtp.Common.Utilities
{
    public class BasicConsoleLoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public ILogger CreateLogger(string categoryName) => new BasicConsoleLogger(categoryName);

        public void Dispose()
        {
        }

        public static BasicConsoleLoggerFactory Create() => new BasicConsoleLoggerFactory();
    }

    public class BasicConsoleLogger : ILogger
    {
        private static readonly object _lock = new object();
        private readonly string _categoryName;

        public BasicConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //formatter = (st, ex) => WriteLine(st, ex, logLevel);
            string line = formatter.Invoke(state, exception);

            Console.WriteLine(line);
        }

        //private string WriteLine<TState>(TState state, Exception ex, LogLevel logLevel)
        //{
        //    var sb = new StringBuilder($"{_categoryName} | {logLevel} | {state}");
           
        //    if (ex != null)
        //    {
        //        sb.AppendLine(ex.Message);
        //        sb.AppendLine(ex.StackTrace);
        //    }

        //    return sb.ToString();
        //}
    }
}