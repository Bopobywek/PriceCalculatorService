using Microsoft.AspNetCore.Builder;
using Route256.Week5.Homework.PriceCalculator.GrpcServices.Services;

namespace Route256.Week5.Homework.PriceCalculator.GrpcServices.Extensions;

public static class WebApplicationExtensions
{
    public static void MapGrpc(this WebApplication app)
    {
        app.MapGrpcService<DeliveryPriceCalculatorService>();
        app.MapGrpcReflectionService();
    }
}