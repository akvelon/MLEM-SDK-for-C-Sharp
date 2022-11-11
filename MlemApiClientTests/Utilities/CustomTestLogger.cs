using Microsoft.Extensions.Logging;
using MlemApi;

namespace MlemApiClientTests.Utilities
{
    internal class CustomTestLogger : ILogger<MlemApiClient>
    {
        public IList<string> Logs { get; set; }

        public CustomTestLogger()
        {
            Logs = new List<string>();
        }

        public bool IsEnabled(LogLevel LogLevel) => LogLevel == LogLevel.Debug;

        public IDisposable BeginScope<TState>(TState state) => state as IDisposable;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Logs.Add(formatter(state, exception));
        }
    }
}
