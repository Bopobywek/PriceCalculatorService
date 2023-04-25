using FluentValidation;

namespace Route256.Week5.Homework.PriceCalculator.Api.GrpcServices.Validators;

public class CalculationRequestValidator : AbstractValidator<CalculationRequest>
{
    public CalculationRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Goods)
            .NotEmpty();
        
        RuleForEach(x => x.Goods)
            .SetValidator(new GoodPropertiesValidator());
    }
}