namespace TestUtilitiesCalculation.UsersData.Meters
{
    // Интерфейс для логического объединения всех типов счетчиков
    public interface IMeter
    {
        public double accumulatedVolume { get; set; }
    }
}
