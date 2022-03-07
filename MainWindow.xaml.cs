using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Windows;

namespace TestUtilitiesCalculation
{
    /// <summary>
    /// Поскольку это десктоп приложение, вся логическая основа будет храниться в данной форме (за неимением класса Main)
    /// </summary>
    public partial class MainWindow : Window
    {
        public WindowPages.MainPage mainPage { get; }       // главная страница, выступающая отправным пунктом для навигации по страницам

        public UsersData.UsersSet usersSet { get; private set; }     // Экземпляр класса для хранения данных пользователей
        public Services.ServicesSet servicesSet { get; private set; }        // Экземпляр класса для хранения данных об услугах
        public Auxiliary.DatabaseConnector connector { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            setDatabaseConnection();

            servicesSet = new Services.ServicesSet(connector);     // устанавливаем соединение с БД и выгружаем список услуг
            usersSet = new UsersData.UsersSet(connector);      // устанавливаем соединение с БД и выгружаем данные пользователей

            mainPage = new WindowPages.MainPage(this);      // создаем первоначальную страницу
            mainPage.getUsersData();

            this.Content = mainPage;
        }

        public void refreshMainPage()       // метод для обновления экрана и возвращения на главную страницу
        {
            this.Content = mainPage;
        }

        private void setDatabaseConnection()        // устанавливаем соединение с базой данных SQLite
        {
            var connectionString = "Data Source=testDatabase.db;Mode=ReadWriteCreate;";
            this.connector = new Auxiliary.DatabaseConnector(connectionString);

            createServicesTable();       // проверяем/создаем таблицу с тарифами
            createUsersTable();     // проверяем/создаем таблицу с юзерами
            createAccountsTable();      // проверяем/создаем таблицу с начислениями
        }

        private void createServicesTable()      // создаем и заполняем таблицу services
        {

            // Проверяем, есть ли таблица tariffs в базе данных
            string checkingQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='services';";
            var reader = connector.ExecuteScalarCommand(checkingQuery);

            // Если таблицы нет, вызываем метод для первичного заполнения таблицы services
            if (reader is not null) return;

            string creationQuery = "CREATE TABLE services (" +
                "serviceID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "nameOfService VARCHAR(20)," +
                "tariff REAL NOT NULL," +
                "norm REAL," +
                "measure VARCHAR(5)," +
                "UNIQUE (nameOfService));";
            connector.ExecuteNonQuaryCommand(creationQuery);

            // Считываем данные о тарифах из CSV-файла
            using (StreamReader streamReader = new StreamReader(@"..\..\..\Auxiliary/TarrifsText.csv"))
            {
                string? line;
                string inputQuery;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] lineElements = line.Split(',');
                    // помещаем данные в таблицу
                    inputQuery = String.Format("INSERT INTO services (nameOfService, tariff, norm, measure)" +
                        " VALUES ('{0}',{1},{2},'{3}');", lineElements);
                    connector.ExecuteNonQuaryCommand(inputQuery);
                }
            }
        }

        private void createUsersTable()
        {
            // Если таблицы users нет, то данный запрос ее создаст. Иначе она останется неизменна
            string creationQuery = "CREATE TABLE IF NOT EXISTS users (" +
            "userID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
            "residentialAddress VARCHAR(100) NOT NULL," +
            "numberOfResidents INT NOT NULL," +
            "hasColdWaterMeter BOOL NOT NULL," +
            "hasHotWaterMeter BOOL NOT NULL," +
            "hasEnergyMeter BOOL NOT NULL," +
            "coldWaterVolume REAL NOT NULL," +
            "hotWaterSupplyVolume REAL NOT NULL," +
            "hotWaterHeatingVolume REAL NOT NULL," +
            "dayEnergyVolume REAL NOT NULL," +
            "nightEnergyVolume REAL NOT NULL," +
            "UNIQUE (residentialAddress));";
            connector.ExecuteNonQuaryCommand(creationQuery);
        }

        private void createAccountsTable()
        {
            // Если таблицы accounts нет, то данный запрос ее создаст. Иначе она останется неизменна
            string creationQuery = "CREATE TABLE IF NOT EXISTS accounts (" +
            "accountID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
            "userID INTEGER NOT NULL," +
            "residentialAddress VARCHAR(100) NOT NULL," +
            "dateOfReadings INT NOT NULL," +
            "coldWaterAccount REAL NOT NULL," +
            "hotWaterAccount REAL NOT NULL," +
            "hotWaterHeatingAccount REAL," +
            "ElectricityAccount REAL NOT NULL," +
            "ElectricityNightAccount REAL," +
            "totalResult REAL NOT NULL," +
            "accountRepaid BOOL NOT NULL," +
            "FOREIGN KEY (userID) REFERENCES users(userID));";
            connector.ExecuteNonQuaryCommand(creationQuery);
        }
    }
}
