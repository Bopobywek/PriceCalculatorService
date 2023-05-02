using Moq;
using Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Stubs;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Builders;

public class AnomalyServiceBuilder
{
    public Mock<IAnomaliesRepository> AnomaliesRepository;
    
    public AnomalyServiceBuilder()
    {
        AnomaliesRepository = new Mock<IAnomaliesRepository>();
    }
    
    public AnomalyServiceStub Build()
    {
        return new AnomalyServiceStub(AnomaliesRepository);
    }
}