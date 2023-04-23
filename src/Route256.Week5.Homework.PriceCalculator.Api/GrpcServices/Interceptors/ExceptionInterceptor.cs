using Grpc.Core;
using Grpc.Core.Interceptors;
using Route256.Week5.Homework.PriceCalculator.Bll.Exceptions;

namespace Route256.Week5.Homework.PriceCalculator.Api.GrpcServices.Interceptors;

public class ExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (OneOrManyCalculationsNotFoundException)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "One or many calculations not found"));
        }
        catch (OneOrManyCalculationsBelongsToAnotherUserException)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                "One or many calculations belongs to another user"));
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, exception.Message));
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(request, responseStream, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, exception.Message));
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(requestStream, responseStream, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, exception.Message));
        }
    }
}