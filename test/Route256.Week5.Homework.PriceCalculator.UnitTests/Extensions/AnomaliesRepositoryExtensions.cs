using System.Threading;
using System.Transactions;
using Moq;
using Route256.Week5.Homework.PriceCalculator.Dal.Entities;
using Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Extensions;

public static class AnomaliesRepositoryExtensions
{
    public static Mock<IAnomaliesRepository> SetupSaveAnomaly(
        this Mock<IAnomaliesRepository> repository)
    {
        repository.Setup(p =>
            p.SaveAnomaly(It.IsAny<AnomalyEntityV1>(),
                It.IsAny<CancellationToken>()));

        return repository;
    }

    public static Mock<IAnomaliesRepository> SetupCreateTransactionScope(
        this Mock<IAnomaliesRepository> repository)
    {
        repository.Setup(p =>
                p.CreateTransactionScope(It.IsAny<IsolationLevel>()))
            .Returns(new TransactionScope());

        return repository;
    }

    public static Mock<IAnomaliesRepository> VerifySaveAnomalyWasCalledOnce(
        this Mock<IAnomaliesRepository> repository, AnomalyEntityV1 entity)
    {
        repository.Verify(p =>
                p.SaveAnomaly(
                    It.Is<AnomalyEntityV1>(x => x.GoodId == entity.GoodId && x.Price == entity.Price),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        return repository;
    }

    public static Mock<IAnomaliesRepository> VerifyCreateTransactionScopeWasCalledOnce(
        this Mock<IAnomaliesRepository> repository,
        IsolationLevel isolationLevel)
    {
        repository.Verify(p =>
                p.CreateTransactionScope(
                    It.Is<IsolationLevel>(x => x == isolationLevel)),
            Times.Once);

        return repository;
    }
}