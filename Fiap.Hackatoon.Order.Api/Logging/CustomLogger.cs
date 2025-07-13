namespace Fiap.Hackatoon.Order.Api.Logging
{
    public class CustomLogger(string loggerName, CustomLoggerProviderConfiguration loggerConfig) : ILogger
    {
        private readonly string loggerName = loggerName;
        private readonly CustomLoggerProviderConfiguration loggerConfig = loggerConfig;
        public static bool Arquivo { get; set; } = false;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string message = $"Log de Execução: {logLevel}: {eventId.Id} - {formatter(state, exception)}";

            if (Arquivo)
                ExcreverTextoNoArquivo(message);
            else
                Console.WriteLine(message);
        }

        private void ExcreverTextoNoArquivo(string message)
        {
            string caminhoArquivo = Environment.CurrentDirectory + @$"\log-{DateTime.Now:yyyy-MM-dd}.txt";
            if (!File.Exists(caminhoArquivo))
            {
                using StreamWriter sw = File.CreateText(caminhoArquivo);
                sw.WriteLine(message);
            }
            else
            {
                using StreamWriter sw = File.AppendText(caminhoArquivo);
                sw.WriteLine(message);
            }
        }
    }
}
