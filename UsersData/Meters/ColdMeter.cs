namespace TestUtilitiesCalculation.UsersData.Meters
{
    // Конкретный счетчик холодной воды
    public class ColdMeter : IMeter
    {
        public double accumulatedVolume { get; set; } = 0;      // накопленный объем счетчика

        // метод для прототипирования. Поскольку в нескольких местах необходимо поменять между юзерами счетчики, значения по ссылке могут остаться неизменными
        public ColdMeter Clone()
        {
            var meter = new ColdMeter();
            meter.accumulatedVolume = accumulatedVolume;
            return meter;
        }
    }
}
