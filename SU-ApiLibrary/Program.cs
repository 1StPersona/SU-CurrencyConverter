using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace SU_ApiLibrary
{

    public class ApiConnected
    {
        // URL для обращения к ExchangeRatesAPI
        private const string ApiBaseUrl = "https://open.er-api.com/v6/latest";

        // Ключ API для аутентификации в ExchangeRatesAPI
        private readonly string apiKey= "8d2259df382379422a2d32c61c051a1d";

        // Объект HttpClient для отправки HTTP-запросов
        private readonly HttpClient httpClient;

        // Конструктор класса, принимающий ключ API при создании экземпляра
        public ApiConnected(string apiKey)
        {
            this.apiKey = apiKey;
            httpClient = new HttpClient();
        }

        // Метод для получения и вывода курсов валют
        public async Task PrintCurrencyList()
        {
            try
            {
                // Отправляем GET-запрос к ExchangeRatesAPI с ключом API
                var response = await httpClient.GetStringAsync($"{ApiBaseUrl}?access_key={apiKey}");

                // Десериализуем ответ в объект ExchangeRatesResponse
                var rates = JsonConvert.DeserializeObject<ExchangeRatesResponse>(response);

                // Проверяем, успешно ли получены курсы
                if (rates != null && rates.Rates != null)
                {
                    Console.WriteLine("Курсы обмена:");

                    // Выводим курсы обмена в консоль
                    foreach (var rate in rates.Rates)
                    {
                        Console.WriteLine($"{rate.Key}: {rate.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("Невозможно получить курсы обмена.");
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок при запросе к API
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        // Метод для конвертации суммы в другую валюту
        public decimal ConvertCurrency(decimal amount, decimal exchangeRate)
        {
            return amount * exchangeRate;
        }
    }

    // Класс, представляющий структуру ответа от ExchangeRatesAPI
    public class ExchangeRatesResponse
    {
        public Dictionary<string, decimal> Rates { get; set; }
    }
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var apiConnected = new ApiConnected("");
            await apiConnected.PrintCurrencyList();
        }
    }

}
