using System.Text.Json;
using Route256.Week5.Homework.PriceCalculator.ProducerApp.Extensions;

namespace Route256.Week5.Homework.PriceCalculator.ProducerApp.NamingPolicies;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) =>
            name.ToSnakeCase();
}