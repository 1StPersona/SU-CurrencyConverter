using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SU.ApiLibrary;
using SU.ExceptionLog;

namespace SU_LogicLevel
{
    /// <summary>
    /// Класс для конвертации валюты.
    /// </summary>
    public class CurrencyConverter
    {
        private readonly ApiConnected apiConnected;

        /// <summary>
        /// Инициализирует новый экземпляр класса CurrencyConverter.
        /// </summary>
        /// <param name="apiConnected">Подключение к API для получения курсов валют.</param>
        public CurrencyConverter(ApiConnected apiConnected)
        {
            this.apiConnected = apiConnected;
        }

        /// <summary>
        /// Метод для конвертации валюты.
        /// </summary>
        public async Task ConvertCurrency()
        {
            try
            {
                var exchangeRates = await apiConnected.GetExchangeRatesAsync();

                Console.WriteLine("Выберите исходную валюту из списка:");
                await PrintCurrencyList(exchangeRates);

                Console.Write("Введите исходную валюту: ");
                string sourceCurrency = Console.ReadLine().ToUpper();

                Console.WriteLine("Выберите целевую валюту из списка:");

                Console.Write("Введите целевую валюту: ");
                string targetCurrency = Console.ReadLine().ToUpper();

                if (!exchangeRates.ContainsKey(sourceCurrency) || !exchangeRates.ContainsKey(targetCurrency))
                {
                    ExceptionLogger.LogError("Выбранная валюта не найдена в списке курсов обмена.");
                    throw new Exception("Выбранная валюта не найдена в списке курсов обмена.");
                }

                Console.Write("Введите сумму для конвертации: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                {
                    ExceptionLogger.LogError("Ошибка ввода суммы.");
                    throw new Exception("Ошибка ввода суммы.");
                }

                if (sourceCurrency == targetCurrency)
                {
                    ExceptionLogger.LogError("Вы выбрали одинаковую валюту. Зачем?");
                    throw new Exception("Вы выбрали одинаковую валюту. Зачем?");
                }

                decimal sourceRate = exchangeRates[sourceCurrency];
                decimal targetRate = exchangeRates[targetCurrency];

                decimal convertedAmount = (amount / sourceRate) * targetRate;

                Console.WriteLine($"{amount} {sourceCurrency} = {convertedAmount} {targetCurrency}");
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogError($"Ошибка при конвертации валют: {ex}");
            }
        }

        /// <summary>
        /// Выводит список доступных валют.
        /// </summary>
        /// <param name="exchangeRates">Словарь с курсами валют.</param>
        public async Task PrintCurrencyList(Dictionary<string, decimal> exchangeRates)
        {
            Console.WriteLine("Доступные валюты:");
            foreach (var rate in exchangeRates)
            {
                Console.Write($"{rate.Key} ");
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Класс с точкой входа в программу.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        static async Task Main(string[] args)
        {
            var apiConnected = new ApiConnected();
            var currencyConverter = new CurrencyConverter(apiConnected);

            bool exitRequested = false;

            while (!exitRequested)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Просмотреть курсы валют");
                Console.WriteLine("2. Конвертировать валюту");
                Console.WriteLine("0. Выйти");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        // Просмотр курсов валют
                        await apiConnected.PrintCurrencyList();
                        break;

                    case "2":
                        // Конвертация валюты
                        await currencyConverter.ConvertCurrency();
                        break;

                    case "0":
                        // Выход из программы
                        exitRequested = true;
                        break;

                    default:
                        Console.WriteLine("Неверный выбор. Пожалуйста, введите корректное значение.");
                        break;
                }

                Console.WriteLine(); // Добавляем пустую строку для разделения
            }
        }
    }
}
