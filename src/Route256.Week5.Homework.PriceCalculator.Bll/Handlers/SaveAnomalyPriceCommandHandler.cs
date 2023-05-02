using MediatR;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Handlers;

public class SaveAnomalyPriceCommandHandler : IRequestHandler<SaveAnomalyPriceCommand, Unit>
{
    private readonly IAnomalyService _anomalyService;

    public SaveAnomalyPriceCommandHandler(
        IAnomalyService anomalyService)
    {
        _anomalyService = anomalyService;
    }
    
    public async Task<Unit> Handle(SaveAnomalyPriceCommand request, CancellationToken cancellationToken)
    {
        await _anomalyService.SaveAnomaly(request.GoodId, request.Price, cancellationToken);
        
        return Unit.Value;
    }
}