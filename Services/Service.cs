namespace TestUtilitiesCalculation.Services
{
    // Класс для хранение данных об услуге
    public class Service
    {
        public int ID { get; set; }     // ID услуги
        public string nameOfService { get; set; }       // название услуги
        public double tariffValue { get; set; }     // значение тарифа услуги
        public double? norm { get; set; }       // значение норматива услуги
        public string measure { get; set; }     // величина измерения
    }
}
