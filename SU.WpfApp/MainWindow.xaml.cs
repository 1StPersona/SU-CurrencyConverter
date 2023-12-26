
using SU.ApiLibrary;
using SU.ExceptionLog;
using System;
using System.Windows;

namespace SU.WpfApp
{
    public partial class MainWindow : Window
    {
        private readonly ApiConnected apiConnected;

        public MainWindow()
        {
            InitializeComponent();
            apiConnected = new ApiConnected();
            LoadCurrencies(); // Загружаем курсы валют при открытии MainWindow
        }

        private async void LoadCurrencies()
        {
            try
            {
                var exchangeRates = await apiConnected.GetExchangeRatesAsync();

                foreach (var currency in exchangeRates.Keys)
                {
                    cmbSourceCurrency.Items.Add(currency);
                    cmbTargetCurrency.Items.Add(currency);
                }

                cmbSourceCurrency.SelectedIndex = 0;
                cmbTargetCurrency.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogError($"Ошибка при загрузке валют: {ex}");
                MessageBox.Show($"Ошибка при загрузке валют: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ButtonConvert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sourceCurrency = cmbSourceCurrency.SelectedItem?.ToString();
                string targetCurrency = cmbTargetCurrency.SelectedItem?.ToString();

                if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
                {
                    ExceptionLogger.LogError("Ошибка ввода суммы.");
                    MessageBox.Show("Ошибка ввода суммы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (sourceCurrency == targetCurrency)
                {
                    ExceptionLogger.LogError("Вы выбрали одинаковую валюту. Зачем?");
                    MessageBox.Show("Вы выбрали одинаковую валюту. Зачем?", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var exchangeRates = await apiConnected.GetExchangeRatesAsync();
                decimal sourceRate = exchangeRates[sourceCurrency];
                decimal targetRate = exchangeRates[targetCurrency];

                decimal convertedAmount = (amount / sourceRate) * targetRate;

                txtResult.Text = $"{amount} {sourceCurrency} = {convertedAmount} {targetCurrency}";
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogError($"Ошибка при конвертации валют: {ex}");
                MessageBox.Show($"Ошибка при конвертации валют: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
