namespace Route256.Week5.Homework.PriceCalculator.BackgroundServices.Messages;

public sealed record CalculationResultMessage(long GoodId, decimal Price);