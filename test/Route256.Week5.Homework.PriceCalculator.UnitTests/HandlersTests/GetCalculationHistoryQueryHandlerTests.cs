using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Builders;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Extensions;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Fakers;
using Route256.Week5.Homework.TestingInfrastructure.Creators;
using Xunit;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.HandlersTests;

public class GetCalculationHistoryQueryHandlerTests
{
    [Fact]
    public async Task Handle_MakeAllCalls()
    {
        //arrange
        var userId = Create.RandomId();

        var command = GetCalculationHistoryQueryFaker.Generate()
            .WithUserId(userId);

        var queryModels = QueryCalculationModelFaker.Generate(5)
            .Select(x => x.WithUserId(userId))
            .ToArray();

        var filter = QueryCalculationFilterFaker.Generate()
            .WithUserId(userId)
            .WithLimit(command.Take)
            .WithOffset(command.Skip);

        var builder = new GetCalculationHistoryHandlerBuilder();
        builder.CalculationService
            .SetupQueryCalculations(queryModels);

        var handler = builder.Build();

        //act
        var result = await handler.Handle(command, default);

        //asserts
        handler.CalculationService
            .VerifyQueryCalculationsWasCalledOnce(filter);

        handler.VerifyNoOtherCalls();

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(queryModels.Length);
        result.Items.Select(x => x.Price)
            .Should().IntersectWith(queryModels.Select(x => x.Price));
        result.Items.Select(x => x.Volume)
            .Should().IntersectWith(queryModels.Select(x => x.TotalVolume));
        result.Items.Select(x => x.Weight)
            .Should().IntersectWith(queryModels.Select(x => x.TotalWeight));
    }

    [Fact]
    public async Task Handle_CalculationIdsPassed_Success()
    {
        // Arrange
        var userId = Create.RandomId();

        var queryModels = QueryCalculationModelFaker.Generate(10)
            .Select(x => x.WithUserId(userId))
            .ToArray();

        var calculationIds = Enumerable.Range(0, 10)
            .Where(x => x % 3 == 0)
            .Select(x => queryModels[x].Id)
            .ToArray();

        var command = GetCalculationHistoryQueryFaker.Generate()
            .WithUserId(userId)
            .WithCalculationIds(calculationIds);

        var filter = QueryCalculationFilterFaker.Generate()
            .WithUserId(userId)
            .WithLimit(command.Take)
            .WithOffset(command.Skip);

        var builder = new GetCalculationHistoryHandlerBuilder();
        builder.CalculationService
            .SetupQueryCalculations(queryModels);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        handler.CalculationService
            .VerifyQueryCalculationsWasCalledOnce(filter);

        handler.VerifyNoOtherCalls();

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(calculationIds.Length);
    }
    
    [Fact]
    public async Task Handle_NotAllCalculationIdsWrong_Success()
    {
        // Arrange
        var userId = Create.RandomId();

        var queryModels = QueryCalculationModelFaker.Generate(10)
            .Select(x => x.WithUserId(userId))
            .ToArray();
        
        var wrongCalculationIds = Enumerable.Range(0, 2)
            .Select(_ => Create.RandomId())
            .Except(queryModels.Select(x => x.Id))
            .ToArray();
        
        var existingCalculationIds = Enumerable.Range(0, 10)
            .Where(x => x % 3 == 0)
            .Select(x => queryModels[x].Id)
            .ToArray();

        var calculationIds = wrongCalculationIds.Union(existingCalculationIds).ToArray();
        
        var command = GetCalculationHistoryQueryFaker.Generate()
            .WithUserId(userId)
            .WithCalculationIds(calculationIds);

        var filter = QueryCalculationFilterFaker.Generate()
            .WithUserId(userId)
            .WithLimit(command.Take)
            .WithOffset(command.Skip);

        var builder = new GetCalculationHistoryHandlerBuilder();
        builder.CalculationService
            .SetupQueryCalculations(queryModels);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        handler.CalculationService
            .VerifyQueryCalculationsWasCalledOnce(filter);

        handler.VerifyNoOtherCalls();

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(existingCalculationIds.Length);
    }
    
    [Fact]
    public async Task Handle_WrongCalculationIdsPassed_ShouldReturnEmptyArray()
    {
        // Arrange
        var userId = Create.RandomId();

        var queryModels = QueryCalculationModelFaker.Generate(10)
            .Select(x => x.WithUserId(userId))
            .ToArray();
        
        var wrongCalculationIds = Enumerable.Range(0, 10)
            .Select(_ => Create.RandomId())
            .Except(queryModels.Select(x => x.Id))
            .ToArray();

        var command = GetCalculationHistoryQueryFaker.Generate()
            .WithUserId(userId)
            .WithCalculationIds(wrongCalculationIds);

        var filter = QueryCalculationFilterFaker.Generate()
            .WithUserId(userId)
            .WithLimit(command.Take)
            .WithOffset(command.Skip);

        var builder = new GetCalculationHistoryHandlerBuilder();
        builder.CalculationService
            .SetupQueryCalculations(queryModels);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        handler.CalculationService
            .VerifyQueryCalculationsWasCalledOnce(filter);

        handler.VerifyNoOtherCalls();

        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }
}