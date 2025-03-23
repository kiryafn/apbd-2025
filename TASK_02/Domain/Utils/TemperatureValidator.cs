using ContainerShipment.Domain.Enums;

namespace ContainerShipment.Services;

public static class TemperatureValidator
{
    public static bool IsValid(ProductType productType, double currentTemperature)
    {
        var allowedTemperature = GetTemperature(productType);
        return currentTemperature >= allowedTemperature;
    }

    public static double GetTemperature(ProductType productType)
    {
        var temperatrue = productType switch
        {
            ProductType.Bananas => 13.3,
            ProductType.Chocolate => 18.0,
            ProductType.MEat => 2.0,
            ProductType.Fish => -15.0,
            ProductType.IceCream => -18.0,
            ProductType.Cheese => -30.0,
            ProductType.FrozenPizza => 7.2,
            ProductType.Butter => 5.0,
            ProductType.Sausages => 20.5,
            ProductType.Eggs => 19.0,
            _ => throw new ArgumentException("Invalid prod type")
        };
        return temperatrue;
    }
}