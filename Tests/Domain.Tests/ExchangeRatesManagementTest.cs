using System;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeRateGateway.Domain.Tests
{
    public class ExchangeRatesManagementTest
    {
        [Fact]
        public async Task GetRatesForGivenPeriodAsync_WhenArgumentIsNull_ShouldThrowArgumentNullException()
        {
            var sut = new ExchangeRatesManagement();
            
            await Assert.ThrowsAsync<ArgumentNullException>( () => sut.GetRatesForGivenPeriodsAsync(null));
        }
    }
}