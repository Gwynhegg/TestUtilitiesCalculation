using Microsoft.Data.Sqlite;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TestUtilitiesCalculation.WindowPages
{
    /// <summary>
    /// Данная страница проста и нужна только для вывода таблицы, содержащей все произведенные в системе транзакции
    /// </summary>
    public partial class TransactionsPage : Page
    {
        MainWindow parent;
        public TransactionsPage(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;

            createDataTable();
        }

        private void createDataTable()       // метод для создания и отображения таблицы с транзакциями
        {
            var selectQuery = "SELECT * FROM accounts;";
            var reader = Auxiliary.DatabaseConnector.getInstance().ExecuteReaderCommand(selectQuery);

            DataTable table = new DataTable();
            table.Columns.Add("Account ID", typeof(string));
            table.Columns.Add("User ID", typeof(string));
            table.Columns.Add("Address", typeof(string));
            table.Columns.Add("Date of Readings", typeof(string));
            table.Columns.Add("Cold Water Account", typeof(string));
            table.Columns.Add("Hot Water Delivery Account", typeof(string));
            table.Columns.Add("Hot Water Heating Account", typeof(string));
            table.Columns.Add("Electricity Day Account", typeof(string));
            table.Columns.Add("Electricity Night Account", typeof(string));
            table.Columns.Add("Total", typeof(string));

            while (reader.Read())
            {
                DataRow row = table.NewRow();
                for (int i = 0; i < row.ItemArray.Length; i++)
                    if (reader.IsDBNull(i)) row[i] = '-'; else row[i] = reader.GetString(i);
                table.Rows.Add(row);
            }

            transactionsTable.ItemsSource = table.AsDataView();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            parent.refreshMainPage();

        }
    }
}
