using Moq;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;
using Route256.Week5.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Stubs;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Builders;

public class SaveAnomalyPriceHandlerBuilder
{
    public Mock<IAnomalyService> AnomalyService;
    
    public SaveAnomalyPriceHandlerBuilder()
    {
        AnomalyService = new Mock<IAnomalyService>();
    }
    
    public SaveAnomalyPriceCommandHandlerStub Build()
    {
        return new SaveAnomalyPriceCommandHandlerStub(AnomalyService);
    }
}