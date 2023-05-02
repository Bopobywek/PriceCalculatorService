namespace Route256.Week5.Homework.PriceCalculator.ProducerApp.Interfaces;

public interface IProducerApp
{
    Task Run(CancellationToken cancellationToken);
}