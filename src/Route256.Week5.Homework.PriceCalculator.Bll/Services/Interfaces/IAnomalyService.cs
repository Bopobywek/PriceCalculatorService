using Route256.Week5.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;

public interface IAnomalyService
{
    Task SaveAnomaly(long goodId, decimal price, CancellationToken cancellationToken);
}