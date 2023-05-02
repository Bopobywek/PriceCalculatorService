using System.Threading.Tasks;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Builders;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Extensions;
using Route256.Week5.Homework.TestingInfrastructure.Creators;
using Xunit;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.HandlersTests;

public class SaveAnomalyPriceCommandHandlerTests
{
    [Fact]
    public async Task Handle_MakeAllCalls()
    {
        // Arrange
        var goodId = Create.RandomId();
        var price = Create.RandomDecimal();
        var command = new SaveAnomalyPriceCommand(goodId, price);

        var builder = new SaveAnomalyPriceHandlerBuilder();
        builder.AnomalyService
            .SetupSaveAnomaly();

        var handler = builder.Build();

        // Act
        await handler.Handle(command, default);

        // Assert
        handler.AnomalyService
            .VerifySaveAnomalyWasCalledOnce(goodId, price)
            .VerifyNoOtherCalls();
    }
}