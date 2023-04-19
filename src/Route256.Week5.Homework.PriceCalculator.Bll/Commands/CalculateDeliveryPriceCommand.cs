using MediatR;
using Route256.Week5.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week5.Homework.PriceCalculator.Bll.Commands;

public record CalculateDeliveryPriceCommand(
        long UserId,
        GoodModel[] Goods)
    : IRequest<CalculateDeliveryPriceResult>;

