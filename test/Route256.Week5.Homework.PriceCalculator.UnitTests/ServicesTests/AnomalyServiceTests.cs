using System.Threading.Tasks;
using System.Transactions;
using Route256.Week5.Homework.PriceCalculator.Dal.Entities;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Builders;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Extensions;
using Route256.Week5.Homework.TestingInfrastructure.Creators;
using Xunit;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.ServicesTests;

public class AnomalyServiceTests
{
    [Fact]
    public async Task SaveAnomaly_Success()
    {
        // Arrange
        var goodId = Create.RandomId();
        var price = Create.RandomDecimal();
        var entity = new AnomalyEntityV1
        {
            GoodId = goodId,
            Price = price
        };

        var builder = new AnomalyServiceBuilder();
        builder.AnomaliesRepository
            .SetupSaveAnomaly()
            .SetupCreateTransactionScope();

        var service = builder.Build();

        // Act
        await service.SaveAnomaly(goodId, price, default);

        // Assert
        service.AnomaliesRepository
            .VerifySaveAnomalyWasCalledOnce(entity)
            .VerifyCreateTransactionScopeWasCalledOnce(IsolationLevel.ReadCommitted);
        service.VerifyNoOtherCalls();
    }
}