using System;
using System.Runtime.CompilerServices;
using ExchangeRatesGateway.Domain.Model;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

#pragma warning disable 1591

[assembly: InternalsVisibleTo("ExchangeRatesGateway.API.Tests")]
[assembly: InternalsVisibleTo("ExchangeRatesGateway.Domain.Tests")]
namespace ExchangeRatesGateway.Domain.Validators
{
    internal class HistoryRatesRequestValidator : AbstractValidator<HistoryRatesRequest>
    {
        public HistoryRatesRequestValidator()
        {
            RuleFor(x => x.BaseCurrency)
                .NotNull()
                .WithMessage("Base currency cannot be null")
                .NotEmpty()
                .WithMessage("Base currency cannot be empty")
                .SetValidator(new CurrencyLengthPropertyValidator("Base currency format is not valid"));

            RuleFor(x => x.TargetCurrency)
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
                .SetValidator(new PastDatePropertyValidator("There are no data for dates before 1999-01-04"));
        }

        protected override bool PreValidate(ValidationContext<HistoryRatesRequest> context, ValidationResult result) {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("HistoryRatesRequest", "Please ensure a model was supplied."));
                return false;
            }
            if(string.Equals(context.InstanceToValidate.BaseCurrency, context.InstanceToValidate.TargetCurrency,StringComparison.InvariantCultureIgnoreCase))
            {
                result.Errors.Add(new ValidationFailure(nameof(context.InstanceToValidate.BaseCurrency), "Base currency cannot be the same as target currency."));
                return false;
            }
            
            return true;
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

                return !(date > DateTime.Now);
            }
        }

        internal class PastDatePropertyValidator : PropertyValidator
        {
            public PastDatePropertyValidator(string errorMessage) : base(errorMessage) { }

            protected override bool IsValid(PropertyValidatorContext context)
            {
                if (!(context.PropertyValue is DateTime date)) return false;

                return !(date < new DateTime(1999,1,4));
            }
        }
    }
}