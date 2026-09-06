using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAccumulator.Application.Interfaces;

namespace OrderAccumulator.Api.Controllers
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/orders")]
    [Authorize]
    public class OrdersController
    : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("receive")]
        [Consumes("text/plain")]
        [Produces("text/plain")]
        public async Task<IActionResult> Receive(CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(Request.Body);

            var fixMessage = await reader.ReadToEndAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(fixMessage))
                return BadRequest("A mensagem FIX é obrigatória.");

            try
            {
                var executionReport = await _orderService.ProcessAsync(
                    fixMessage,
                    cancellationToken);

                return Content(executionReport, "text/plain");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao processar mensagem FIX: {ex.Message}");
            }
        }
    }
}
