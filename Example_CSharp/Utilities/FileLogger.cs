using System.Text;
using Microsoft.Extensions.Logging;
using MlemApi;

namespace Example.Utilities
{
    internal class FileLogger : ILogger<MlemApiClient>
    {
        private string targetFilePath;
        public FileLogger(string targetFilePath)
        {
            this.targetFilePath = targetFilePath;
        }
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
            var logMessageBuilder = new StringBuilder();
            var currentDateTimeUtcString = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            logMessageBuilder.Append($"[{currentDateTimeUtcString}] [{logLevel}] {formatter(state, exception)}");
            File.AppendAllLines(targetFilePath, new List<string>() { logMessageBuilder.ToString() });
        }
    }
}
