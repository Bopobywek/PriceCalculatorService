using Dapper;
using Microsoft.Extensions.Options;
using Route256.Week5.Homework.PriceCalculator.Dal.Entities;
using Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week5.Homework.PriceCalculator.Dal.Settings;

namespace Route256.Week5.Homework.PriceCalculator.Dal.Repositories;

class AnomaliesRepository : BaseRepository, IAnomaliesRepository
{
    public AnomaliesRepository(IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }
    
    public async Task SaveAnomaly(AnomalyEntityV1 entity, CancellationToken token)
    {
        const string sqlQuery = @"
insert into anomalies (good_id, price)
values (@GoodId, @Price)
";
        
        var sqlQueryParams = new
        {
            entity.GoodId,
            entity.Price
        };
        
        await using var connection = await GetAndOpenConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                sqlQueryParams,
                cancellationToken: token));
    }
}