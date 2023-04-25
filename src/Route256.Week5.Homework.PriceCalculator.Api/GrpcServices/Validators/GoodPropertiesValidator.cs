using FluentValidation;

namespace Route256.Week5.Homework.PriceCalculator.Api.GrpcServices.Validators;

public class GoodPropertiesValidator: AbstractValidator<Good>
{
    public GoodPropertiesValidator()
    {
        RuleFor(x => x.Height)
            .GreaterThan(0);

        RuleFor(x => x.Length)
            .GreaterThan(0);

        RuleFor(x => x.Width)
            .GreaterThan(0);

        RuleFor(x => x.Weight)
            .GreaterThan(0);
    }
}