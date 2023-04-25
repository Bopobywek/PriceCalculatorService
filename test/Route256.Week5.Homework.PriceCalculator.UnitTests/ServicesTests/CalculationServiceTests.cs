using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using FluentAssertions;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;
using Route256.Week5.Homework.PriceCalculator.Bll.Services;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Builders;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Extensions;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Fakers;
using Route256.Week5.Homework.TestingInfrastructure.Creators;
using Route256.Week5.Homework.TestingInfrastructure.Fakers;
using Xunit;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.ServicesTests;

public class CalculationServiceTests
{
    [Fact]
    public async Task SaveCalculation_Success()
    {
        // arrange
        const int goodsCount = 5;

        var userId = Create.RandomId();
        var calculationId = Create.RandomId();

        var goodModels = GoodModelFaker.Generate(goodsCount)
            .ToArray();

        var goods = goodModels
            .Select(x => GoodEntityV1Faker.Generate().Single()
                .WithUserId(userId)
                .WithHeight(x.Height)
                .WithWidth(x.Width)
                .WithLength(x.Length)
                .WithWeight(x.Weight))
            .ToArray();
        var goodIds = goods.Select(x => x.Id)
            .ToArray();

        var calculationModel = SaveCalculationModelFaker.Generate()
            .Single()
            .WithUserId(userId)
            .WithGoods(goodModels);

        var calculations = CalculationEntityV1Faker.Generate()
            .Select(x => x
                .WithId(calculationId)
                .WithUserId(userId)
                .WithPrice(calculationModel.Price)
                .WithTotalWeight(calculationModel.TotalWeight)
                .WithTotalVolume(calculationModel.TotalVolume))
            .ToArray();

        var builder = new CalculationServiceBuilder();
        builder.CalculationRepository
            .SetupAddCalculations(new[] {calculationId})
            .SetupCreateTransactionScope();
        builder.GoodsRepository
            .SetupAddGoods(goodIds);

        var service = builder.Build();

        // act
        var result = await service.SaveCalculation(calculationModel, default);

        // assert
        result.Should().Be(calculationId);
        service.CalculationRepository
            .VerifyAddWasCalledOnce(calculations)
            .VerifyCreateTransactionScopeWasCalledOnce(IsolationLevel.ReadCommitted);
        service.GoodsRepository
            .VerifyAddWasCalledOnce(goods);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public void CalculatePriceByVolume_Success()
    {
        // arrange
        var goodModels = GoodModelFaker.Generate(5)
            .ToArray();

        var builder = new CalculationServiceBuilder();
        var service = builder.Build();

        //act
        var price = service.CalculatePriceByVolume(goodModels, out var volume);

        //asserts
        volume.Should().BeApproximately(goodModels.Sum(x => x.Height * x.Width * x.Length), 1e-9d);
        price.Should().Be((decimal) volume * CalculationService.VolumeToPriceRatio);
    }

    [Fact]
    public void CalculatePriceByWeight_Success()
    {
        // arrange
        var goodModels = GoodModelFaker.Generate(5)
            .ToArray();

        var builder = new CalculationServiceBuilder();
        var service = builder.Build();

        //act
        var price = service.CalculatePriceByWeight(goodModels, out var weight);

        //asserts
        weight.Should().Be(goodModels.Sum(x => x.Weight));
        price.Should().Be((decimal) weight * CalculationService.WeightToPriceRatio);
    }

    [Fact]
    public async Task QueryCalculations_Success()
    {
        // arrange
        var userId = Create.RandomId();

        var filter = QueryCalculationFilterFaker.Generate()
            .WithUserId(userId);

        var calculations = CalculationEntityV1Faker.Generate(5)
            .Select(x => x.WithUserId(userId))
            .ToArray();

        var queryModel = CalculationHistoryQueryModelFaker.Generate()
            .WithUserId(userId)
            .WithLimit(filter.Limit)
            .WithOffset(filter.Offset);

        var builder = new CalculationServiceBuilder();
        builder.CalculationRepository
            .SetupQueryCalculation(calculations);
        var service = builder.Build();

        //act
        var result = await service.QueryCalculations(filter, default);

        //asserts
        service.CalculationRepository
            .VerifyQueryWasCalledOnce(queryModel);

        service.VerifyNoOtherCalls();

        result.Should().NotBeEmpty();
        result.Should().OnlyContain(x => x.UserId == userId);
        result.Should().OnlyContain(x => x.Id > 0);
        result.Select(x => x.TotalWeight)
            .Should().IntersectWith(calculations.Select(x => x.TotalWeight));
        result.Select(x => x.TotalVolume)
            .Should().IntersectWith(calculations.Select(x => x.TotalVolume));
        result.Select(x => x.Price)
            .Should().IntersectWith(calculations.Select(x => x.Price));
    }

    [Fact]
    public async Task QueryCalculations_WithCalculationsIds_Success()
    {
        // Arrange
        var userId = Create.RandomId();

        var calculations = CalculationEntityV1Faker.Generate(5)
            .Select(x => x.WithUserId(userId))
            .ToArray();
        var calculationsIds = calculations.Select(x => x.Id).ToArray();

        var builder = new CalculationServiceBuilder();
        builder.CalculationRepository
            .SetupQueryCalculationWithIds(calculations);
        var service = builder.Build();

        // Act
        var result = await service.QueryCalculations(calculationsIds, default);

        // Assert
        service.CalculationRepository.VerifyQueryWithIdsWasCalledOnce(calculationsIds);
        service.VerifyNoOtherCalls();

        result.Should().HaveCount(5);
        result.Should().OnlyContain(x => x.UserId == userId);
        result.Should().OnlyContain(x => x.Id > 0);
        result.Select(x => x.TotalWeight)
            .Should().IntersectWith(calculations.Select(x => x.TotalWeight));
        result.Select(x => x.TotalVolume)
            .Should().IntersectWith(calculations.Select(x => x.TotalVolume));
        result.Select(x => x.Price)
            .Should().IntersectWith(calculations.Select(x => x.Price));
    }

    [Fact]
    public async Task ClearCalculationsHistory_WithUserId_Success()
    {
        // Arrange
        var userId = Create.RandomId();
        int transactionScopeCreatedCount = 0;

        var builder = new CalculationServiceBuilder();
        builder.CalculationRepository
            .SetupClearCalculationsWithUserId()
            .SetupCreateTransactionScopeWithReturns()
            .Callback(() => Interlocked.Increment(ref transactionScopeCreatedCount));
        builder.GoodsRepository
            .SetupClearGoodsWithUserId()
            .SetupCreateTransactionScopeWithReturns()
            .Callback(() => Interlocked.Increment(ref transactionScopeCreatedCount));
        
        var service = builder.Build();
        
        // Act
        await service.ClearCalculationsHistory(userId, default);
        
        // Assert
        service.CalculationRepository
            .VerifyClearCalculationsWasCalledOnce(userId);

        service.GoodsRepository
            .VerifyClearGoodsWasCalledOnce(userId);

        transactionScopeCreatedCount.Should().Be(1);
    }

    [Fact]
    public async Task ClearCalculationsHistory_WithModel_Success()
    {
        // Arrange
        int transactionScopeCreatedCount = 0;
        var userId = Create.RandomId();
        
        var calculations = CalculationEntityV1Faker.Generate(5)
            .Select(x => x.WithUserId(userId))
            .ToArray();
        var calculationsIds = calculations.Select(x => x.Id).ToArray();
        
        var goods = GoodEntityV1Faker.Generate(5)
            .Select(x => x.WithUserId(userId))
            .ToArray();
        var goodsIds = goods.Select(x => x.Id).ToArray();

        var clearCalculationsHistoryModel = new ClearCalculationsHistoryModel(goodsIds, calculationsIds);

        var builder = new CalculationServiceBuilder();
        builder.CalculationRepository
            .SetupClearCalculationsWithCalculationsIds()
            .SetupCreateTransactionScopeWithReturns()
            .Callback(() => Interlocked.Increment(ref transactionScopeCreatedCount));
        builder.GoodsRepository
            .SetupClearGoodsWithGoodsIds()
            .SetupCreateTransactionScopeWithReturns()
            .Callback(() => Interlocked.Increment(ref transactionScopeCreatedCount));
        
        var service = builder.Build();
        
        // Act
        await service.ClearCalculationsHistory(clearCalculationsHistoryModel, default);
        
        // Assert
        service.CalculationRepository
            .VerifyClearCalculationsWasCalledOnce(calculationsIds);

        service.GoodsRepository
            .VerifyClearGoodsWasCalledOnce(goodsIds);

        transactionScopeCreatedCount.Should().Be(1);
    }
}