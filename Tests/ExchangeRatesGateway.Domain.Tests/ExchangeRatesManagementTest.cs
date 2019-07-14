using System;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRatesGateway.Domain.Exceptions;
using ExchangeRatesGateway.Domain.Model;
using ExchangeRatesGateway.Domain.Validators;
using FluentValidation;
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
            var historyRatesRequestValidatorMock = new HistoryRatesRequestValidator();
            
            _sut = new ExchangeRatesManagement(httpClientMock.Object, historyRatesRequestValidatorMock);
        }

        [Fact]
        public void Constructor_WhenHttpClientArgumentIsNull_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>( () =>
            {
                var historyRatesRequestValidatorMock = new Mock<IValidator<HistoryRatesRequest>>();
                new ExchangeRatesManagement(null, historyRatesRequestValidatorMock.Object);
            });
        }
        
        [Fact]
        public void Constructor_WhenHistoryRatesRequestValidatorArgumentIsNull_ShouldThrowArgumentNullException()
        {
            
            Assert.Throws<ArgumentNullException>( () =>
            {
                var httpClientMock = new Mock<HttpClient>();
                new ExchangeRatesManagement(httpClientMock.Object, null);
            });
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