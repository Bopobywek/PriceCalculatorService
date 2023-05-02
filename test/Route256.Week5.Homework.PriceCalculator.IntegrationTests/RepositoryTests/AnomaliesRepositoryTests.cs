using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Npgsql;
using Route256.Week5.Homework.PriceCalculator.Dal.Entities;
using Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week5.Homework.PriceCalculator.Dal.Settings;
using Route256.Week5.Homework.PriceCalculator.IntegrationTests.Fixtures;
using Route256.Week5.Homework.TestingInfrastructure.Creators;
using Xunit;

namespace Route256.Week5.Homework.PriceCalculator.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class AnomaliesRepositoryTests
{
    private readonly DalOptions _dalSettings;
    private readonly IAnomaliesRepository _anomaliesRepository;

    public AnomaliesRepositoryTests(TestFixture fixture)
    {
        _dalSettings = fixture.DalSettings;
        _anomaliesRepository = fixture.AnomaliesRepository;
    }
    
    [Fact]
    public async Task SaveAnomaly_Success()
    {
        // Arrange
        var goodId = Create.RandomId();
        var price = Create.RandomDecimal();

        const string sqlValidationQuery = @"select good_id, price from anomalies where good_id=@GoodId";

        var anomaly = new AnomalyEntityV1
        {
            GoodId = goodId,
            Price = price
        };
        
        var sqlQueryParams = new
        {
            anomaly.GoodId,
            anomaly.Price
        }; 
        
        // Act
        await _anomaliesRepository.SaveAnomaly(anomaly, default);
        
        await using var connection = new NpgsqlConnection(_dalSettings.ConnectionString);
        await connection.OpenAsync();
        connection.ReloadTypes();
        
        var result = await connection.QueryAsync<AnomalyQueryResult>(
            new CommandDefinition(
                sqlValidationQuery,
                sqlQueryParams,
                cancellationToken: default));

        var queryResult = result.ToArray();
        
        // Asserts
        queryResult.Should().NotBeNullOrEmpty();
        queryResult.Should().HaveCount(1);
    }

    private record AnomalyQueryResult
    {
        public long GoodId { get; init; }
    
        public decimal Price { get; init; }
    }
}