using MediatR;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Commands;

public record CalculateGoodDeliveryPriceCommand(GoodModel Good) : IRequest<CalculateGoodDeliveryPriceResult>;

public class CalculateGoodDeliveryPriceCommandHandler 
    : IRequestHandler<CalculateGoodDeliveryPriceCommand, CalculateGoodDeliveryPriceResult>
{
    private readonly ICalculationService _calculationService;

    public CalculateGoodDeliveryPriceCommandHandler(
        ICalculationService calculationService)
    {
        _calculationService = calculationService;
    }
    
    public Task<CalculateGoodDeliveryPriceResult> Handle(
        CalculateGoodDeliveryPriceCommand request, 
        CancellationToken cancellationToken)
    {
        var volumePrice = _calculationService.CalculatePriceByVolume(new []{ request.Good }, out _);
        var weightPrice = _calculationService.CalculatePriceByWeight(new []{ request.Good }, out _);
        var resultPrice = Math.Max(volumePrice, weightPrice);

        return Task.FromResult(new CalculateGoodDeliveryPriceResult(resultPrice));
    }
}
