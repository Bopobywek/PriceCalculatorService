using Moq;
using Route256.Week5.Homework.PriceCalculator.Bll.Services;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;
using Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Stubs;

public class AnomalyServiceStub : AnomalyService
{
    public readonly Mock<IAnomaliesRepository> AnomaliesRepository;

    public AnomalyServiceStub(Mock<IAnomaliesRepository> anomaliesRepository) : base(anomaliesRepository.Object)
    {
        AnomaliesRepository = anomaliesRepository;
    }

    public void VerifyNoOtherCalls()
    {
        AnomaliesRepository.VerifyNoOtherCalls();
    }
}