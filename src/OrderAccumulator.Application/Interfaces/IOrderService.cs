namespace OrderAccumulator.Application.Interfaces
{
    public interface IOrderService
    {
        Task<string> ProcessAsync(
            string fixMessage,
            CancellationToken cancellationToken);
    }
}
