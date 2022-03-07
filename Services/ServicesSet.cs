using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace TestUtilitiesCalculation.Services
{
    // Класс, представляющий собой хранилище для набора услуг, а также осуществляющий базовые операции с ними
    public class ServicesSet
    {
        private List<Service> services = new List<Service>();       // здес хранятся услуги

        // При создании класса устанавливается соединение с БД и достается набор всех услуг, после чего они добавляются в коллекцию
        public ServicesSet(Auxiliary.DatabaseConnector connector)
        {
            string selectTariffsQuery = "SELECT * FROM services";
            var reader = connector.ExecuteReaderCommand(selectTariffsQuery);
            while (reader.Read())
                addService(reader);
        }

        private void addService(SqliteDataReader reader)        // вспомогательный модуль для разбиения строки-ответа на составляющие услуги
        {
            var service = new Service();
            service.ID = reader.GetInt32(0);
            service.nameOfService = reader.GetString(1);
            service.tariffValue = reader.GetDouble(2);
            if (reader.IsDBNull(3)) service.norm = null; else service.norm = reader.GetDouble(3);
            service.measure = reader.GetString(4);
            this.services.Add(service);
        }

        public IEnumerator<Service> GetEnumerator() => services.GetEnumerator();        // IEnum для возможности перечисления услуг

        public double getNormByID(int ID)       // геттер норматива по указанному ID услуги
        {
            try
            {
                var value = (double)services.Find(x => x.ID == ID).norm;
                return value;
            }
            catch
            {
                return 0;
            }

        }
        public double getTariffByID(int ID)     // геттер тарифа по указанному ID услуги
        {
            try
            {
                var value = (double)services.Find(x => x.ID == ID).tariffValue;
                return value;
            }
            catch
            {
                return 0;
            }
        }

    }
}
