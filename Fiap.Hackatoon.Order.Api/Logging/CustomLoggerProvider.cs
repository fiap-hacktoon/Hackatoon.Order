using System.Collections.Concurrent;

namespace Fiap.Hackatoon.Order.Api.Logging
{
    public class CustomLoggerProvider(CustomLoggerProviderConfiguration _loggerConfig) : ILoggerProvider
    {

        private readonly CustomLoggerProviderConfiguration loggerConfig = _loggerConfig;
        private readonly ConcurrentDictionary<string, CustomLogger> loggers = new ConcurrentDictionary<string, CustomLogger>();

        public ILogger CreateLogger(string categoryName)
            => loggers.GetOrAdd(categoryName, name => new CustomLogger(name, loggerConfig));

        public void Dispose()
           => throw new NotImplementedException();
    }
}
