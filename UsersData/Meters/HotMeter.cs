namespace TestUtilitiesCalculation.UsersData.Meters
{
    // Конкретный счетчик для горячей воды
    public class HotMeter : IMeter
    {
        public double accumulatedVolume { get; set; } = 0;      // накопленный объем поставки горячей воды
        public double accumulatedHeatingVolume { get; set; } = 0;       // накопленный объем подогрева горячей воды

        // метод для прототипирования. Поскольку в нескольких местах необходимо поменять между юзерами счетчики, значения по ссылке могут остаться неизменными
        public HotMeter Clone()
        {
            var meter = new HotMeter();
            meter.accumulatedVolume = accumulatedVolume;
            meter.accumulatedHeatingVolume = accumulatedHeatingVolume;
            return meter;
        }
    }
}
