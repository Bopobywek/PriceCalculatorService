using FluentValidation;

namespace Route256.Week5.Homework.PriceCalculator.Api.GrpcServices.Validators;

public class ClearHistoryRequestValidator : AbstractValidator<ClearHistoryRequest>
{
    public ClearHistoryRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);
        
        RuleForEach(x => x.CalculationIds)
            .GreaterThan(0);
    }
}