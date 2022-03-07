using Microsoft.Data.Sqlite;
using System;

namespace TestUtilitiesCalculation.UsersData
{
    // Класс, овтечающий за хранения данных о пользователях и операций с ними
    public class User
    {
        public int Id { get; set; }     // ID пользователя
        public string residentialAddress { get; set; }      // Зарегистрированный адрес пребывания
        public bool hasColdWaterMeter { get; set; } = false;        // наличие счетчика холодной воды
        public bool hasHotWaterMeter { get; set; } = false;     // наличие счетчика горячей воды
        public bool hasElectricityMeter { get; set; } = false;      // наличие счетчика электричества
        public int residentsCount { get; set; }     // количество человек, проживающих по адресу

        public Meters.ColdMeter? coldMeter { get; private set; } = null;     // класс-счетчик холодной воды
        public Meters.HotMeter? hotMeter { get; private set; } = null;       // класс-счетчик горячей воды
        public Meters.ElectricityMeter? electricityMeter { get; private set; } = null;       // класс-счетчик электроэнергии

        // Методы для создания экземпляров классов счетчиков. Могут быть использованы в дальнейшем для реализации паттерна "Строитель"
        public void createColdWaterMeter()
        {
            this.coldMeter = new Meters.ColdMeter();
        }
        public void createHotWaterMeter()
        {
            this.hotMeter = new Meters.HotMeter();
        }
        public void createElectricityMeter()
        {
            this.electricityMeter = new Meters.ElectricityMeter();
        }

        // Метод клонирования для прототипирования класса пользователя
        public User Clone(bool hasColdMeter, bool hasHotMeter, bool hasElectricityMeter)
        {
            var user = new User();
            user.Id = Id;
            user.residentialAddress = residentialAddress;
            user.residentsCount = residentsCount;
            user.coldMeter = this.coldMeter.Clone();
            user.hotMeter = this.hotMeter.Clone();
            user.electricityMeter = this.electricityMeter.Clone();
            if (hasColdMeter) user.hasColdWaterMeter = true; else user.hasColdWaterMeter = false;
            if (hasHotMeter) user.hasHotWaterMeter = true; else user.hasHotWaterMeter = false;
            if (hasElectricityMeter) user.hasElectricityMeter = true; else user.hasElectricityMeter = false;

            return user;
        }

        // Метод для обновления данных о пользователе в случае подтверждения формы заполнения показаний
        public void refreshUser(User tempUser, Auxiliary.DatabaseConnector connector)
        {
            // Обновляем данные о конфигурации счетчиков...
            if (tempUser.hasColdWaterMeter) this.coldMeter = tempUser.coldMeter.Clone();
            if (tempUser.hasHotWaterMeter) this.hotMeter = tempUser.hotMeter.Clone();
            if (tempUser.hasElectricityMeter) this.electricityMeter = tempUser.electricityMeter.Clone();
            this.hasColdWaterMeter = tempUser.hasColdWaterMeter;
            this.hasHotWaterMeter = tempUser.hasHotWaterMeter;
            this.hasElectricityMeter = tempUser.hasElectricityMeter;

            // Создаем UPDATE запрос для обновления информации о наличии счетчиков и о накопленном объеме
            var updateQuery = String.Format("UPDATE users SET hasColdWaterMeter = {0}, hasHotWaterMeter = {1}, hasEnergyMeter = {2}, " +
                "coldWaterVolume = {3}, hotWaterSupplyVolume = {4}, HotWaterHeatingVolume = {5}, dayEnergyVolume = {6}, nightEnergyVolume = {7} " +
                "WHERE userID = {8};", this.hasColdWaterMeter ? "TRUE" : "FALSE", this.hasHotWaterMeter ? "TRUE" : "FALSE", this.hasElectricityMeter ? "TRUE" : "FALSE",
                this.coldMeter is null ? 0 : this.coldMeter.accumulatedVolume.ToString().Replace(',', '.'),
                this.hotMeter is null ? 0 : this.hotMeter.accumulatedVolume.ToString().Replace(',', '.'),
                this.hotMeter is null ? 0 : this.hotMeter.accumulatedHeatingVolume.ToString().Replace(',', '.'),
                this.electricityMeter is null ? 0 : this.electricityMeter.accumulatedVolume.ToString().Replace(',', '.'),
                 this.electricityMeter is null ? 0 : this.electricityMeter.accumulatedNightVolume.ToString().Replace(',', '.'), this.Id);
            connector.ExecuteNonQuaryCommand(updateQuery);
        }
    }
}
