using System;
using System.Windows;
using System.Windows.Controls;

namespace TestUtilitiesCalculation.WindowPages
{
    /// <summary>
    /// На данной странице происходит задание начальных характеристик пользователя и его последующее добавление в таблицу 
    /// </summary>
    public partial class UserCreationPage : Page
    {
        MainWindow parent;
        public UserCreationPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void goAndSave_Click(object sender, RoutedEventArgs e)
        {
            if (AllFieldsAlright()) createNewUser();        // если поля заполнены корректно, создаем нового пользователя
            parent.usersSet.Refresh(parent.connector);     // обновляем коллекцию UsersSet, добавляя туда нового пользователя
            parent.mainPage.refreshDataTable(parent.usersSet.getLast());        // обновляем таблицу для отображения пользователей
            parent.refreshMainPage();
        }

        private void createNewUser()        // метод для создания нового юзера и добавление его в таблицу
        {
            string addQuery = string.Format("INSERT INTO users " +
                "(residentialAddress, numberOfResidents, hasColdWaterMeter, hasHotWaterMeter, hasEnergyMeter, coldWaterVolume, hotWaterSupplyVolume, hotWaterHeatingVolume, dayEnergyVolume, nightEnergyVolume) " +
                "VALUES ('{0}', {1}, {2}, {3}, {4}, 0, 0, 0, 0, 0);",
                addressTextBox.Text, Math.Round(residentsCount.Value).ToString(), hasColdWaterMeter.IsChecked.Value ? 1 : 0, hasHotWaterMeter.IsChecked.Value ? 1 : 0, hasElectricityMeter.IsChecked.Value ? 1 : 0);
            parent.connector.ExecuteNonQuaryCommand(addQuery);
        }
        private bool AllFieldsAlright()     // проверка корректности полей (под это попадает только адрес, на самом деле)
        {
            if (addressTextBox.Text == "")
            {
                MessageBox.Show("Введите ваш действительный адрес");
                return false;
            }
            return true;
        }

        private void residentsCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            residentsCountText.Text = "Количество проживающих человек: " + Math.Round(residentsCount.Value).ToString();
        }

        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            parent.refreshMainPage();
        }
    }
}
