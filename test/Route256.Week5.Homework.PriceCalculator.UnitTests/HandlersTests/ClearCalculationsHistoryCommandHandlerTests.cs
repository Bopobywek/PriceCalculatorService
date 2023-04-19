using System;
using System.Linq;
using System.Threading.Tasks;
using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Exceptions;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Builders;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Extensions;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Fakers;
using Route256.Week5.Homework.TestingInfrastructure.Creators;
using Xunit;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.HandlersTests;

public class ClearCalculationsHistoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_MakeAllCalls_WhenCalculationsIdsArrayIsNotEmpty()
    {
        // Arrange
        var userId = Create.RandomId();
        var calculationsQueryModels = QueryCalculationModelFaker
            .Generate(10)
            .WithUserId(userId);
        
        var calculationsIds = calculationsQueryModels.Select(x => x.Id).ToArray();
        
        var goodsIds = calculationsQueryModels
            .Select(x => x.GoodIds)
            .SelectMany(x => x)
            .Distinct()
            .ToArray();
        
        var queryModel = new ClearCalculationsHistoryModel(goodsIds, calculationsIds);
        var command = new ClearCalculationsHistoryCommand(userId, calculationsIds);

        var builder = new ClearCalculationsHistoryCommandHandlerBuilder();

        builder
            .CalculationService
            .SetupQueryCalculationsWithIds(calculationsQueryModels)
            .SetupClearCalculationsHistoryWithQueryModel();

        var handler = builder.Build();

        // Act
        await handler.Handle(command, default);

        // Assert
        builder.CalculationService
            .VerifyQueryCalculationsWithIdsWasCalledOnce(calculationsIds)
            .VerifyClearCalculationsHistoryWithQueryModelWasCalledOnce(queryModel)
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_MakeAllCalls_WhenCalculationsIdsArrayIsEmpty()
    {
        // Arrange
        var userId = Create.RandomId();
        var calculationsIds = Array.Empty<long>();
        var command = new ClearCalculationsHistoryCommand(userId, calculationsIds);

        var builder = new ClearCalculationsHistoryCommandHandlerBuilder();

        builder
            .CalculationService
            .SetupClearCalculationsHistoryWithUserId();

        var handler = builder.Build();

        // Act
        await handler.Handle(command, default);

        // Assert
        builder.CalculationService
            .VerifyClearCalculationsHistoryWithUserIdWasCalledOnce(userId)
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenOneOrManyCalculationsBelongsToAnotherUser_ShouldThrow()
    {
        // Arrange
        var userId = Create.RandomId();
        var anotherUserId = Create.RandomId();

        var calculationsQueryModels = QueryCalculationModelFaker
            .Generate(10)
            .WithUserId(userId);

        calculationsQueryModels[2] = calculationsQueryModels[2].WithUserId(anotherUserId);
        calculationsQueryModels[5] = calculationsQueryModels[5].WithUserId(anotherUserId);

        var calculationsIds = calculationsQueryModels.Select(x => x.Id).ToArray();
        
        var command = new ClearCalculationsHistoryCommand(userId, calculationsIds);
        var builder = new ClearCalculationsHistoryCommandHandlerBuilder();

        builder
            .CalculationService
            .SetupQueryCalculationsWithIds(calculationsQueryModels)
            .SetupClearCalculationsHistoryWithUserId();

        var handler = builder.Build();

        // Act, Assert
        await Assert.ThrowsAsync<OneOrManyCalculationsBelongsToAnotherUserException>(() =>
            handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_WhenOneOrManyCalculationsNotFound_ShouldThrow()
    {
        // Arrange
        var userId = Create.RandomId();

        var calculationsQueryModels = QueryCalculationModelFaker
            .Generate(10)
            .WithUserId(userId);

        var calculationsIds = calculationsQueryModels.Select(x => x.Id).ToArray();

        calculationsQueryModels = calculationsQueryModels.Skip(1).SkipLast(2).ToArray();
        
        var command = new ClearCalculationsHistoryCommand(userId, calculationsIds);
        var builder = new ClearCalculationsHistoryCommandHandlerBuilder();

        builder
            .CalculationService
            .SetupQueryCalculationsWithIds(calculationsQueryModels)
            .SetupClearCalculationsHistoryWithUserId();

        var handler = builder.Build();

        // Act, Assert
        await Assert.ThrowsAsync<OneOrManyCalculationsNotFoundException>(() =>
            handler.Handle(command, default));
    }
}