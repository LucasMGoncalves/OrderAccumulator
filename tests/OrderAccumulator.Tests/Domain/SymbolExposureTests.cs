using OrderAccumulator.Domain.Entities;
using OrderAccumulator.Domain.Enum;

namespace OrderAccumulator.Tests.Domain
{
    public sealed class SymbolExposureTests
    {
        #region [HasLimit]

        [Fact]
        public void HasLimit_ShouldReturnTrue_WhenValueIsWithinLimit_Positive()
        {
            var exposure = new SymbolExposure("PETR4");

            var result = exposure.HasLimit(100_000_000m);

            Assert.True(result);
        }

        [Fact]
        public void HasLimit_ShouldReturnTrue_WhenValueIsWithinLimit_Negative()
        {
            var exposure = new SymbolExposure("PETR4");

            var result = exposure.HasLimit(-100_000_000m);

            Assert.True(result);
        }

        [Fact]
        public void HasLimit_ShouldReturnFalse_WhenValueExceedsLimit()
        {
            var exposure = new SymbolExposure("PETR4");

            var result = exposure.HasLimit(100_000_001m);

            Assert.False(result);
        }

        #endregion

        #region [CalculateNewValue]

        [Fact]
        public void CalculateNewValue_ShouldIncreaseValue_WhenOrderIsBuy()
        {
            var exposure = new SymbolExposure("PETR4");

            var order = new Order(
                "PETR4",
                OrderSide.BUY,
                2,
                100m,
                Guid.NewGuid().ToString("N"));

            var result = exposure.CalculateNewValue(order);

            Assert.Equal(200m, result);
        }

        [Fact]
        public void CalculateNewValue_ShouldDecreaseValue_WhenOrderIsSell()
        {
            var exposure = new SymbolExposure("PETR4");
            exposure.ApplyNewValue(200m);

            var order = new Order(
                "PETR4",
                OrderSide.SELL,
                1,
                100m,
                Guid.NewGuid().ToString("N"));

            var result = exposure.CalculateNewValue(order);

            Assert.Equal(100m, result);
        }

        [Fact]
        public void CalculateNewValue_ShouldReturnCurrentValue_WhenOrderSideIsInvalid()
        {
            var exposure = new SymbolExposure("PETR4");
            exposure.ApplyNewValue(200m);

            var order = new Order(
                "PETR4",
                (OrderSide)999,
                1,
                100m,
                Guid.NewGuid().ToString("N"));

            var result = exposure.CalculateNewValue(order);

            Assert.Equal(200m, result);
        }

        #endregion
    }
}
