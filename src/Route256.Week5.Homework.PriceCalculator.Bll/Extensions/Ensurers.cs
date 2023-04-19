using Route256.Week5.Homework.PriceCalculator.Bll.Commands;
using Route256.Week5.Homework.PriceCalculator.Bll.Exceptions;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Extensions;

public static class Ensurers
{
    public static CalculateDeliveryPriceCommand EnsureHasGoods(
        this CalculateDeliveryPriceCommand src)
    {
        if (!src.Goods.Any())
        {
            throw new GoodsNotFoundException();
        }

        return src;
    }

    public static void EnsureAllCalculationsFound(this ClearCalculationsHistoryCommand request,
        QueryCalculationModel[] result)
    {
        if (request.CalculationIds.Length != result.Length)
        {
            throw new OneOrManyCalculationsNotFoundException();
        }
    }

    public static void EnsureAllCalculationsBelongsToOneUser(this ClearCalculationsHistoryCommand request,
        QueryCalculationModel[] result)
    {
        var wrongCalculationIds = result
            .Where(x => x.UserId != request.UserId)
            .Select(x => x.Id)
            .ToArray();

        if (wrongCalculationIds.Length != 0)
        {
            throw new OneOrManyCalculationsBelongsToAnotherUserException
            {
                Data =
                {
                    {"wrongCalculationIds", wrongCalculationIds}
                }
            };
        }
    }
}