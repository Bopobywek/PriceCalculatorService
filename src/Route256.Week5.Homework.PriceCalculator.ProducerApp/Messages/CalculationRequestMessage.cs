namespace Route256.Week5.Homework.PriceCalculator.ProducerApp.Messages;

public record CalculationRequestMessage(long GoodId, double Height, double Length, double Width, double Weight);