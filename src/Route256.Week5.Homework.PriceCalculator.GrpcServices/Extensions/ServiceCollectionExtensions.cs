using Microsoft.Extensions.DependencyInjection;

namespace Route256.Week5.Homework.PriceCalculator.GrpcServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        services
            .AddGrpcReflection()
            .AddGrpc();
        
        return services;
    }
}