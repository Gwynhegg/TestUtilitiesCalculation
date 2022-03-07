namespace TestUtilitiesCalculation.Accounts
{
    // Интерфейс, описывающий общую структуру сущности "Счёт"
    public interface IBill
    {
        // базовая величина в случае с оплатой по нормативу - количество человек, по объему - разница между показаниями и накопленным объемом
        public double basicQuantity { get; set; }

        // переменная для хранения конечной цены по счету
        public double totalCost { get; set; }

        // метод для проведения расчета. В классах-наследниках он переопределяется в соответствии с подходящим алгоритмом вычисления
        public double makeCalculation(double calculationValue, double tariff);
    }
}
