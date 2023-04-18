using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Route256.Week5.Homework.PriceCalculator.GrpcServices.Options;

namespace Route256.Week5.Homework.PriceCalculator.GrpcServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<GrpcDeliveryPriceCalculatorOptions>(
                configuration.GetSection("GrpcDeliveryPriceCalculatorOptions"))
            .AddGrpcReflection()
            .AddGrpc();
        
        return services;
    }
}