
using System.Net.Http;
using ExchangeRatesGateway.Domain.Model;
using ExchangeRatesGateway.Domain.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRatesGateway.Domain
{
    public static class ModuleExtension
    {
        public static IServiceCollection AddDomainModule(this IServiceCollection services)
        {
            services.AddTransient<IExchangeRatesManagement, ExchangeRatesManagement>();
            services.AddTransient<IValidator<HistoryRatesRequest>, HistoryRatesRequestValidator>();

            services.AddTransient<HttpClient>();
            
            return services;
        }
    }

}