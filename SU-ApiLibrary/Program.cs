using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SU.ApiLibrary;
using SU.ExceptionLog;

namespace SU_LogicLevel
{
    public class CurrencyConverter
    {
        private readonly ApiConnected apiConnected;

        public CurrencyConverter(ApiConnected apiConnected)
        {
            this.apiConnected = apiConnected;
        }

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

    internal class Program
    {
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
