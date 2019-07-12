using ExchangeRateGateway.Domain.Model;
using FluentValidation;
using FluentValidation.Results;
#pragma warning disable 1591

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
                .Must(baseCurrency => baseCurrency != null && baseCurrency.Trim().Length == 3)
                .WithMessage("Base currency format is not valid");
            
            RuleFor(x=> x.TargetCurrency)
                .NotNull()
                .WithMessage("Target currency cannot be null")
                .NotEmpty()
                .WithMessage("Target currency cannot be empty")
                .Must(targetCurrency => targetCurrency != null && targetCurrency.Trim().Length == 3)
                .WithMessage("Target currency format is not valid");

            RuleFor(x => x.Dates)
                .NotNull()
                .WithMessage("Provided date array cannot be null")
                .NotEmpty()
                .WithMessage("Provided date array cannot be empty");
        }
        
        protected override bool PreValidate(ValidationContext<HistoryRatesRequest> context, ValidationResult result) {
            if (context.InstanceToValidate != null) return true;
            
            result.Errors.Add(new ValidationFailure("HistoryRatesRequest", "Please ensure a model was supplied."));
            return false;
        }
    }
}