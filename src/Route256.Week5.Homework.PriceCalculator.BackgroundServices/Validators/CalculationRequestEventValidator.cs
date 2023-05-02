using FluentValidation;
using Route256.Week5.Homework.PriceCalculator.BackgroundServices.Messages;

namespace Route256.Week5.Homework.PriceCalculator.BackgroundServices.Validators;

public class CalculationRequestEventValidator : AbstractValidator<CalculationRequestMessage>
{
    public CalculationRequestEventValidator()
    {
        RuleFor(x => x.Height)
            .GreaterThan(0);
        RuleFor(x => x.Length)
            .GreaterThan(0);
        RuleFor(x => x.Weight)
            .GreaterThan(0);
        RuleFor(x => x.Width)
            .GreaterThan(0);
        RuleFor(x => x.GoodId)
            .GreaterThanOrEqualTo(0);
    }
}