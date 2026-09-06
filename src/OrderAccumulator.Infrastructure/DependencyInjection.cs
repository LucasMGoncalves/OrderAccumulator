using Microsoft.Extensions.DependencyInjection;
using OrderAccumulator.Application.Interfaces;
using OrderAccumulator.Infrastructure.Services;

namespace OrderAccumulator.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IFixOrderMessageService, FixOrderMessageService>();

            return services;
        }
    }
}
