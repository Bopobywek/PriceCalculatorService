using System.Threading;
using Moq;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Extensions;

public static class AnomalyServiceExtensions
{
    public static Mock<IAnomalyService> SetupSaveAnomaly(
        this Mock<IAnomalyService> service)
    {
        service.Setup(p =>
            p.SaveAnomaly(It.IsAny<long>(),
                It.IsAny<decimal>(),
                It.IsAny<CancellationToken>()));

        return service;
    }
    
    public static Mock<IAnomalyService> VerifySaveAnomalyWasCalledOnce(
        this Mock<IAnomalyService> service,
        long goodId, decimal price)
    {
        service.Verify(p =>
                p.SaveAnomaly(
                    It.Is<long>(x => x == goodId),
                    It.Is<decimal>(x => x == price),
                    It.IsAny<CancellationToken>()),
            Times.Once);
        
        return service;
    }
}