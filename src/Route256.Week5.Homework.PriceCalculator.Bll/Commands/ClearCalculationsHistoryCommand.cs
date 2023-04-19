using MediatR;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Commands;

public record ClearCalculationsHistoryCommand(
    long UserId,
    long[] CalculationIds) : IRequest<ClearCalculationsHistoryResult>;

