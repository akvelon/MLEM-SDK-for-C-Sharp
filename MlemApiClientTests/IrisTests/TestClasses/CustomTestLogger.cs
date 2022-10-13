using Microsoft.Extensions.Logging;
using MlemApi;

namespace MlemApiClientTests.IrisTests.TestClasses
{
    internal class CustomTestLogger : ILogger<MlemApiClient>
    {
        public IList<string> Logs { get; set; }

        public CustomTestLogger()
        {
            this.Logs = new List<string>();
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
            Logs.Add(formatter(state, exception));
        }
    }
}
