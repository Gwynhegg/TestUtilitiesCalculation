namespace TestUtilitiesCalculation.UsersData.Meters
{
    // Конкретный счетчик для отслеживания энергопотребления
    public class ElectricityMeter : IMeter
    {
        public double accumulatedVolume { get; set; } = 0;      // накопленный объем дневного потребления электроэнергии
        public double accumulatedNightVolume { get; set; } = 0;         // накопленный объем ночного потребления электроэнергии

        // метод для прототипирования. Поскольку в нескольких местах необходимо поменять между юзерами счетчики, значения по ссылке могут остаться неизменными
        public ElectricityMeter Clone()
        {
            var meter = new ElectricityMeter();
            meter.accumulatedVolume = accumulatedVolume;
            meter.accumulatedNightVolume = accumulatedNightVolume;
            return meter;
        }

    }
}
