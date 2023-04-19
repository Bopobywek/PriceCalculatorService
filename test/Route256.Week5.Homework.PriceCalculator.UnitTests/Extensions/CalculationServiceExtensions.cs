using System.Linq;
using System.Threading;
using Moq;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Comparers;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Extensions;

public static class CalculationServiceExtensions
{
    public static Mock<ICalculationService> SetupSaveCalculation(
        this Mock<ICalculationService> service,
        long id)
    {
        service.Setup(p =>
                p.SaveCalculation(It.IsAny<SaveCalculationModel>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(id);

        return service;
    }

    public static Mock<ICalculationService> SetupCalculatePriceByVolume(
        this Mock<ICalculationService> service,
        double volume,
        decimal price)
    {
        service.Setup(p =>
                p.CalculatePriceByVolume(It.IsAny<GoodModel[]>(),
                    out volume))
            .Returns(price);

        return service;
    }

    public static Mock<ICalculationService> SetupCalculatePriceByWeight(
        this Mock<ICalculationService> service,
        double weight,
        decimal price)
    {
        service.Setup(p =>
                p.CalculatePriceByWeight(It.IsAny<GoodModel[]>(),
                    out weight))
            .Returns(price);

        return service;
    }

    public static Mock<ICalculationService> SetupQueryCalculations(
        this Mock<ICalculationService> service,
        QueryCalculationModel[] model)
    {
        service.Setup(p =>
                p.QueryCalculations(It.IsAny<QueryCalculationFilter>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);

        return service;
    }

    public static Mock<ICalculationService> SetupQueryCalculationsWithIds(
        this Mock<ICalculationService> service,
        QueryCalculationModel[] model)
    {
        service.Setup(p =>
                p.QueryCalculations(It.IsAny<long[]>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(model);

        return service;
    }

    public static Mock<ICalculationService> SetupClearCalculationsHistoryWithUserId(
        this Mock<ICalculationService> service)
    {
        service.Setup(p =>
            p.ClearCalculationsHistory(It.IsAny<long>(),
                It.IsAny<CancellationToken>()));

        return service;
    }

    public static Mock<ICalculationService> SetupClearCalculationsHistoryWithQueryModel(
        this Mock<ICalculationService> service)
    {
        service.Setup(p =>
            p.ClearCalculationsHistory(It.IsAny<ClearCalculationsHistoryModel>(),
                It.IsAny<CancellationToken>()));

        return service;
    }

    public static Mock<ICalculationService> VerifySaveCalculationWasCalledOnce(
        this Mock<ICalculationService> service,
        SaveCalculationModel model)
    {
        service.Verify(p =>
                p.SaveCalculation(
                    It.Is<SaveCalculationModel>(x => new CalculationModelComparer().Equals(x, model)),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        return service;
    }

    public static Mock<ICalculationService> VerifyCalculatePriceByVolumeWasCalledOnce(
        this Mock<ICalculationService> service,
        GoodModel[] model)
    {
        service.Verify(p =>
                p.CalculatePriceByVolume(
                    It.Is<GoodModel[]>(x => x.SequenceEqual(model)),
                    out It.Ref<double>.IsAny),
            Times.Once);

        return service;
    }

    public static Mock<ICalculationService> VerifyCalculatePriceByWeightWasCalledOnce(
        this Mock<ICalculationService> service,
        GoodModel[] model)
    {
        service.Verify(p =>
                p.CalculatePriceByWeight(
                    It.Is<GoodModel[]>(x => x.SequenceEqual(model)),
                    out It.Ref<double>.IsAny),
            Times.Once);

        return service;
    }

    public static Mock<ICalculationService> VerifyQueryCalculationsWasCalledOnce(
        this Mock<ICalculationService> service,
        QueryCalculationFilter filter)
    {
        service.Verify(p =>
                p.QueryCalculations(
                    It.Is<QueryCalculationFilter>(x => x == filter),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        return service;
    }

    public static Mock<ICalculationService> VerifyQueryCalculationsWithIdsWasCalledOnce(
        this Mock<ICalculationService> service,
        long[] calculationsIds)
    {
        service.Verify(p =>
                p.QueryCalculations(
                    It.Is<long[]>(x => calculationsIds.SequenceEqual(x)),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        return service;
    }

    public static Mock<ICalculationService> VerifyClearCalculationsHistoryWithUserIdWasCalledOnce(
        this Mock<ICalculationService> service,
        long userId)
    {
        service.Verify(p =>
                p.ClearCalculationsHistory(
                    It.Is<long>(x => x == userId),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        return service;
    }

    public static Mock<ICalculationService> VerifyClearCalculationsHistoryWithQueryModelWasCalledOnce(
        this Mock<ICalculationService> service,
        ClearCalculationsHistoryModel model)
    {
        service.Verify(p =>
                p.ClearCalculationsHistory(
                    It.Is<ClearCalculationsHistoryModel>(x =>
                        model.CalculationsIds.SequenceEqual(x.CalculationsIds) &&
                        model.GoodsIds.SequenceEqual(x.GoodsIds)),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        return service;
    }
}