using Microsoft.Data.Sqlite;

namespace TestUtilitiesCalculation.Auxiliary
{
    public sealed class DatabaseConnector
    {
        private DatabaseConnector() { }

        private static DatabaseConnector instance;
        private static SqliteConnection connection;

        public static DatabaseConnector getInstance()
        {
            if (instance == null) instance = new DatabaseConnector();
            return instance;
        }

        public void setDatabaseConnector(string connectionString)
        {
            connection = new SqliteConnection(connectionString);
        }
        // Коннектор для выполнения запросов без возвращения объекта
        public void ExecuteNonQuaryCommand(string commandQuery)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = commandQuery;
            command.ExecuteNonQuery();
        }

        // Коннектор для выполнения запросов с возвращение скалярного объекта
        public object? ExecuteScalarCommand(string commandQuery)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = commandQuery;
            return command.ExecuteScalar();
        }

        // Коннектор для выполнения запросов с возвратом набора значений
        public SqliteDataReader? ExecuteReaderCommand(string commandQuery)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = commandQuery;
            return command.ExecuteReader();
        }
    }
}
