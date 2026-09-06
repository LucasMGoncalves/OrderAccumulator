using OrderAccumulator.Application.Interfaces;

namespace OrderAccumulator.Application.Services
{
    public sealed class OrderService : IOrderService
    {
        private readonly IFixOrderMessageService _fixOrderMessageService;
        private readonly IExposureAccumulator _exposureAccumulator;

        public OrderService(
            IFixOrderMessageService fixOrderMessageService,
            IExposureAccumulator exposureAccumulator)
        {
            _fixOrderMessageService = fixOrderMessageService;
            _exposureAccumulator = exposureAccumulator;
        }

        public async Task<string> ProcessAsync(
            string fixMessage,
            CancellationToken cancellationToken)
        {
            var order = _fixOrderMessageService.Parse(fixMessage);

            var validationError = order.Validate();

            if (validationError is not null)
                return _fixOrderMessageService.CreateExecutionReport(order, false);

            var accepted = await _exposureAccumulator.TryApplyExposureAsync(
                order,
                cancellationToken);

            return _fixOrderMessageService.CreateExecutionReport(
                order,
                accepted);
        }
    }
}
