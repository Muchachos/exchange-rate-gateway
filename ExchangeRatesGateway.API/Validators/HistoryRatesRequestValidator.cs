using System;
using System.Runtime.CompilerServices;
using ExchangeRateGateway.Domain.Model;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

#pragma warning disable 1591

[assembly: InternalsVisibleTo("ExchangeRateGateway.ExchangeRatesGateway.API.Tests")]
namespace ExchangeRateGateway.API.Validators
{
    public class HistoryRatesRequestValidator : AbstractValidator<HistoryRatesRequest>
    {
        public HistoryRatesRequestValidator()
        {
            RuleFor(x => x.BaseCurrency)
                .NotNull()
                .WithMessage("Base currency cannot be null")
                .NotEmpty()
                .WithMessage("Base currency cannot be empty")
                .SetValidator(new CurrencyLengthPropertyValidator("Base currency format is not valid"));
            
            RuleFor(x=> x.TargetCurrency)
                .NotNull()
                .WithMessage("Target currency cannot be null")
                .NotEmpty()
                .WithMessage("Target currency cannot be empty")
                .SetValidator(new CurrencyLengthPropertyValidator("Target currency format is not valid"));

            RuleFor(x => x.Dates)
                .NotNull()
                .WithMessage("Provided date array cannot be null")
                .NotEmpty()
                .WithMessage("Provided date array cannot be empty");

            RuleForEach(x => x.Dates)
                .SetValidator(new FutureDatePropertyValidator("Date cannot be specified in the future"))
                .SetValidator(new NullDatePropertyValidator("Date cannot have null value"))
                .SetValidator(new PrehistoricDatePropertyValidator("There are no data for dates before 1999-01-04"));
        }
        
        protected override bool PreValidate(ValidationContext<HistoryRatesRequest> context, ValidationResult result) {
            if (context.InstanceToValidate != null) return true;
            
            result.Errors.Add(new ValidationFailure("HistoryRatesRequest", "Please ensure a model was supplied."));
            return false;
        }


        internal class CurrencyLengthPropertyValidator : PropertyValidator
        {
            public CurrencyLengthPropertyValidator(string errorMessage) : base(errorMessage) { }

            protected override bool IsValid(PropertyValidatorContext context)
            {
                if (!(context.PropertyValue is string currency)) return false;

                return currency.Trim().Length == 3;
            }
        }
       
        internal class FutureDatePropertyValidator : PropertyValidator
        {
            public FutureDatePropertyValidator(string errorMessage) : base(errorMessage) { }

            protected override bool IsValid(PropertyValidatorContext context)
            {
                if (!(context.PropertyValue is DateTime date)) return false;

                return !((DateTime?) date > DateTime.Now);
            }
        }
        
        internal class PrehistoricDatePropertyValidator : PropertyValidator
        {
            public PrehistoricDatePropertyValidator(string errorMessage) : base(errorMessage) { }

            protected override bool IsValid(PropertyValidatorContext context)
            {
                if (!(context.PropertyValue is DateTime date)) return false;

                return !((DateTime?) date < new DateTime(1999,1,4));
            }
        }
        
        internal class NullDatePropertyValidator : PropertyValidator
        {
            public NullDatePropertyValidator(string errorMessage) : base(errorMessage) { }

            protected override bool IsValid(PropertyValidatorContext context)
            {
                return context.PropertyValue is DateTime;
            }
        }
    }
}