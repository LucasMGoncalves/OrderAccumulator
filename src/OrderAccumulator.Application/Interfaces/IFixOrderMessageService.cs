using OrderAccumulator.Domain.Entities;

namespace OrderAccumulator.Application.Interfaces
{
    public interface IFixOrderMessageService
    {
        Order Parse(string fixMessage);

        string CreateExecutionReport(
            Order order,
            bool accepted);
    }
}
