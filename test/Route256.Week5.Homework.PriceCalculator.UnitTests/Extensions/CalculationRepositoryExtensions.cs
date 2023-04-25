using System.Linq;
using System.Threading;
using System.Transactions;
using Moq;
using Moq.Language.Flow;
using Route256.Week5.Homework.PriceCalculator.Dal.Entities;
using Route256.Week5.Homework.PriceCalculator.Dal.Models;
using Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Comparers;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Extensions;

public static class CalculationRepositoryExtensions
{
    public static Mock<ICalculationRepository> SetupAddCalculations(
        this Mock<ICalculationRepository> repository,
        long[] ids)
    {
        repository.Setup(p =>
                p.Add(It.IsAny<CalculationEntityV1[]>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(ids);

        return repository;
    }
    
    public static Mock<ICalculationRepository> SetupCreateTransactionScope(
        this Mock<ICalculationRepository> repository)
    {
        repository.Setup(p =>
                p.CreateTransactionScope(It.IsAny<IsolationLevel>()))
            .Returns(new TransactionScope());

        return repository;
    }
    
    public static IReturnsResult<ICalculationRepository> SetupCreateTransactionScopeWithReturns(
        this Mock<ICalculationRepository> repository)
    {
        return repository.Setup(p =>
                p.CreateTransactionScope(It.IsAny<IsolationLevel>()))
            .Returns(() => new TransactionScope());
    }
    
    public static Mock<ICalculationRepository> SetupQueryCalculation(
        this Mock<ICalculationRepository> repository,
        CalculationEntityV1[] calculations)
    {
        repository.Setup(p =>
                p.Query(It.IsAny<CalculationHistoryQueryModel>(), 
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(calculations);

        return repository;
    }
    
    public static Mock<ICalculationRepository> SetupQueryCalculationWithIds(
        this Mock<ICalculationRepository> repository,
        CalculationEntityV1[] calculations)
    {
        repository.Setup(p =>
                p.Query(It.IsAny<long[]>(), 
                        It.IsAny<CancellationToken>()))
            .ReturnsAsync(calculations);

        return repository;
    }
    
    public static Mock<ICalculationRepository> SetupClearCalculationsWithUserId(
        this Mock<ICalculationRepository> repository)
    {
        repository.Setup(p =>
            p.ClearCalculations(It.IsAny<long>(),
                It.IsAny<CancellationToken>()));

        return repository;
    }
    
    public static Mock<ICalculationRepository> SetupClearCalculationsWithCalculationsIds(
        this Mock<ICalculationRepository> repository)
    {
        repository.Setup(p =>
            p.ClearCalculations(It.IsAny<long[]>(),
                It.IsAny<CancellationToken>()));

        return repository;
    }
    
    public static Mock<ICalculationRepository> VerifyClearCalculationsWasCalledOnce(
        this Mock<ICalculationRepository> repository,
        long userId)
    {
        repository.Verify(p =>
                p.ClearCalculations(
                    It.Is<long>(x => x == userId),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        return repository;
    }
    
    public static Mock<ICalculationRepository> VerifyClearCalculationsWasCalledOnce(
        this Mock<ICalculationRepository> repository,
        long[] calculationsIds)
    {
        repository.Verify(p =>
                p.ClearCalculations(
                    It.Is<long[]>(x => calculationsIds.SequenceEqual(x)),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        return repository;
    }

    public static Mock<ICalculationRepository> VerifyAddWasCalledOnce(
        this Mock<ICalculationRepository> repository,
        CalculationEntityV1[] calculations)
    {
        repository.Verify(p =>
                p.Add(
                    It.Is<CalculationEntityV1[]>(x => 
                        x.SequenceEqual(calculations, new CalculationEntityV1Comparer())),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        return repository;
    }
    
    public static Mock<ICalculationRepository> VerifyQueryWasCalledOnce(
        this Mock<ICalculationRepository> repository,
        CalculationHistoryQueryModel query)
    {
        repository.Verify(p =>
                p.Query(
                    It.Is<CalculationHistoryQueryModel>(x => x == query),
                    It.IsAny<CancellationToken>()),
            Times.Once);
        
        return repository;
    }
    
    public static Mock<ICalculationRepository> VerifyQueryWithIdsWasCalledOnce(
        this Mock<ICalculationRepository> repository,
        long[] calculationsIds)
    {
        repository.Verify(p =>
                p.Query(
                    It.Is<long[]>(x => calculationsIds.SequenceEqual(x)),
                    It.IsAny<CancellationToken>()),
            Times.Once);
        
        return repository;
    }
    
    public static Mock<ICalculationRepository> VerifyCreateTransactionScopeWasCalledOnce(
        this Mock<ICalculationRepository> repository,
        IsolationLevel isolationLevel)
    {
        repository.Verify(p =>
                p.CreateTransactionScope(
                    It.Is<IsolationLevel>(x => x == isolationLevel)),
            Times.Once);
        
        return repository;
    }
}