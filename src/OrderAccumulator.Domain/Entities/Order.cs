using OrderAccumulator.Domain.Enum;

namespace OrderAccumulator.Domain.Entities
{
    public class Order
    {
        public string Symbol { get; }
        public OrderSide Side { get; }
        public decimal Amount { get; }
        public decimal Price { get; }
        public string ClientOrderId { get; }

        public Order(
            string symbol,
            OrderSide side,
            decimal amount,
            decimal price,
            string clientOrderId)
        {
            Symbol = symbol;
            Side = side;
            Amount = amount;
            Price = price;
            ClientOrderId = clientOrderId;
        }

        public string? Validate()
        {
            if (string.IsNullOrWhiteSpace(Symbol))
                return "Simbolo é obrigatório.";

            if (Amount <= 0)
                return "Quantidade deve ser maior que 0.";

            if (Price <= 0)
                return "Preço deve ser maior que 0.";

            return null;
        }
    }
}
