using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ExchangeRatesGateway.Domain.Exceptions;
using ExchangeRatesGateway.Domain.Model;
using ExchangeRatesGateway.Domain.Validators;
using ExchangeRatesGateway.Domain.ValueObject;
using FluentValidation;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("ExchangeRatesGateway.Domain.Tests")]
[assembly: InternalsVisibleTo("ExchangeRatesGateway.API.Tests")]
namespace ExchangeRatesGateway.Domain
{
    internal class ExchangeRatesManagement : IExchangeRatesManagement
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<HistoryRatesRequest> _historyRatesRequestValidator;
        private const string _EXCHANGE_RATES_API = "https://api.exchangeratesapi.io/";

        public ExchangeRatesManagement(HttpClient httpClient, IValidator<HistoryRatesRequest> historyRatesRequestValidator)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClient),$"Cannot resolve {nameof(HttpClient)}");
            _historyRatesRequestValidator = historyRatesRequestValidator ?? throw new ArgumentNullException(nameof(IValidator<HistoryRatesRequest>),$"Cannot resolve {nameof(IValidator<HistoryRatesRequest>)}");
        }
        
        public async Task<ExchangeRatesResponse> GetRatesForGivenPeriodsAsync(HistoryRatesRequest request)
        {
            ValidateArgument(request);

            var requestUrls = CreateRequestUrls(request);
            var result = (await GetRatesFromApiAsync(requestUrls))
                .OrderBy(x => x.Value)
                .ToList();

            var minimumRate = result.First();
            var maximumRate = result.Last();
            var averageRate = result.Average(x => x.Value);

            return new ExchangeRatesResponse(minimumRate, maximumRate, averageRate);
        }

        private void ValidateArgument(HistoryRatesRequest request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(HistoryRatesRequest), $"Argument {nameof(request)} cannot be null");
            
            var validationResult = _historyRatesRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                validationResult.Errors.Any(x =>
                {
                    switch (x.ErrorCode)
                    {
                        case nameof(HistoryRatesRequestValidator.CurrencyLengthPropertyValidator):
                            throw new CurrencyException(x.ErrorMessage, x.PropertyName);
                        case nameof(HistoryRatesRequestValidator.FutureDatePropertyValidator):
                        case nameof(HistoryRatesRequestValidator.PastDatePropertyValidator):
                            throw new DateException(x.ErrorMessage, x.PropertyName);
                        default:
                            throw new ArgumentException(x.ErrorMessage, x.PropertyName);
                    }
                });
            }
        }

        private static async Task<IEnumerable<Rate>> GetRatesFromApiAsync(Task<HttpResponseMessage>[] requestUrls)
        {
            var contentsAsyncArray = await Task
                .WhenAll(requestUrls)
                .ContinueWith(response => response.Result.Select(x => x.Content.ReadAsStringAsync()));

            var rates = await Task
                .WhenAll(contentsAsyncArray)
                .ContinueWith(response => response
                    .Result
                    .Select(jsonResult =>
                    {
                        var result = JsonConvert.DeserializeObject<ExternalRatesApiResponse>(jsonResult);
                        return new Rate(result.GetRate(), result.Date);
                    }));

            return rates;
        }

        private Task<HttpResponseMessage>[] CreateRequestUrls(HistoryRatesRequest request)
        {
            return request
                .Dates
                .Select(date => _httpClient.GetAsync(
                    $"{_EXCHANGE_RATES_API}{date:yyyy-MM-dd}?base={request.BaseCurrency}&symbols={request.TargetCurrency}"))
                .ToArray();
        }
    }
}