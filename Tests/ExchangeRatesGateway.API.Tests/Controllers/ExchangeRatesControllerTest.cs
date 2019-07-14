using System;
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
    public class ExchangeRatesControllerTest
    {   
        private readonly ExchangeRatesController _sut;
        
        public ExchangeRatesControllerTest()
        {
            var loggerMock = new Mock<ILogger<ExchangeRatesController>>();
            var exchangeRatesManagementMock = new Mock<IExchangeRatesManagement>();
            
            _sut = new ExchangeRatesController(loggerMock.Object, exchangeRatesManagementMock.Object);
        }

        [Fact]
        public void Constructor_WhenLoggerArgumentIsNull_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var exchangeRatesManagementMock = new Mock<IExchangeRatesManagement>();
                new ExchangeRatesController(null, exchangeRatesManagementMock.Object);
            });
        }
        
        [Fact]
        public void Constructor_WhenExchangeRatesManagementArgumentIsNull_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var loggerMock = new Mock<ILogger<ExchangeRatesController>>();
                new ExchangeRatesController(loggerMock.Object, null);
            });
        }
        
        [Fact]
        public async Task GetHistoryRatesForGivenPeriodsAsync_WhenArgumentIsNull_ShouldReturnBadRequest()
        {
            var result = await _sut.GetHistoryRatesForGivenPeriodsAsync(null);
            
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public async Task GetHistoryRatesForGivenPeriodsAsync_WhenBManagementThrowsException_ShouldReturnBadRequest()
        {
            var loggerMock = new Mock<ILogger<ExchangeRatesController>>();
            var exchangeRatesManagementMock = new Mock<IExchangeRatesManagement>();
            
            exchangeRatesManagementMock
                .Setup( x=> 
                    x.GetRatesForGivenPeriodsAsync(It.IsAny<HistoryRatesRequest>()))
                .Throws<Exception>();
            
            var sut = new ExchangeRatesController(loggerMock.Object, exchangeRatesManagementMock.Object);
            
            var result = await sut.GetHistoryRatesForGivenPeriodsAsync(It.IsAny<HistoryRatesRequest>());
            
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}