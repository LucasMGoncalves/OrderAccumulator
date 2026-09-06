using Microsoft.Extensions.DependencyInjection;
using OrderAccumulator.Application.Interfaces;
using OrderAccumulator.Application.Services;

namespace OrderAccumulator.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
