using ExchangeRateGateway.API.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace ExchangeRateGateway.API.Tests.Validators
{
    public class HistoryRatesRequestValidatorTest
    {
        private readonly HistoryRatesRequestValidator _sut;
        
        public HistoryRatesRequestValidatorTest()
        {
            _sut = new HistoryRatesRequestValidator();
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
        public void BaseCurrency_WhenInvalid_ShouldReturnErrors(string baseCurrency)
        {
            _sut.ShouldHaveValidationErrorFor(historyRatesRequest => historyRatesRequest.BaseCurrency, baseCurrency);
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
        public void TargetCurrency_WhenInvalid_ShouldReturnErrors(string targetCurrency)
        {
            _sut.ShouldHaveValidationErrorFor(historyRatesRequest => historyRatesRequest.TargetCurrency, targetCurrency);
        }
        
        [Fact]
        public void BaseCurrency_WhenProvidedThreeLetterCode_ShouldNotReturnException()
        {
            _sut.ShouldNotHaveValidationErrorFor(historyRatesRequest => historyRatesRequest.BaseCurrency, "EUR");
        }
        
        [Fact]
        public void TargetCurrency_WhenProvidedThreeLetterCode_ShouldNotReturnException()
        {
            _sut.ShouldNotHaveValidationErrorFor(historyRatesRequest => historyRatesRequest.TargetCurrency, "EUR");
        }
    }
}