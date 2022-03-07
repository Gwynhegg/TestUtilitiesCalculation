using System;
using System.Windows;

namespace TestUtilitiesCalculation.WindowPages
{
    /// <summary>
    /// На данной странице отображаются высчитанные итоги стоимости услуг с указанием конфигурации
    /// </summary>
    public partial class ResultWindow : Window
    {
        public ResultWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Метод для отображения результатов подсчета стоимости оказания услуг
        /// </summary>
        /// <param name="account">Конкретный экземпляр счета, прошедший вычисления</param>
        /// <param name="services">Список услуг</param>
        public void getResultsToDisplay(Accounts.Account account, Services.ServicesSet services)
        {
            if (account.coldWaterAccount is Accounts.NormativeBill)
                coldWaterResult.Text = String.Format("{0:0.##} (норматив)", account.coldWaterAccount.totalCost);
            else
                coldWaterResult.Text = String.Format("{0:0.##} (счётчик)", account.coldWaterAccount.totalCost);

            if (account.hotWaterAccount is Accounts.NormativeBill)
                hotWaterResult.Text = String.Format("{0:0.##} (норматив)", account.hotWaterAccount.totalCost);
            else
                hotWaterResult.Text = String.Format("{0:0.##} (подача) + {1:0.###} (нагрев)", account.hotWaterAccount.totalCost, account.hotWaterHeatingAccount.totalCost);

            if (account.electricityAccount is Accounts.NormativeBill)
                ElectricityResult.Text = String.Format("{0:0.##} (норматив)", account.electricityAccount.totalCost);
            else
                ElectricityResult.Text = String.Format("{0:0.##} (день) + {1:0.##} (ночь)", account.electricityAccount.totalCost, account.electricityNightAccount.totalCost);

            totalResult.Text = String.Format("{0:0.##} рублей", account.totalResult);

        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
