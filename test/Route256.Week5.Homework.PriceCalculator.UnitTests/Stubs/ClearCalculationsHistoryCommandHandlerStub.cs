using Moq;
using Route256.Week5.Homework.PriceCalculator.Bll.Handlers;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Stubs;

public class ClearCalculationsHistoryCommandHandlerStub : ClearCalculationsHistoryCommandHandler
{
    public Mock<ICalculationService> CalculationService { get; }
    
    public ClearCalculationsHistoryCommandHandlerStub(
        Mock<ICalculationService> calculationService) 
        : base(
            calculationService.Object)
    {
        CalculationService = calculationService;
    }
}