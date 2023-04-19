using MediatR;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Extensions;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Handlers;

public class ClearCalculationsHistoryCommandHandler
    : IRequestHandler<ClearCalculationsHistoryCommand, ClearCalculationsHistoryResult>
{
    private readonly ICalculationService _calculationService;

    public ClearCalculationsHistoryCommandHandler(
        ICalculationService calculationService)
    {
        _calculationService = calculationService;
    }

    public async Task<ClearCalculationsHistoryResult> Handle(
        ClearCalculationsHistoryCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CalculationIds.Length == 0)
        {
            await _calculationService.ClearCalculationsHistory(request.UserId, cancellationToken);
            return new ClearCalculationsHistoryResult();
        }

        var calculations = await _calculationService.QueryCalculations(request.CalculationIds,
            cancellationToken);
        
        request.EnsureAllCalculationsFound(calculations);
        request.EnsureAllCalculationsBelongsToOneUser(calculations);

        var goodsIds = calculations
            .Select(x => x.GoodIds)
            .SelectMany(x => x)
            .Distinct()
            .ToArray();

        await _calculationService.ClearCalculationsHistory(
            new ClearCalculationsHistoryModel(goodsIds, request.CalculationIds), cancellationToken);

        return new ClearCalculationsHistoryResult();
    }
}