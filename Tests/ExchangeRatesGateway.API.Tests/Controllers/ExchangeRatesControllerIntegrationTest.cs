using System;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRatesGateway.API.Controllers;
using ExchangeRatesGateway.Domain;
using ExchangeRatesGateway.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExchangeRatesGateway.API.Tests.Controllers
{
    public class ExchangeRatesControllerIntegrationTest
    {
        [Fact]
        public async Task GetHistoryRatesForGivenPeriodsAsync_WhenModelIsValid_ShouldReturnOk()
        {
            var sut = new ExchangeRatesController(
                new Logger<ExchangeRatesController>(new LoggerFactory()),
                new ExchangeRatesManagement(new HttpClient()));
            
            var result = await sut.GetHistoryRatesForGivenPeriodsAsync(new HistoryRatesRequest(new []{ new DateTime(2018,1,1), DateTime.Now }, "USD", "EUR"));
            
            Assert.IsType<OkObjectResult>(result);
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
            var loggerMock = new Mock<ILogger<ExchangeRatesController>>();
            var exchangeRatesManagement = new ExchangeRatesManagement( new Mock<HttpClient>().Object);
            
            var sut = new ExchangeRatesController(loggerMock.Object, exchangeRatesManagement);
            
            var result = await sut.GetHistoryRatesForGivenPeriodsAsync(new HistoryRatesRequest(new []{ DateTime.Now}, baseCurrency, "EUR"));
            
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
            var loggerMock = new Mock<ILogger<ExchangeRatesController>>();
            var exchangeRatesManagement = new ExchangeRatesManagement(new Mock<HttpClient>().Object);
            
            var sut = new ExchangeRatesController(loggerMock.Object, exchangeRatesManagement);

            var result = await sut.GetHistoryRatesForGivenPeriodsAsync(new HistoryRatesRequest(new []{ DateTime.Now}, "EUR", targetCurrency ));

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetHistoryRatesForGivenPeriodsAsync_WhenDateIsBefore_1999_01_04_ShouldReturnBadRequest()
        {
            var loggerMock = new Mock<ILogger<ExchangeRatesController>>();
            var exchangeRatesManagement = new ExchangeRatesManagement(new Mock<HttpClient>().Object);
            
            var sut = new ExchangeRatesController(loggerMock.Object, exchangeRatesManagement);

            var result = await sut.GetHistoryRatesForGivenPeriodsAsync(new HistoryRatesRequest(new []{ new DateTime( 1995, 2, 3) },"EUR","EUR"));
            
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public async Task GetHistoryRatesForGivenPeriodsAsync_WhenDateIsInFuture_ShouldReturnBadRequest()
        {
            var loggerMock = new Mock<ILogger<ExchangeRatesController>>();
            var exchangeRatesManagement = new ExchangeRatesManagement(new Mock<HttpClient>().Object);
            
            var sut = new ExchangeRatesController(loggerMock.Object, exchangeRatesManagement);

            var result = await sut.GetHistoryRatesForGivenPeriodsAsync(new HistoryRatesRequest(new []{ DateTime.Now.AddDays(1) }, "EUR", "EUR"));
            
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}