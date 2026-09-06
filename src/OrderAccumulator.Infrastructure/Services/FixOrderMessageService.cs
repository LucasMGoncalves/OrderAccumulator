using OrderAccumulator.Application.Interfaces;
using OrderAccumulator.Domain.Entities;
using OrderAccumulator.Domain.Enum;
using QuickFix.DataDictionary;
using QuickFix.Fields;
using QuickFix.FIX44;
using System.Globalization;

namespace OrderAccumulator.Infrastructure.Services
{
    public class FixOrderMessageService
    : IFixOrderMessageService
    {
        private readonly DataDictionary _dataDictionary;

        public FixOrderMessageService()
        {
            var dictionaryPath = Path.Combine($"{AppContext.BaseDirectory}/Resources","FIX44.xml");

            if (!File.Exists(dictionaryPath))
            {
                throw new FileNotFoundException(
                    "O arquivo FIX44.xml não foi encontrado.",
                    dictionaryPath);
            }

            _dataDictionary = new DataDictionary(dictionaryPath);
        }

        public Order Parse(string fixMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fixMessage))
                    throw new ArgumentException("A mensagem FIX é obrigatória.", nameof(fixMessage));

                //Apenas para facilitar a leitura/testes.
                //var normalizedMessage = fixMessage.Replace('|', QuickFix.Message.SOH);

                var message = new QuickFix.Message();

                message.FromString(
                    fixMessage,
                    true,
                    _dataDictionary,
                    _dataDictionary,
                    null);

                var symbol = message.GetString(Tags.Symbol);
                var side = ParseSide(message.GetString(Tags.Side));
                var amount = decimal.Parse(message.GetString(Tags.OrderQty), CultureInfo.InvariantCulture);
                var price = decimal.Parse(message.GetString(Tags.Price), CultureInfo.InvariantCulture);
                var clientOrderId = message.GetString(Tags.ClOrdID);

                return new Order(
                    symbol,
                    side,
                    amount,
                    price,
                    clientOrderId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Falha ao converter a mensagem FIX.", ex);
            }
        }

        private static OrderSide ParseSide(string side)
        {
            return side switch
            {
                "1" => OrderSide.BUY,
                "2" => OrderSide.SELL,
                _ => throw new ArgumentOutOfRangeException($"Lado {side} não suportado.")
            };
        }

        public string CreateExecutionReport(Order order, bool accepted)
        {
            var executionReport = new ExecutionReport();

            executionReport.OrderID = new OrderID(Guid.NewGuid().ToString("N"));
            
            executionReport.ExecID = new ExecID(Guid.NewGuid().ToString("N"));

            executionReport.ExecType = new ExecType(accepted ? ExecType.NEW : ExecType.REJECTED);
            
            executionReport.OrdStatus = new OrdStatus(accepted ? OrdStatus.NEW : OrdStatus.REJECTED);

            executionReport.ClOrdID = new ClOrdID(order.ClientOrderId);
            
            executionReport.Symbol = new Symbol(order.Symbol);

            executionReport.Side = new Side(order.Side == OrderSide.BUY ? Side.BUY : Side.SELL);

            executionReport.OrderQty = new OrderQty(order.Amount);

            executionReport.Price = new Price(order.Price);

            if (!accepted)
            {
                executionReport.OrdRejReason = new OrdRejReason(OrdRejReason.OTHER);
                executionReport.Text = new Text("Limite de exposição excedido.");
            }

            return executionReport.ConstructString();
        }
    }
}
