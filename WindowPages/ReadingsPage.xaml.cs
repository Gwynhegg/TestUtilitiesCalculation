using System;
using System.Windows;
using System.Windows.Controls;

namespace TestUtilitiesCalculation.WindowPages
{
    /// <summary>
    /// На данной странице происходит ввод показаний, расчет суммы к оплате. Имеется возможность смены сценария наличия или отсутствия учетного прибора
    /// </summary>
    public partial class ReadingsPage : Page
    {
        private MainWindow parent;      // ссылка на окно-родитель, для навигации в будущем

        // два пользователя. Для того, чтобы случайно не изменить данные о пользователе, в большинстве операций будет применяться "фиктивный" пользователь,
        // созданный прототипированием
        private UsersData.User user, tempUser;

        private Accounts.Account account = new Accounts.Account();      // создание нового экземпляра класса-счета
        public ReadingsPage(MainWindow parent, UsersData.User user)
        {
            InitializeComponent();

            this.parent = parent;
            this.user = user;

            setDefaultData();       // устанавливаем начальные значения полей
        }

        // метод для расчета величины оплаты каждой из услуг
        private void calculateCost_Click(object sender, RoutedEventArgs e)
        {
            // создаем новый экземпляр пользователя путем клонирования
            tempUser = user.Clone((bool)hasColdMeterBox.IsChecked ? true : false, (bool)hasHotMeterBox.IsChecked ? true : false, (bool)hasElectricityMeterBox.IsChecked ? true : false);
            account.syncWithUser(tempUser);     // синхронизируем данные счета с пользователем(наличие или отсутствие ПУ)
            account.syncWithServices(parent.servicesSet);       // синхронизируем данные счета со списком услуг (для получения нормативов)
            var readings = new double?[5];
            try
            {
                readings = getReadings();       // пытаемся получить значения текстовых полей для ввода показаний
            }
            catch
            {
                MessageBox.Show("Введены некорректные значения. Проверьте поля ввода на правильность");
                return;
            }

            try
            {
                account.calculateResult(tempUser, parent.servicesSet, readings);        // выситываем результат на основе данных пользователя, услуг и показаний
            }
            catch
            {
                MessageBox.Show("Введены некорректные показания счетчиков - показания не могут быть меньше накопленного объема");
                return;
            }

            var resultWindow = new ResultWindow();      // создаем и вызываем результирующее окно
            resultWindow.getResultsToDisplay(account, parent.servicesSet);      // отображаем результаты
            resultWindow.ShowDialog();

            goForward.IsEnabled = true;     // Делаем возможность перехода к следующему месяцу доступным. При изменении любого из полей кнопка вновь заблокируется
        }

        private double?[] getReadings()     // метод для получения показаний из полей TextBox
        {
            double?[] readings = new double?[5];
            try
            {
                readings[0] = getValueFromElement(coldWaterReadings);
                readings[1] = getValueFromElement(hotWaterReadingsDeploy);
                readings[2] = getValueFromElement(hotWaterReadingsHeating);
                readings[3] = getValueFromElement(ElectricityReadingsDay);
                readings[4] = getValueFromElement(ElectricityReadingsNight);
            }
            catch
            {
                throw new Exception();
                return null;
            }

            return readings;
        }

        private double? getValueFromElement(TextBox textBox)
        {
            try
            {
                if (textBox.IsEnabled == false) return null;
                double? value = Double.Parse(textBox.Text);
                return value;
            }
            catch
            {
                if (textBox.IsEnabled == true) throw new Exception();
                return null;
            }
        }
        private void setDefaultData()       // Метод для установки стартовых значений при вызове новой страницы
        {

            // отображаем информацию о пользователе
            currentMonthText.Text = "Текущий месяц: " + account.getDateOfLastAccount(user);
            userInfoText.Text = "ID : " + user.Id + " , Адрес : " + user.residentialAddress + " , количество жильцов : " + user.residentsCount;

            // отображаем информацию о наличии ПУ и блокируем соответствующие поля
            if (user.hasHotWaterMeter)
                hasHotMeterBox.IsChecked = true;
            else
            {
                hasHotMeterBox.IsChecked = false;
                hotWaterReadingsDeploy.IsEnabled = false;
                hotWaterReadingsHeating.IsEnabled = false;
            }

            hotWaterAccumulatedDeploy.Text = String.Format("{0:0.##}", user.hotMeter.accumulatedVolume);
            hotWaterAccumulatedHeating.Text = String.Format("{0:0.##}", user.hotMeter.accumulatedHeatingVolume);

            if (user.hasColdWaterMeter)
                hasColdMeterBox.IsChecked = true;
            else
            {
                hasColdMeterBox.IsChecked = false;
                coldWaterReadings.IsEnabled = false;
            }

            coldWaterAccumulated.Text = String.Format("{0:0.##}", user.coldMeter.accumulatedVolume);

            if (user.hasElectricityMeter)
                hasElectricityMeterBox.IsChecked = true;
            else
            {
                hasElectricityMeterBox.IsChecked = false;
                ElectricityReadingsDay.IsEnabled = false;
                ElectricityReadingsNight.IsEnabled = false;
            }

            ElectricityAccumulatedDay.Text = String.Format("{0:0.##}", user.electricityMeter.accumulatedVolume);
            ElectricityAccumulatedNight.Text = String.Format("{0:0.##}", user.electricityMeter.accumulatedNightVolume);
        }


        private void goForward_Click(object sender, RoutedEventArgs e)      // метод, подтверждающий введенные данные, после чего произойдет переход на новую страницу
        {
            user.refreshUser(tempUser);      // обновляем данные о текущем пользователе с занесением в БД
            account.saveResult();      // сохраняем результаты транзакции в отдельную таблицу
            parent.Content = new ReadingsPage(parent, user);        // переходим к следующему месяцу
        }

        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            parent.refreshMainPage();
        }

        // Обработчики событий и прочие неинтересные вещи
        //----------------------------------------------------------------------------------------------
        private void hasColdMeterBox_Checked(object sender, RoutedEventArgs e)
        {
            coldWaterReadings.IsEnabled = true;
            goForward.IsEnabled = false;
        }

        private void hasColdMeterBox_Unchecked(object sender, RoutedEventArgs e)
        {
            coldWaterReadings.IsEnabled = false;
            goForward.IsEnabled = false;
        }

        private void hasHotMeterBox_Checked(object sender, RoutedEventArgs e)
        {
            hotWaterReadingsDeploy.IsEnabled = true;
            goForward.IsEnabled = false;
        }

        private void hasHotMeterBox_Unchecked(object sender, RoutedEventArgs e)
        {
            hotWaterReadingsDeploy.IsEnabled = false;
            hotWaterReadingsHeating.IsEnabled = false;
            goForward.IsEnabled = false;
        }

        private void hasElectricityMeterBox_Checked(object sender, RoutedEventArgs e)
        {
            ElectricityReadingsDay.IsEnabled = true;
            ElectricityReadingsNight.IsEnabled = true;
            goForward.IsEnabled = false;

        }

        private void hasElectricityMeterBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ElectricityReadingsDay.IsEnabled = false;
            ElectricityReadingsNight.IsEnabled = false;
            goForward.IsEnabled = false;

        }

        private void Readings_TextChanged(object sender, TextChangedEventArgs e)
        {
            goForward.IsEnabled = false;
        }
    }
}
