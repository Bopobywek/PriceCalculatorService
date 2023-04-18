using FluentValidation;

namespace Route256.Week5.Homework.PriceCalculator.GrpcServices.Validators;

public class GetHistoryRequestValidator : AbstractValidator<GetHistoryRequest>
{
    public GetHistoryRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);
    }
}