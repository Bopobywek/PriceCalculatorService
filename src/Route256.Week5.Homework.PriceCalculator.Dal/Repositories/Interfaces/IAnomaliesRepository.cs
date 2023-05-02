using Route256.Week5.Homework.PriceCalculator.Dal.Entities;
using Route256.Week5.Homework.PriceCalculator.Dal.Settings;

namespace Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;

public interface IAnomaliesRepository : IDbRepository
{
    Task SaveAnomaly(AnomalyEntityV1 entity, 
        CancellationToken token);
}
