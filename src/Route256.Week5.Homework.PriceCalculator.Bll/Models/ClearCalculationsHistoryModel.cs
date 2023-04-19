namespace Route256.Week5.Homework.PriceCalculator.Bll.Models;

public record ClearCalculationsHistoryModel(
    long[] GoodsIds,
    long[] CalculationsIds);