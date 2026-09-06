using OrderAccumulator.Domain.Enum;

namespace OrderAccumulator.Domain.Entities
{
    public class SymbolExposure
    {
        public const decimal EXPOSURE_LIMIT = 100_000_000m;

        public string Symbol { get; }
        public decimal Value { get; private set; }

        public SymbolExposure(string symbol)
        {
            Symbol = symbol;
        }

        public decimal CalculateNewValue(Order order)
        {
            var orderTotalPrice = order.Price * order.Amount;

            if (order.Side == OrderSide.BUY)
                return Value + orderTotalPrice;
            else if (order.Side == OrderSide.SELL)
                return Value - orderTotalPrice;
            else
                return Value;
        }

        public bool HasLimit(decimal value)
        {
            return Math.Abs(value) <= EXPOSURE_LIMIT;
        }

        public void ApplyNewValue(decimal value)
        {
            Value = value;
        }
    }
}
