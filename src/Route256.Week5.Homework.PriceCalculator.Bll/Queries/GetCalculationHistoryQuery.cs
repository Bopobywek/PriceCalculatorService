using MediatR;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Queries;

public record GetCalculationHistoryQuery(
        long UserId,
        int Take,
        int Skip,
        long[] CalculationIds)
    : IRequest<GetHistoryQueryResult>;

public class GetCalculationHistoryQueryHandler
    : IRequestHandler<GetCalculationHistoryQuery, GetHistoryQueryResult>
{
    private readonly ICalculationService _calculationService;

    public GetCalculationHistoryQueryHandler(
        ICalculationService calculationService)
    {
        _calculationService = calculationService;
    }

    public async Task<GetHistoryQueryResult> Handle(
        GetCalculationHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var query = new QueryCalculationFilter(
            request.UserId,
            request.Take,
            request.Skip);

        var log = await _calculationService.QueryCalculations(query, cancellationToken);

        if (request.CalculationIds.Length == 0)
        {
            return new GetHistoryQueryResult(
                log.Select(x => new GetHistoryQueryResult.HistoryItem(
                        x.TotalVolume,
                        x.TotalWeight,
                        x.Price,
                        x.GoodIds))
                    .ToArray());
        }

        var queryIds = request.CalculationIds.ToHashSet();

        return new GetHistoryQueryResult(
            log
                .Where(x => queryIds.Contains(x.Id))
                .Select(x => new GetHistoryQueryResult.HistoryItem(
                    x.TotalVolume,
                    x.TotalWeight,
                    x.Price,
                    x.GoodIds))
                .ToArray());
    }
}