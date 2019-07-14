using System;
using System.Threading.Tasks;
using ExchangeRateGateway.Domain.Model;
using Xunit;

namespace ExchangeRateGateway.Domain.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public async Task ExchangeRates_SEK_To_NOK_Returns_0970839476467_AsAverage()
        {
            var sut = new ExchangeRatesManagement();

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
            var sut = new ExchangeRatesManagement();

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
    }
}