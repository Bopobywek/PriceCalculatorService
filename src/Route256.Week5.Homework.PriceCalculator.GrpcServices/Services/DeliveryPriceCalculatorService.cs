using Grpc.Core;
using MediatR;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week5.Homework.PriceCalculator.GrpcServices.Services;

public class DeliveryPriceCalculatorService : DeliveryPriceCalculator.DeliveryPriceCalculatorBase
{
    private readonly IMediator _mediator;

    public DeliveryPriceCalculatorService(IMediator mediator)
    {
        _mediator = mediator;
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

        var result = await _mediator.Send(command, context.CancellationToken);

        return new CalculationResponse
        {
            CalculationId = result.CalculationId,
            Price = DecimalValue.FromDecimal(result.Price)
        };
    }

    public override async Task<ClearHistoryResponse> ClearHistory(ClearHistoryRequest request,
        ServerCallContext context)
    {
        var query = new ClearHistoryCommand(
            request.UserId,
            request.CalculationIds.ToArray()
        );
        
        await _mediator.Send(query, context.CancellationToken);

        return new ClearHistoryResponse();
    }
}