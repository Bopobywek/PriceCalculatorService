using Moq;
using Route256.Week5.Homework.PriceCalculator.Bll.Handlers;
using Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;

namespace Route256.Week5.Homework.PriceCalculator.UnitTests.Stubs;

public class SaveAnomalyPriceCommandHandlerStub : SaveAnomalyPriceCommandHandler
{
    public Mock<IAnomalyService> AnomalyService { get; }

    public SaveAnomalyPriceCommandHandlerStub(Mock<IAnomalyService> anomalyService) : base(anomalyService.Object)
    {
        AnomalyService = anomalyService;
    }
    
    
}