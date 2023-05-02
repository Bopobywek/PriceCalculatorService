namespace Route256.Week5.Homework.PriceCalculator.Dal.Entities;

public record AnomalyEntityV1
{
    public long GoodId { get; init; } 
    
    public decimal Price { get; init; }   
}