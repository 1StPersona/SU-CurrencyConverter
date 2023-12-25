using System;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole; // Добавьте эту директиву
using Serilog.Sinks.File;

namespace SU.ExceptionLog
{
    /// <summary>
    /// Статический класс для логирования исключений.
    /// </summary>
    public static class ExceptionLogger
    {
        static ExceptionLogger()
        {
            // Указываем путь к файлу лога
            string logFilePath = $@"C:\Users\danil\OneDrive\Документы\ComputerScience\SU-CurrencyConverter\Logs\log-{Guid.NewGuid().ToString()}log.txt";

            // Конфигурация Serilog для записи в текстовый файл
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Добавляем обработчик события завершения приложения для закрытия логгера
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
        }

        /// <summary>
        /// Записывает ошибку в лог.
        /// </summary>

        public static void LogError(string errorMessage)
        {
            Log.Error(errorMessage);
        }
    }
}
