using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace TestUtilitiesCalculation.UsersData
{
    // Класс, представляющий собой хранилище для данных пользователей, а также осуществляющий базовые операции с ними
    public class UsersSet
    {
        private List<User> users = new List<User>();        // Коллекция пользователей

        // Конструктор, устанавливающий соединение с базой данных и заполняющий коллекцию пользователей
        public UsersSet(Auxiliary.DatabaseConnector connector)
        {
            string selectTariffsQuery = "SELECT * FROM users";
            var reader = connector.ExecuteReaderCommand(selectTariffsQuery);
            while (reader.Read())
                addUser(reader);
        }

        private void addUser(SqliteDataReader reader)       // вспомогательный модуль для разбиения строки-ответа на составляющие
        {
            var user = new User();
            user.Id = reader.GetInt32(0);
            user.residentialAddress = reader.GetString(1);
            user.residentsCount = reader.GetInt32(2);

            bool hasColdMeter = reader.GetBoolean(3);
            if (hasColdMeter) user.hasColdWaterMeter = true;
            user.createColdWaterMeter();
            user.coldMeter.accumulatedVolume = reader.GetDouble(6);

            bool hasHotMeter = reader.GetBoolean(4);
            if (hasHotMeter) user.hasHotWaterMeter = true;
            user.createHotWaterMeter();
            user.hotMeter.accumulatedVolume = reader.GetDouble(7);
            user.hotMeter.accumulatedHeatingVolume = reader.GetDouble(8);

            bool hasElectricityMeter = reader.GetBoolean(5);
            if (hasElectricityMeter) user.hasElectricityMeter = true;
            user.createElectricityMeter();
            user.electricityMeter.accumulatedVolume = reader.GetDouble(9);
            user.electricityMeter.accumulatedNightVolume = reader.GetDouble(10);

            users.Add(user);
        }

        // Метод для обновления списка пользователей. Нужен для отображения коллекции на главной форме после добавления нового пользователя
        public void Refresh(Auxiliary.DatabaseConnector connector)
        {
            string selectTariffsQuery = "SELECT * FROM users ORDER BY userID DESC LIMIT 1";
            var reader = connector.ExecuteReaderCommand(selectTariffsQuery);
            while (reader.Read())
                addUser(reader);
        }

        public IEnumerator<User> GetEnumerator() => users.GetEnumerator();      // IEnum для итерации сквозь коллекцию

        public User getLast()
        {
            return users.Last<User>();
        }

        public User getByID(int ID)     // Получение конкретного пользователя по запросу ID
        {
            return users.Find(x => x.Id == ID);
        }
    }
}
