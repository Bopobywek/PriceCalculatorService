using FluentValidation;
using Route256.Week5.Homework.PriceCalculator.Api.Requests.V1;

namespace Route256.Week5.Homework.PriceCalculator.Api.Validators.V1;

public class ClearHistoryRequestValidator : AbstractValidator<ClearHistoryRequest>
{
    public ClearHistoryRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);
        RuleFor(x => x.CalculationIds)
            .ForEach(x => x.GreaterThan(0));
    }
}