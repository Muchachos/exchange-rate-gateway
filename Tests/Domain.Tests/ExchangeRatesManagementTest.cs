using System;
using System.Threading.Tasks;
using ExchangeRateGateway.Domain.Exceptions;
using ExchangeRateGateway.Domain.Model;
using Xunit;

namespace ExchangeRateGateway.Domain.Tests
{
    public class ExchangeRatesManagementTest
    {
        private readonly ExchangeRatesManagement _sut;

        public ExchangeRatesManagementTest()
        {
            _sut = new ExchangeRatesManagement();
        }

        [Fact]
        public async Task GetRatesForGivenPeriodAsync_WhenArgumentIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>( () => _sut.GetRatesForGivenPeriodsAsync(null));
        }
        
        [Theory]
        [InlineData("E")]
        [InlineData("EU")]
        [InlineData("EU ")]
        [InlineData("EURO")]
        [InlineData(" E ")]
        public async Task GetRatesForGivenPeriodsAsync_WhenBaseCurrencyHasInvalidFormat_ShouldThrowCurrencyException(string baseCurrency)
        {
            await Assert.ThrowsAsync<CurrencyException>(async () => 
                await _sut
                        .GetRatesForGivenPeriodsAsync(
                            new HistoryRatesRequest(new []{ DateTime.Now}, baseCurrency, "EUR")));
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetRatesForGivenPeriodsAsync_WhenBaseCurrencyHasNullOrEmptyOrWhiteSpace_ShouldThrowArgumentException(string baseCurrency)
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await _sut
                    .GetRatesForGivenPeriodsAsync(
                        new HistoryRatesRequest(new []{ DateTime.Now}, baseCurrency, "EUR")));
        }
        
        
        [Theory]
        [InlineData("E")]
        [InlineData("EU")]
        [InlineData("EU ")]
        [InlineData("EURO")]
        [InlineData(" E ")]
        public async Task GetRatesForGivenPeriodsAsync_WhenTargetCurrencyHasInvalidFormat_ShouldThrowCurrencyException(string targetCurrency)
        {
            await Assert.ThrowsAsync<CurrencyException>(async () => 
                await _sut
                    .GetRatesForGivenPeriodsAsync(
                        new HistoryRatesRequest(new []{ DateTime.Now}, "EUR", targetCurrency)));
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetRatesForGivenPeriodsAsync_WhenTargetCurrencyHasNullOrEmptyOrWhiteSpace_ShouldThrowArgumentException(string targetCurrency)
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await _sut
                    .GetRatesForGivenPeriodsAsync(
                        new HistoryRatesRequest(new []{ DateTime.Now}, "EUR", targetCurrency)));
        }

        [Fact]
        public async Task GetRatesForGivenPeriodsAsync_WhenDateIsBefore_1999_01_0_ShouldThrowDateException()
        {
            await Assert.ThrowsAsync<DateException>(async () => 
                await _sut
                    .GetRatesForGivenPeriodsAsync(
                        new HistoryRatesRequest(new []{ new DateTime(1998,1,1), }, "EUR", "EUR")));
        }
        
        [Fact]
        public async Task GetRatesForGivenPeriodsAsync_WhenDateIsInFuture_ShouldThrowDateException()
        {
            await Assert.ThrowsAsync<DateException>(async () => 
                await _sut
                    .GetRatesForGivenPeriodsAsync(
                        new HistoryRatesRequest(new []{ DateTime.Now.AddDays(1), }, "EUR", "EUR")));
        }
    }
}