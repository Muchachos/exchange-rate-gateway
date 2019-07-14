using System;
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

        [Fact]
        public void Dates_WhenDateIsInFuture_ShouldReturnErrors()
        {
            _sut.ShouldHaveValidationErrorFor(x => x.Dates, new[] {DateTime.Now.AddDays(1)});
        }
        
        [Fact]
        public void Dates_WhenDateIsBefore_1999_01_04_ShouldReturnErrors()
        {
            _sut.ShouldHaveValidationErrorFor(x => x.Dates, new[] { new DateTime(1999,1,3) });
        }

        [Fact]
        public void Dates_WhenDateIsBetween_1999_01_04_And_Current_ShouldSucceed()
        {
            _sut.ShouldNotHaveValidationErrorFor(x => x.Dates, new []{ new DateTime(1999,1,4), DateTime.Now, new DateTime(2018,1,4) });
        }
    }
}