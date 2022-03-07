namespace TestUtilitiesCalculation.Accounts
{
    public class VolumeBill : IBill
    {
        public double basicQuantity { get; set; }

        public double totalCost { get; set; }

        /// <summary>
        /// Расчет стоимости услуги по показаниям приборов
        /// </summary>
        /// <param name="calculationValue">Разница между объемом, указанном на счетчике, и накопленным объемом пользователя</param>
        /// <param name="tariff">Тариф, по которому будет рассчитана величина</param>
        /// <returns>Возвращает конечную стоимость услуги</returns>
        public double makeCalculation(double calculationValue, double tariff)
        {
            basicQuantity = calculationValue;
            totalCost = tariff * basicQuantity;
            return totalCost;
        }
    }
}
