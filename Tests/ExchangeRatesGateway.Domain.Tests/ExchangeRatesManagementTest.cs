using System;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRatesGateway.Domain.Exceptions;
using ExchangeRatesGateway.Domain.Model;
using Moq;
using Xunit;

namespace ExchangeRatesGateway.Domain.Tests
{
    public class ExchangeRatesManagementTest
    {
        private readonly ExchangeRatesManagement _sut;

        public ExchangeRatesManagementTest()
        {
            var httpClientMock = new Mock<HttpClient>();
            
            _sut = new ExchangeRatesManagement(httpClientMock.Object);
        }

        [Fact]
        public void Constructor_WhenHttpClientArgumentIsNull_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>( () =>
            {
                new ExchangeRatesManagement(null);
            });
        }
        
        
        [Fact]
        public async Task GetRatesForGivenPeriodAsync_WhenArgumentIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>( () => _sut.GetRatesForGivenPeriodsAsync(null));
        }

        [Fact]
        public async Task GetRatesForGivenPeriodsAsync_WhenBaseCurrencyAndTargetCurrencyAreSame_ShouldThrowCurrencyException()
        {
            await Assert.ThrowsAsync<CurrencyException>(() =>
                _sut.GetRatesForGivenPeriodsAsync(new HistoryRatesRequest(It.IsAny<DateTime[]>(), "EUR", "EUR")));
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
                            new HistoryRatesRequest(It.IsAny<DateTime[]>(), baseCurrency, "EUR")));
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
                        new HistoryRatesRequest(It.IsAny<DateTime[]>(), baseCurrency, "EUR")));
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
                        new HistoryRatesRequest(It.IsAny<DateTime[]>(), "EUR", targetCurrency)));
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
                        new HistoryRatesRequest(It.IsAny<DateTime[]>(), "EUR", targetCurrency)));
        }

        [Fact]
        public async Task GetRatesForGivenPeriodsAsync_WhenDateIsBefore_1999_01_0_ShouldThrowDateException()
        {
            await Assert.ThrowsAsync<DateException>(async () => 
                await _sut
                    .GetRatesForGivenPeriodsAsync(
                        new HistoryRatesRequest(new []{ new DateTime(1998,1,1), }, "EUR", "USD")));
        }
        
        [Fact]
        public async Task GetRatesForGivenPeriodsAsync_WhenDateIsInFuture_ShouldThrowDateException()
        {
            await Assert.ThrowsAsync<DateException>(async () => 
                await _sut
                    .GetRatesForGivenPeriodsAsync(
                        new HistoryRatesRequest(new []{ DateTime.Now.AddDays(1), }, "EUR", "USD")));
        }
    }
}