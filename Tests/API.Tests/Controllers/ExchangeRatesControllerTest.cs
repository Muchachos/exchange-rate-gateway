using System;
using System.Threading.Tasks;
using ExchangeRateGateway.API.Controllers;
using ExchangeRateGateway.Domain;
using ExchangeRateGateway.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ExchangeRateGateway.API.Tests.Controllers
{
    public class ExchangeRatesControllerTest
    {
        private readonly ExchangeRatesController _sut;
        
        public ExchangeRatesControllerTest()
        {
            var exchangeRatesManagementMoo = new Mock<IExchangeRatesManagement>();
            
            _sut = new ExchangeRatesController(exchangeRatesManagementMoo.Object);
        }

        [Fact]
        public void Constructor_WhenArgumentIsNull_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ExchangeRatesController(null));
        }

        [Fact]
        public async Task GetHistoryRatesForGivenPeriodsAsync_WhenArgumentIsNull_ShouldReturnBadRequest()
        {
            var result = await _sut.GetHistoryRatesForGivenPeriodsAsync(null);
            
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("E")]
        [InlineData("EU")]
        [InlineData("EU ")]
        [InlineData("EURO")]
        [InlineData(" E ")]
        public async Task GetHistoryRatesForGivenPeriodsAsync_WhenBaseCurrencyIsInvalid_ShouldReturnBadRequest(string baseCurrency)
        {
            var result = await _sut.GetHistoryRatesForGivenPeriodsAsync(new HistoryRatesRequest
            {
                Dates = new []{ (DateTime?)DateTime.Now}, 
                BaseCurrency = baseCurrency, 
                TargetCurrency = "EUR"
            });
            
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("E")]
        [InlineData("EU")]
        [InlineData("EU ")]
        [InlineData("EURO")]
        [InlineData(" E ")]
        public async Task GetHistoryRatesForGivenPeriodsAsync_WhenTargetCurrencyIsInvalid_ShouldReturnBadRequest(string targetCurrency)
        {
            var result = await _sut.GetHistoryRatesForGivenPeriodsAsync(new HistoryRatesRequest
            {
                Dates = new []{ (DateTime?)DateTime.Now}, 
                BaseCurrency = "EUR", 
                TargetCurrency = targetCurrency
            });
            
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}