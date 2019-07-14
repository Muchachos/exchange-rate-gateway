using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ExchangeRatesGateway.Domain.Exceptions;
using ExchangeRatesGateway.Domain.Model;
using ExchangeRatesGateway.Domain.ValueObject;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("ExchangeRatesGateway.Domain.Tests")]
[assembly: InternalsVisibleTo("ExchangeRatesGateway.API.Tests")]
namespace ExchangeRatesGateway.Domain
{
    internal class ExchangeRatesManagement : IExchangeRatesManagement
    {
        private readonly HttpClient _httpClient;
        private const string _EXCHANGE_RATES_API = "https://api.exchangeratesapi.io/";

        public ExchangeRatesManagement(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClient),$"Cannot resolve {nameof(HttpClient)}");
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

            CheckCurrency(request.BaseCurrency, nameof(request.BaseCurrency));
            CheckCurrency(request.TargetCurrency, nameof(request.TargetCurrency));
            
            if(string.Equals(request.BaseCurrency, request.TargetCurrency))
                throw new CurrencyException("Base currency cannot be the same as target currency", nameof(request.BaseCurrency));
                
            CheckDates(request.Dates, nameof(request.Dates));
        }

        private static void CheckCurrency(string currency, string parameterName)
        {
            if(string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be null, empty or whitespace", parameterName);

            if(currency.Trim().Length != 3)
                throw new CurrencyException("Invalid currency format", parameterName);
        }

        private static void CheckDates(DateTime[] dates, string parameterName)
        {
            if(dates.Any(x=> x > DateTime.Now))
                throw new DateException("Cannot look for dates in future", parameterName);
            if(dates.Any(x => x < new DateTime(1999,1,4)))
                throw new DateException("Cannot look for dates before 1999-01-04", parameterName);
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
                    $"{_EXCHANGE_RATES_API}{date:yyyy-MM-dd}?base={request.BaseCurrency.ToUpper()}&symbols={request.TargetCurrency.ToUpper()}"))
                .ToArray();
        }
    }
}