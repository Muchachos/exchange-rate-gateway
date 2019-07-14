
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRatesGateway.Domain
{
    public static class ModuleExtension
    {
        public static IServiceCollection AddDomainModule(this IServiceCollection services)
        {
            services.AddTransient<IExchangeRatesManagement, ExchangeRatesManagement>();            
            return services;
        }
    }

}