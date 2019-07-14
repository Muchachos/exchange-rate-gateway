using System;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRatesGateway.Domain.Exceptions;
using ExchangeRatesGateway.Domain.Model;
using Xunit;

namespace ExchangeRatesGateway.Domain.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public async Task ExchangeRates_SEK_To_NOK_Returns_0970839476467_AsAverage()
        {
            var sut = new ExchangeRatesManagement(new HttpClient());

            var result = await sut.GetRatesForGivenPeriodsAsync(
                new HistoryRatesRequest(
                    new[]
                    {
                        new DateTime(2018, 2, 1),
                        new DateTime(2018, 2, 15),
                        new DateTime(2018, 3, 1)
                    }, "SEK", "NOK"));

            Assert.Equal(0.970839476467m, result.AverageRate, precision: 12);
        }

        [Fact]
        public async Task ExchangeRates_WhenInvalidCurrency_ShouldThrowException()
        {
            var sut = new ExchangeRatesManagement(new HttpClient());

            await Assert.ThrowsAsync<Exception>(async () =>
                await sut.GetRatesForGivenPeriodsAsync(
                    new HistoryRatesRequest(
                        new[]
                        {
                            new DateTime(2018, 2, 1),
                            new DateTime(2018, 2, 15),
                            new DateTime(2018, 3, 1)
                        }, 
                        "EEE", 
                        "NOK")));
        }
        
        [Fact]
        public async Task ExchangeRates_WhenBaseCurrencyAndTargetCurrencySame_ShouldThrowCurrencyException()
        {
            var sut = new ExchangeRatesManagement(new HttpClient());

            await Assert.ThrowsAsync<CurrencyException>(async () =>
                await sut.GetRatesForGivenPeriodsAsync(
                    new HistoryRatesRequest(
                        new[]
                        {
                            new DateTime(2018, 2, 1),
                            new DateTime(2018, 2, 15),
                            new DateTime(2018, 3, 1)
                        }, 
                        "NOK", 
                        "NOK")));
        }
        
        [Fact]
        public async Task ExchangeRates_WhenDateInFuture_ShouldThrowDateException()
        {
            var sut = new ExchangeRatesManagement(new HttpClient());

            await Assert.ThrowsAsync<DateException>(async () =>
                await sut.GetRatesForGivenPeriodsAsync(
                    new HistoryRatesRequest(
                        new[]
                        {
                            new DateTime(2028, 2, 1),
                            new DateTime(2018, 2, 15),
                            new DateTime(2018, 3, 1)
                        }, 
                        "SEK", 
                        "NOK")));
        }
        
        [Fact]
        public async Task ExchangeRates_WhenDateBefore_1999_01_04_ShouldThrowDateException()
        {
            var sut = new ExchangeRatesManagement(new HttpClient());

            await Assert.ThrowsAsync<DateException>(async () =>
                await sut.GetRatesForGivenPeriodsAsync(
                    new HistoryRatesRequest(
                        new[]
                        {
                            new DateTime(1998, 2, 1),
                            new DateTime(2018, 2, 15),
                            new DateTime(2018, 3, 1)
                        }, 
                        "SEK", 
                        "NOK")));
        }
    }
}