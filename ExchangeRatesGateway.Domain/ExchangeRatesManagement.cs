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

namespace ExchangeRatesGateway.Domain
{
    internal class ExchangeRatesManagement : IExchangeRatesManagement
    {
        private static readonly HttpClient HTTP_CLIENT = new HttpClient();
        private const string _EXCHANGE_RATES_API = "https://api.exchangeratesapi.io/";

        public async Task<ExchangeRatesResponse> GetRatesForGivenPeriodsAsync(HistoryRatesRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(HistoryRatesRequest), "Argument cannot be null");

            CheckCurrency(request.BaseCurrency, nameof(request.BaseCurrency));
            CheckCurrency(request.TargetCurrency, nameof(request.TargetCurrency));
            CheckDates(request.Dates, nameof(request.Dates));
            
            var requestUrls = CreateRequestUrls(request);
            var result = (await GetRatesFromApiAsync(requestUrls))
                .OrderBy(x => x.Value)
                .ToList();

            var minimumRate = result.First();
            var maximumRate = result.Last();
            var averageRate = result.Average(x => x.Value);

            return new ExchangeRatesResponse(minimumRate, maximumRate, averageRate);
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

        private static void CheckDates(DateTime[] dates, string parameterName)
        {
            if (!dates.Any(x => x > DateTime.Now || x < new DateTime(1999,1,4))) return;

            var message = string.Join(',', dates.Where(x => x > DateTime.Now || x < new DateTime(1999,1,4)));
            throw new DateException($"Dates: {message} cannot be in the future or before 1999-01-4", parameterName);
        }

        private static void CheckCurrency(string currency, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Argument cannot be null, empty or whitespace", parameterName);
            if (currency.Trim().Length != 3)
                throw new CurrencyException("Invalid currency format", parameterName);
        }

        private static Task<HttpResponseMessage>[] CreateRequestUrls(HistoryRatesRequest request)
        {
            return request
                .Dates
                .Select(date => HTTP_CLIENT.GetAsync(
                    $"{_EXCHANGE_RATES_API}{date:yyyy-MM-dd}?base={request.BaseCurrency}&symbols={request.TargetCurrency}"))
                .ToArray();
        }
    }
}