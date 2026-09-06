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

            //Necessário ser singleton para manter o estado do acumulador de exposição durante a vida útil da aplicação
            services.AddSingleton<IExposureAccumulator, ExposureAccumulator>();

            return services;
        }
    }
}
