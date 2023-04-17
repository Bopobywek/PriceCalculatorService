using Grpc.Core;
using MediatR;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week5.Homework.PriceCalculator.GrpcServices.Services;

public class DeliveryPriceCalculatorService : DeliveryPriceCalculator.DeliveryPriceCalculatorBase
{
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;

    public DeliveryPriceCalculatorService(IMediator mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public override async Task<CalculationResponse> Calculate(CalculationRequest request, ServerCallContext context)
    {
        var command = new CalculateDeliveryPriceCommand(
            request.UserId,
            request.Goods
                .Select(x => new GoodModel(
                    x.Height,
                    x.Length,
                    x.Width,
                    x.Weight))
                .ToArray());

        await using var scope = _serviceProvider.CreateAsyncScope(); 
        var result = await _mediator.Send(command, context.CancellationToken);

        return new CalculationResponse
        {
            CalculationId = result.CalculationId,
            Price = DecimalValue.FromDecimal(result.Price)
        };
    }
}