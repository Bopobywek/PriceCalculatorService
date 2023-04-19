using Moq;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;
using Route256.Week5.Homework.PriceCalculator.UnitTests.Stubs;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Builders;

public class ClearCalculationsHistoryCommandHandlerBuilder
{
    public Mock<ICalculationService> CalculationService;
    
    public ClearCalculationsHistoryCommandHandlerBuilder()
    {
        CalculationService = new Mock<ICalculationService>();
    }
    
    public ClearCalculationsHistoryCommandHandlerStub Build()
    {
        return new ClearCalculationsHistoryCommandHandlerStub(
            CalculationService);
    }
}