namespace OrderAccumulator.Application.DTOs
{
    public class ExecutionReportResponse
    {
        public string ClientOrderId { get; init; } = string.Empty;

        public bool Accepted { get; init; }

        public string? Text { get; init; }
    }
}
