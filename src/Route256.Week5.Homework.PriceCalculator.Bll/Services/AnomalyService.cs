using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;
using Route256.Week5.Homework.PriceCalculator.Dal.Entities;
using Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Services;

public class AnomalyService : IAnomalyService
{
    private readonly IAnomaliesRepository _anomaliesRepository;

    public AnomalyService(
        IAnomaliesRepository anomaliesRepository)
    {
        _anomaliesRepository = anomaliesRepository;
    }
    
    public async Task SaveAnomaly(long goodId, decimal price, CancellationToken cancellationToken)
    {
        var anomalyEntity = new AnomalyEntityV1
        {
            GoodId = goodId,
            Price = price
        };

        using var transaction = _anomaliesRepository.CreateTransactionScope();
        
        await _anomaliesRepository.SaveAnomaly(anomalyEntity, cancellationToken);
        
        transaction.Complete();
    }
}