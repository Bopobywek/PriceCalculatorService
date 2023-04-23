using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Options;
using Route256.Week5.Homework.PriceCalculator.Api.GrpcServices.Validators;
using Route256.Week5.Homework.PriceCalculator.Api.Options;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;
using Route256.Week5.Homework.PriceCalculator.Bll.Queries;

namespace Route256.Week5.Homework.PriceCalculator.Api.GrpcServices;

public class DeliveryPriceCalculatorService : DeliveryPriceCalculator.DeliveryPriceCalculatorBase
{
    private readonly IMediator _mediator;
    private GrpcDeliveryPriceCalculatorOptions _options;

    public DeliveryPriceCalculatorService(IMediator mediator,
        IOptionsMonitor<GrpcDeliveryPriceCalculatorOptions> optionsMonitor)
    {
        _mediator = mediator;
        _options = optionsMonitor.CurrentValue;

        optionsMonitor.OnChange(options => _options = options);
    }

    public override async Task<CalculationResponse> Calculate(CalculationRequest request, ServerCallContext context)
    {
        var validator = new CalculationRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, validationResult.ToString()));
        }

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
        var validator = new ClearHistoryRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, validationResult.ToString()));
        }

        var query = new ClearCalculationsHistoryCommand(
            request.UserId,
            request.CalculationIds.ToArray()
        );

        await _mediator.Send(query, context.CancellationToken);

        return new ClearHistoryResponse();
    }

    public override async Task GetHistory(GetHistoryRequest request,
        IServerStreamWriter<GetHistoryResponse> responseStream,
        ServerCallContext context)
    {
        var validator = new GetHistoryRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, validationResult.ToString()));
        }

        var skip = 0;
        GetHistoryQueryResult result;
        do
        {
            var take = _options.HistoryTake;
            var query = new GetCalculationHistoryQuery(
                request.UserId,
                take,
                skip,
                Array.Empty<long>());

            result = await _mediator.Send(query, context.CancellationToken);

            foreach (var historyItem in result.Items)
            {
                await responseStream.WriteAsync(new GetHistoryResponse
                {
                    Cargo = CargoResponse.Create(historyItem.Volume, historyItem.Weight, historyItem.GoodIds),
                    Price = DecimalValue.FromDecimal(historyItem.Price)
                });
            }

            skip += take;
        } while (result.Items.Length != 0);
    }
}