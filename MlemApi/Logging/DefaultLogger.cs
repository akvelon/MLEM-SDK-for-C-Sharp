using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MlemApi.Logging
{
    public class DefaultLogger : ILogger<MlemApiClient>
    {
        /// <summary>
        /// Log levels for which timestamps should be displayed
        /// </summary>
        public List<LogLevel> TimestampsLogLevels { get; set; } = new List<LogLevel>() { LogLevel.Warning, LogLevel.Error, LogLevel.Information, LogLevel.Debug, LogLevel.Trace };
        /// <summary>
        /// Log levels for which class names should be displayed
        /// </summary>
        public List<LogLevel> ClassNameLogLevels { get; set; } = new List<LogLevel>() { LogLevel.Debug, LogLevel.Trace };
        /// <summary>
        /// Log levels for which method names should be displayed
        /// </summary>
        public List<LogLevel> MethodNameLogLevels { get; set; } = new List<LogLevel>() { LogLevel.Debug, LogLevel.Trace };

        /// <summary>
        /// Methods to skip in stack trace - to get relevant class/method name of the caller
        /// Denends on calls nesting inside of DefaultLogger class
        /// </summary>
        private readonly int methodsToSkip = 4;

        public bool IsEnabled(LogLevel LogLevel)
        {
            return LogLevel == LogLevel.Debug;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return state as IDisposable;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Console.WriteLine(FormatLogMessage(logLevel, eventId, state, exception, formatter));
        }

        public string FormatLogMessage<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var logMessageBuilder = new StringBuilder();

            if (TimestampsLogLevels.Contains(logLevel))
            {
                logMessageBuilder.Append(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }

            logMessageBuilder.Append($" [{logLevel}]");
            if (ClassNameLogLevels.Contains(logLevel))
            {
                StackTrace stackTrace = new StackTrace();
                
                var className = stackTrace.GetFrame(methodsToSkip).GetMethod().ReflectedType.Name;
                var frames = stackTrace.GetFrames();
                logMessageBuilder.Append($" {className}");
            }

            if (MethodNameLogLevels.Contains(logLevel))
            {
                StackTrace stackTrace = new StackTrace();
                var methodName = stackTrace.GetFrame(methodsToSkip).GetMethod().Name;
                logMessageBuilder.Append($".{methodName}");
            }

            logMessageBuilder.Append($" {formatter(state, exception)}");

            return logMessageBuilder.ToString();
        }
    }
}
