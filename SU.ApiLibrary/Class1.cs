using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SU.ExceptionLog;
namespace SU.ApiLibrary
{
    /// <summary>
    /// Класс для работы с API курсов валют.
    /// </summary>
    public class ApiConnected
    {
        private const string ApiBaseUrl = "https://open.er-api.com/v6/latest";
        private static string apiKey;
        private static HttpClient httpClient;

        /// <summary>
        /// Статический конструктор для установки значения API ключа и инициализации HttpClient.
        /// </summary>
        static ApiConnected()
        {
            apiKey = "8d2259df382379422a2d32c61c051a1d";
            httpClient = new HttpClient();
        }

        /// <summary>
        /// Свойство для получения или установки API ключа.
        /// </summary>
        public static string ApiKey
        {
            get { return apiKey; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("API ключ не может быть пустым или содержать только пробелы.");
                }
                else
                {
                    apiKey = value;
                    httpClient = new HttpClient();
                }
            }
        }

        /// <summary>
        /// Метод для получения базового URL API.
        /// </summary>
        /// <returns>Базовый URL API.</returns>
        public static string GetApiBaseUrl()
        {
            return ApiBaseUrl;
        }

        /// <summary>
        /// Метод для получения экземпляра HttpClient.
        /// </summary>
        /// <returns>Экземпляр HttpClient.</returns>
        public static HttpClient GetHttpClient()
        {
            return httpClient;
        }

        /// <summary>
        /// Метод для асинхронного получения курсов валют с API.
        /// </summary>
        /// <returns>Словарь с курсами валют.</returns>
        public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync()
        {
            try
            {
                var response = await httpClient.GetStringAsync($"{ApiBaseUrl}?access_key={apiKey}");
                var rates = JsonConvert.DeserializeObject<ExchangeRatesResponse>(response);
                return rates?.Rates;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении курсов валют: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для асинхронного вывода списка курсов валют в консоль.
        /// </summary>
        public async Task PrintCurrencyList()
        {
            try
            {
                var response = await httpClient.GetStringAsync($"{ApiBaseUrl}?access_key={apiKey}");
                var rates = JsonConvert.DeserializeObject<ExchangeRatesResponse>(response);

                if (rates != null && rates.Rates != null)
                {
                    Console.WriteLine("Курсы обмена:");

                    foreach (var rate in rates.Rates)
                    {
                        Console.WriteLine($"{rate.Key}: {rate.Value}");
                    }
                }
                else
                {
                    throw new Exception("Невозможно получить курсы обмена.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для конвертации валюты.
        /// </summary>
        /// <param name="amount">Сумма для конвертации.</param>
        /// <param name="exchangeRate">Курс обмена валюты.</param>
        /// <returns>Сумма после конвертации.</returns>
        public decimal ConvertCurrency(decimal amount, decimal exchangeRate)
        {
            return amount * exchangeRate;
        }

        /// <summary>
        /// Внутренний класс для десериализации ответа от API с курсами валют.
        /// </summary>
        public class ExchangeRatesResponse
        {
            public Dictionary<string, decimal> Rates { get; set; }
        }
    }
}
