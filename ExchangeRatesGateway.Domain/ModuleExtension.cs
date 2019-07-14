
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRatesGateway.Domain
{
    public static class ModuleExtension
    {
        public static IServiceCollection AddDomainModule(this IServiceCollection services)
        {
            services.AddTransient<IExchangeRatesManagement, ExchangeRatesManagement>();
            services.AddTransient<HttpClient>();
            
            return services;
        }
    }

}