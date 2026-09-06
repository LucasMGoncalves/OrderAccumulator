using OrderAccumulator.Domain.Entities;
using OrderAccumulator.Domain.Enum;
using OrderAccumulator.Infrastructure.Services;
using QuickFix.Fields;

namespace OrderAccumulator.Tests.Infrastructure
{
    public class FixOrderMessageServiceTests
    {
        private const string FIX_MESSAGE = "8=FIX.4.4\u00019=97\u000135=D\u000111=0f856eb0b7ec4f4c9d16c0d736a0a7bc\u000138=1\u000140=2\u000144=100\u000154=1\u000155=PETR4\u000160=20260906-15:45:59.622\u000110=148\u0001";

        #region [Parse]

        [Fact]
        public void Parse_Success_WhenFixMessageIsValid()
        {
            var service = new FixOrderMessageService();

            var order = service.Parse(FIX_MESSAGE);

            Assert.Equal("PETR4", order.Symbol);
            Assert.Equal(OrderSide.BUY, order.Side);
            Assert.Equal(1, order.Amount);
            Assert.Equal(100m, order.Price);
        }

        [Fact]
        public void Parse_Fail_WhenFixMessageIsNotValid()
        {
            var service = new FixOrderMessageService();

            //Mensagem sem a tag 8 (BeginString)
            var fixInvalidMessage = FIX_MESSAGE.Replace("8=", "");

            var exception = Assert.Throws<InvalidOperationException>(
                () => service.Parse(fixInvalidMessage));

            Assert.Equal("Falha ao converter a mensagem FIX.", exception.Message);
        }

        #endregion

        #region [CreateExecutionReport]

        [Fact]
        public void CreateExecutionReport_ShouldCreateAcceptedReport()
        {
            var service = new FixOrderMessageService();

            var order = new Order(
                "PETR4",
                OrderSide.BUY,
                10,
                1000m,
                Guid.NewGuid().ToString("N"));

            var fixMessage = service.CreateExecutionReport(order, true);

            var message = new QuickFix.Message();

            message.FromString(
                fixMessage,
                false,
                null,
                null,
                null);

            Assert.Equal(order.Symbol, message.GetString(Tags.Symbol));
            Assert.Equal(Side.BUY.ToString(), message.GetString(Tags.Side));
            Assert.Equal(order.Amount.ToString(), message.GetString(Tags.OrderQty));
            Assert.Equal(order.Price.ToString(), message.GetString(Tags.Price));
            Assert.Equal(ExecType.NEW.ToString(), message.GetString(Tags.ExecType));
            Assert.Equal(OrdStatus.NEW.ToString(), message.GetString(Tags.OrdStatus));
        }

        #endregion

    }
}
