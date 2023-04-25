namespace Route256.Week5.Homework.PriceCalculator.GrpcClient.Models;

public record GoodCalculationRequestModel(long GoodId, double Height, double Length, double Width, double Weight);