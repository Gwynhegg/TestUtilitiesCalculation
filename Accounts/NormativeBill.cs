namespace TestUtilitiesCalculation.Accounts
{
    public class NormativeBill : IBill
    {
        // В случае с использованием нормативов - величина норматива за услугу
        public double basicQuantity { get; set; }

        public double totalCost { get; set; }

        /// <summary>
        /// Расчет стоимости услуги по нормативу, объем рассчитывается по формуле n x N
        /// </summary>
        /// <param name="calculationValue">В данном случае основным параметром является количество жильцов в квартире</param>
        /// <param name="tariff">Тариф, по которому будет рассчитана величина</param>
        /// <returns>Возвращает конечную стоимость услуги</returns>
        public double makeCalculation(double calculationValue, double tariff)
        {
            var volumeOfService = basicQuantity * calculationValue;
            totalCost = volumeOfService * tariff;
            return totalCost;
        }
    }
}
