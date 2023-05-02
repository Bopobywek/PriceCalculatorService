using MediatR;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Commands;

public record SaveAnomalyPriceCommand(long GoodId, decimal Price) : IRequest<Unit>;
