using OrderAccumulator.Domain.Entities;

namespace OrderAccumulator.Application.Interfaces
{
    public interface IExposureAccumulator
    {
        Task<bool> TryApplyExposureAsync(
            Order order,
            CancellationToken cancellationToken);
    }
}
