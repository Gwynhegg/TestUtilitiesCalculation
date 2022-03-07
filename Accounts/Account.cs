using Microsoft.Data.Sqlite;
using System;

namespace TestUtilitiesCalculation.Accounts
{
    public class Account
    {
        // Константы для обращения к индексам услуг в таблице услуг
        const byte COLD_WATER_ID = 1, HOT_WATER_ID = 2, ELECTRICITY_ID = 3, ELECTRICITY_DAY_ID = 4, ELECTRICITY_NIGHT_ID = 5, HOT_WATER_DELIVERY_ID = 6, HOT_WATER_HEATING_ID = 7;

        // Константы для обращения к индексам показаний, передаваемым в виде массива
        const byte COLD_READING = 0, HOT_READING = 1, HEATING_READING = 2, ELECTRICITY_READING = 3, NIGHT_READING = 4;
        public int accountID { get; private set; }      // номер счета
        public int userID { get; private set; }     // ID пользователя
        public int dateOfReadings { get; private set; }     // дата подачи показаний
        public double totalResult { get; private set; } = 0;        // общая стоимость оказания услуг
        public string residentialAddress { get; private set; }      // адрес пользователя
        public IBill coldWaterAccount { get; private set; }     // отдельный счет для услуг ГВС
        public IBill hotWaterAccount { get; private set; }      // отдельный счет для услуг ХВС
        public VolumeBill? hotWaterHeatingAccount { get; private set; } = null;     // опциональный счет при наличии счетчиков. Отвечает за услуги нагрева
        public IBill electricityAccount { get; private set; }       //отдельный счет для услуг ЭЭ
        public VolumeBill? electricityNightAccount { get; private set; } = null;        // опциональный счет при наличии счетчиков. Отвечает за ночной тариф
        public bool accountRepaid { get; set; } = false;        // (бессмысленная переменная, отображающая, уплачен ли счет)

        // Синхронизация с данными пользователя, создание соответствующих счетов для каждой услуги: по показаниям или нормативу
        public void syncWithUser(UsersData.User user)
        {
            userID = user.Id;
            residentialAddress = user.residentialAddress;

            if (!user.hasColdWaterMeter)
                coldWaterAccount = new NormativeBill();
            else
                coldWaterAccount = new VolumeBill();

            if (!user.hasHotWaterMeter)
                hotWaterAccount = new NormativeBill();
            else
            {
                hotWaterAccount = new VolumeBill();
                hotWaterHeatingAccount = new VolumeBill();
            }

            if (!user.hasElectricityMeter)
                electricityAccount = new NormativeBill();
            else
            {
                electricityAccount = new VolumeBill();
                electricityNightAccount = new VolumeBill();
            }
        }

        // Метод для синхронизации с таблицей услуг. Нужен для задания базового значения при рассчете - норматива (в случае, если считаем не по показаниям)
        public void syncWithServices(Services.ServicesSet services)
        {
            if (coldWaterAccount is NormativeBill) coldWaterAccount.basicQuantity = services.getNormByID(COLD_WATER_ID);
            if (hotWaterAccount is NormativeBill) hotWaterAccount.basicQuantity = services.getNormByID(HOT_WATER_ID);
            if (electricityAccount is NormativeBill) electricityAccount.basicQuantity = services.getNormByID(ELECTRICITY_ID);
        }

        // Проверка корректности показаний. Переданное значение не должно превышать накопленного объема по счетчикам
        private bool checkCorrectness(UsersData.User user, double?[] readings)
        {
            if (readings[COLD_READING] != null)
                if (readings[COLD_READING] < user.coldMeter.accumulatedVolume) return false;
            if (readings[HOT_READING] != null)
                if (readings[HOT_READING] < user.hotMeter.accumulatedVolume) return false;
            if (readings[ELECTRICITY_READING] != null)
            {
                if (readings[ELECTRICITY_READING] < user.electricityMeter.accumulatedVolume) return false;
                if (readings[NIGHT_READING] < user.electricityMeter.accumulatedNightVolume) return false;
            }
            return true;


        }

        /// <summary>
        /// Метод для рассчета стоимости всех услуг
        /// </summary>
        /// <param name="user">Пользователь, относительно которого рассчитываются услуги</param>
        /// <param name="services">Список услуг с нормативной и тарифной информацией</param>
        /// <param name="readings">Показания, переданные пользователем</param>
        /// <exception cref="Exception">Провал проверки корректности введенных данных</exception>
        public void calculateResult(UsersData.User user, Services.ServicesSet services, double?[] readings)
        {
            if (!checkCorrectness(user, readings))      // проверяем данные на корректность
            {
                throw new Exception();
                return;
            }

            // В соответствии с типом счета передаем соответствующие показания и вызываем функцию расчета

            totalResult = 0;

            // Холодная вода:
            if (coldWaterAccount is NormativeBill)
                calculateColdWater(coldWaterAccount as NormativeBill, user, services, readings);
            else
                calculateColdWater(coldWaterAccount as VolumeBill, user, services, readings);

            // Горячая вода:
            if (hotWaterAccount is NormativeBill)
                calculateHotWater(hotWaterAccount as NormativeBill, user, services, readings);
            else
                calculateHotWater(hotWaterAccount as VolumeBill, user, services, readings);

            // Электричество:
            if (electricityAccount is NormativeBill)
                calculateElectricity(electricityAccount as NormativeBill, user, services, readings);
            else
                calculateElectricity(electricityAccount as VolumeBill, user, services, readings);

        }

        //Перегруженные методы для вычисления начислений при разной конфигурации ПУ
        private void calculateColdWater(NormativeBill coldWater, UsersData.User user, Services.ServicesSet services, double?[] readings)
        {
            totalResult += coldWaterAccount.makeCalculation(user.residentsCount, services.getTariffByID(COLD_WATER_ID));
        }

        private void calculateColdWater(VolumeBill coldWater, UsersData.User user, Services.ServicesSet services, double?[] readings)
        {
            totalResult += coldWaterAccount.makeCalculation((double)readings[COLD_READING] - user.coldMeter.accumulatedVolume, services.getTariffByID(COLD_WATER_ID));
            user.coldMeter.accumulatedVolume = (double)readings[COLD_READING];
        }

        private void calculateHotWater(NormativeBill hotWater, UsersData.User user, Services.ServicesSet services, double?[] readings)
        {
            totalResult += hotWaterAccount.makeCalculation(user.residentsCount, services.getTariffByID(HOT_WATER_ID));
        }

        private void calculateHotWater(VolumeBill hotWater, UsersData.User user, Services.ServicesSet services, double?[] readings)
        {
            totalResult += hotWaterAccount.makeCalculation((double)readings[HOT_READING] - user.hotMeter.accumulatedVolume, services.getTariffByID(HOT_WATER_DELIVERY_ID));
            totalResult += hotWaterHeatingAccount.makeCalculation(hotWaterAccount.basicQuantity * services.getNormByID(HOT_WATER_HEATING_ID), services.getTariffByID(HOT_WATER_HEATING_ID));
#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
            user.hotMeter.accumulatedVolume = (double)readings[HOT_READING];
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.
            user.hotMeter.accumulatedHeatingVolume = hotWaterAccount.basicQuantity * services.getNormByID(HOT_WATER_HEATING_ID);
        }

        private void calculateElectricity(NormativeBill electricity, UsersData.User user, Services.ServicesSet services, double?[] readings)
        {
            totalResult += electricityAccount.makeCalculation(user.residentsCount, services.getTariffByID(ELECTRICITY_ID));
        }

        private void calculateElectricity(VolumeBill electricity, UsersData.User user, Services.ServicesSet services, double?[] readings)
        {
            totalResult += electricityAccount.makeCalculation((double)readings[ELECTRICITY_READING] - user.electricityMeter.accumulatedVolume, services.getTariffByID(ELECTRICITY_DAY_ID));
            totalResult += electricityNightAccount.makeCalculation((double)readings[NIGHT_READING] - user.electricityMeter.accumulatedNightVolume, services.getTariffByID(ELECTRICITY_NIGHT_ID));
            user.electricityMeter.accumulatedVolume = (double)readings[ELECTRICITY_READING];
            user.electricityMeter.accumulatedNightVolume = (double)readings[NIGHT_READING];
        }

        /// <summary>
        /// Метод для определения последней записи об акте оплаты услуг конкретного пользователя
        /// </summary>
        /// <param name="connection">Соединение с базой данных</param>
        /// <param name="user">Конкретный пользователь, данные которого мы хотим найти</param>
        /// <returns>Возвращает индекс текущего неуплаченного месяца, по формуле (последний уплаченный месяц + 1)</returns>
        public int getDateOfLastAccount(Auxiliary.DatabaseConnector connector, UsersData.User user)
        {
            string getLastQuery = String.Format("SELECT dateOfReadings FROM accounts " +
                "WHERE userID = {0} ORDER BY dateOfReadings DESC LIMIT 1", user.Id);
            var request = connector.ExecuteScalarCommand(getLastQuery);

            // Если данных нет, значит, пользователь еще не пользовался услугами и начинает оплату с нулевого месяца
            if (request is null) dateOfReadings = 0; else dateOfReadings = Int32.Parse(request.ToString()) + 1;

            return dateOfReadings;
        }

        // Метод для записи результата транзакции в базу данных
        public void saveResult(Auxiliary.DatabaseConnector connector)
        {
            string insertTransactionQuery = String.Format("INSERT INTO accounts (" +
            "userID, residentialAddress, dateOfReadings, coldWaterAccount, hotWaterAccount, hotWaterHeatingAccount, ElectricityAccount, ElectricityNightAccount, totalResult,accountRepaid) " +
            "VALUES ({0}, '{1}', {2}, {3:0.##}, {4:0.##}, {5:0.##}, {6:0.##}, {7:0.##}, {8:0.##}, {9});", userID, residentialAddress, dateOfReadings, coldWaterAccount.totalCost.ToString().Replace(',', '.'),
            hotWaterAccount.totalCost.ToString().Replace(',', '.'), hotWaterHeatingAccount is null ? "NULL" : hotWaterHeatingAccount.totalCost.ToString().Replace(',', '.'),
            electricityAccount.totalCost.ToString().Replace(',', '.'), electricityNightAccount is null ? "NULL" : electricityNightAccount.totalCost.ToString().Replace(',', '.'),
            totalResult.ToString().Replace(',', '.'), true);
            connector.ExecuteNonQuaryCommand(insertTransactionQuery);
        }
    }
}
