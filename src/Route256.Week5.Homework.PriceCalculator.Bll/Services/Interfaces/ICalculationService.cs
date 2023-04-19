using Route256.Week5.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Services.Interfaces;

public interface ICalculationService
{
    Task<long> SaveCalculation(
        SaveCalculationModel saveCalculation,
        CancellationToken cancellationToken);

    decimal CalculatePriceByVolume(
        GoodModel[] goods,
        out double volume);

    public decimal CalculatePriceByWeight(
        GoodModel[] goods,
        out double weight);

    Task<QueryCalculationModel[]> QueryCalculations(
        QueryCalculationFilter query,
        CancellationToken token);

    Task<QueryCalculationModel[]> QueryCalculations(
        long[] calculationsIds,
        CancellationToken token);

    Task ClearCalculationsHistory(
        ClearCalculationsHistoryModel data,
        CancellationToken token);

    Task ClearCalculationsHistory(
        long userId,
        CancellationToken token);
}