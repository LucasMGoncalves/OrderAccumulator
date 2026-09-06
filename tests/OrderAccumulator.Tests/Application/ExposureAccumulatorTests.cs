using OrderAccumulator.Application.Services;
using OrderAccumulator.Domain.Entities;
using OrderAccumulator.Domain.Enum;

namespace OrderAccumulator.Tests.Application
{
    public sealed class ExposureAccumulatorTests
    {
        #region [TryApplyExposureAsync]

        [Fact]
        public async Task TryApplyExposureAsync_ShouldAcceptOrder_SingleSymbol()
        {
            var accumulator = new ExposureAccumulator();

            var order = new Order(
                "PETR4",
                OrderSide.BUY,
                10,
                1000m,
                Guid.NewGuid().ToString("N"));

            var result = await accumulator.TryApplyExposureAsync(order, CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task TryApplyExposureAsync_ShouldAcceptOrders_DifferentSymbolsAtSameTime()
        {
            var accumulator = new ExposureAccumulator();

            var orderOne = new Order(
                "PETR4",
                OrderSide.BUY,
                10,
                1000m,
                Guid.NewGuid().ToString("N"));

            var orderTwo = new Order(
                "VALE3",
                OrderSide.BUY,
                10,
                500m,
                Guid.NewGuid().ToString("N"));

            var results = await Task.WhenAll(
                accumulator.TryApplyExposureAsync(orderOne, CancellationToken.None),
                accumulator.TryApplyExposureAsync(orderTwo, CancellationToken.None));

            Assert.All(results, Assert.True);
        }

        [Fact]
        public async Task TryApplyExposureAsync_ShouldRejectOrder_WhenSymbolExceedsLimit()
        {
            var accumulator = new ExposureAccumulator();

            var orderOne = new Order(
                "PETR4",
                OrderSide.BUY,
                100_000,
                999.99m,
                Guid.NewGuid().ToString("N"));

            var orderTwo = new Order(
                "PETR4",
                OrderSide.BUY,
                1,
                1001m,
                Guid.NewGuid().ToString("N"));

            var resultOne = await accumulator.TryApplyExposureAsync(orderOne, CancellationToken.None);
            var resultTwo = await accumulator.TryApplyExposureAsync(orderTwo, CancellationToken.None);

            Assert.True(resultOne);
            Assert.False(resultTwo);
        }

        #endregion
    }
}
