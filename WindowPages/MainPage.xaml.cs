using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TestUtilitiesCalculation.WindowPages
{
    /// <summary>
    /// Главная страница приложения. Здесь отображается список всех пользователей, а также происходит переход к другим функциональным блокам
    /// </summary>
    public partial class MainPage : Page
    {
        MainWindow parent;      // ссылка на главное окно, являющееся родителем. В основном нужно для переключения страниц
        DataTable table = new DataTable();      // в данной таблице будут отображены записи о пользователях
        public MainPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;

            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Address", typeof(string));
            table.Columns.Add("Number of residents", typeof(int));
        }

        public void getUsersData()     // поднимаем из базы данных данные все записи пользователей
        {
            foreach (UsersData.User user in parent.usersSet)
                refreshDataTable(user);
        }

        // Чтобы не производить запрос ко всем записям для обновления отображения таблицы, можно вызывать данную функцию, добавляющую новый элемент в конец списка
        public void refreshDataTable(UsersData.User user)
        {
            table.Rows.Add(user.Id, user.residentialAddress, user.residentsCount);
            usersTable.ItemsSource = table.AsDataView();
        }

        // Событие, срабатывающее при выборе элемента (пользователя) в таблице
        private void usersTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (usersTable.SelectedItem is null) return;     // если выбрана не пустая строка...
                var ID = (int)(usersTable.SelectedItem as DataRowView).Row[0];      // получаем ID выбранного пользователя
                parent.Content = new ReadingsPage(parent, parent.usersSet.getByID((int)ID));        // создаем новую форму и передаем туда вызванного пользователя
                usersTable.UnselectAll();       // Убираем выделение
            }
            catch
            {
                MessageBox.Show("Была вызвана недействительная строка");
                return;
            }
        }

        // Обработчики событий и прочие неинтересные вещи
        // --------------------------------------------------------------------------------------
        private void viewAccounts_Click(object sender, RoutedEventArgs e)
        {
            parent.Content = new WindowPages.TransactionsPage(parent);

        }
        private void createUser_Click(object sender, RoutedEventArgs e)
        {
            parent.Content = new UserCreationPage(parent);
        }

        private void viewTariffs_Click(object sender, RoutedEventArgs e)
        {
            parent.Content = new WindowPages.ServicesPage(parent, parent.servicesSet);
        }
    }
}
