using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TestUtilitiesCalculation.WindowPages
{
    /// <summary>
    /// Страница нужна для вывода таблицы с перечнем всех услуг
    /// </summary>
    public partial class ServicesPage : Page
    {
        MainWindow parent;
        public ServicesPage(MainWindow parent, Services.ServicesSet services)
        {
            InitializeComponent();
            this.parent = parent;

            createDataTable(services);
        }

        private void createDataTable(Services.ServicesSet services)     // метод для создания и отображения таблицы с услугами
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Name of Service", typeof(string));
            dataTable.Columns.Add("Tariff", typeof(double));
            dataTable.Columns.Add("Norm", typeof(string));
            dataTable.Columns.Add("Measure", typeof(string));

            foreach (Services.Service service in services)
                dataTable.Rows.Add(service.ID, service.nameOfService, service.tariffValue, service.norm, service.measure);

            servicesTable.ItemsSource = dataTable.AsDataView();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            parent.refreshMainPage();
        }
    }
}
