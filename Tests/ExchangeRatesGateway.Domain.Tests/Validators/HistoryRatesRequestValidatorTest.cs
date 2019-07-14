using System;
using ExchangeRatesGateway.Domain.Validators;
using FluentValidation.TestHelper;
using FluentValidation.Validators;
using FluentValidation.Validators.UnitTestExtension.Composer;
using FluentValidation.Validators.UnitTestExtension.Core;
using Xunit;

namespace ExchangeRatesGateway.Domain.Tests.Validators
{
    public class HistoryRatesRequestValidatorTest
    {
        private readonly HistoryRatesRequestValidator _sut;
        
        public HistoryRatesRequestValidatorTest()
        {
            _sut = new HistoryRatesRequestValidator();
        }


        [Fact]
        public void BaseCurrencyProperty_WhenHistoryRatesRequestValidatorConstructing_ShouldConfigureRulesCorrectly()
        {
            _sut.ShouldHaveRules(x => x.BaseCurrency,
                BaseVerifiersSetComposer.Build()
                    .AddPropertyValidatorVerifier<NotNullValidator>()
                    .AddPropertyValidatorVerifier<NotEmptyValidator>()
                    .AddPropertyValidatorVerifier<HistoryRatesRequestValidator.CurrencyLengthPropertyValidator>()
                    .Create());
        }
        
        [Fact]
        public void TargetCurrencyProperty_WhenHistoryRatesRequestValidatorConstructing_ShouldConfigureRulesCorrectly()
        {
            _sut.ShouldHaveRules(x => x.TargetCurrency,
                BaseVerifiersSetComposer.Build()
                    .AddPropertyValidatorVerifier<NotNullValidator>()
                    .AddPropertyValidatorVerifier<NotEmptyValidator>()
                    .AddPropertyValidatorVerifier<HistoryRatesRequestValidator.CurrencyLengthPropertyValidator>()
                    .Create());
        }
        
        [Fact]
        public void DatesProperty_WhenHistoryRatesRequestValidatorConstructing_ShouldConfigureRulesCorrectly()
        {
            _sut.ShouldHaveRules(x => x.Dates,
                BaseVerifiersSetComposer.Build()
                    .AddPropertyValidatorVerifier<NotNullValidator>()
                    .AddPropertyValidatorVerifier<NotEmptyValidator>()
                    .AddPropertyValidatorVerifier<HistoryRatesRequestValidator.FutureDatePropertyValidator>()
                    .AddPropertyValidatorVerifier<HistoryRatesRequestValidator.PastDatePropertyValidator>()
                    .Create());
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
        public void BaseCurrencyProperty_WhenInvalid_ShouldReturnErrors(string baseCurrency)
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
        public void TargetCurrencyProperty_WhenInvalid_ShouldReturnErrors(string targetCurrency)
        {
            _sut.ShouldHaveValidationErrorFor(historyRatesRequest => historyRatesRequest.TargetCurrency, targetCurrency);
        }

        [Fact]
        public void BaseCurrencyProperty_WhenProvidedThreeLetterCode_ShouldNotReturnException()
        {
            _sut.ShouldNotHaveValidationErrorFor(historyRatesRequest => historyRatesRequest.BaseCurrency, "EUR");
        }
        
        [Fact]
        public void TargetCurrencyProperty_WhenProvidedThreeLetterCode_ShouldNotReturnException()
        {
            _sut.ShouldNotHaveValidationErrorFor(historyRatesRequest => historyRatesRequest.TargetCurrency, "EUR");
        }

        [Fact]
        public void DatesProperty_WhenDateIsInFuture_ShouldReturnErrors()
        {
            _sut.ShouldHaveValidationErrorFor(x => x.Dates, new[] {DateTime.Now.AddDays(1)});
        }
        
        [Fact]
        public void DatesProperty_WhenDateIsBefore_1999_01_04_ShouldReturnErrors()
        {
            _sut.ShouldHaveValidationErrorFor(x => x.Dates, new[] { new DateTime(1999,1,3) });
        }

        [Fact]
        public void DatesProperty_WhenDateIsBetween_1999_01_04_And_Current_ShouldSucceed()
        {
            _sut.ShouldNotHaveValidationErrorFor(x => x.Dates, new []{ new DateTime(1999,1,4), DateTime.Now, new DateTime(2018,1,4) });
        }
    }
}