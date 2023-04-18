using Grpc.Core;
using MediatR;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Exceptions;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;
using Route256.Week5.Homework.PriceCalculator.Bll.Queries;
using Route256.Week5.Homework.PriceCalculator.GrpcServices.Validators;

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

        CalculateDeliveryPriceResult result;
        try
        {
            result = await _mediator.Send(command, context.CancellationToken);
        }
        catch (Exception exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, exception.Message));
        }

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

        var query = new ClearHistoryCommand(
            request.UserId,
            request.CalculationIds.ToArray()
        );

        try
        {
            await _mediator.Send(query, context.CancellationToken);
        }
        catch (OneOrManyCalculationsNotFoundException)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "One or manu calculations not found"));
        }
        catch (OneOrManyCalculationsBelongsToAnotherUserException)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "One or manu belongs to another user"));
        }
        catch (Exception exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, exception.Message));
        }

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

        const int take = 100;
        int skip = 0;
        GetHistoryQueryResult result;
        do
        {
            var query = new GetCalculationHistoryQuery(
                request.UserId,
                take,
                skip);

            try
            {
                result = await _mediator.Send(query, context.CancellationToken);
            }
            catch (Exception exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, exception.Message));
            }

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