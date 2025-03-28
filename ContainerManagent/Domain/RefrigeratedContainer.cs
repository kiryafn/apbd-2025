using ContainerShipment.Core.AbstractClasses;
using ContainerShipment.Domain.Enums;
using ContainerShipment.Services;

namespace ContainerShipment.Domain;

public class RefrigeratedContainer : Container
{
    public ProductType ProductType { get; }
    public double MaintainedTemperature { get; }

    public RefrigeratedContainer(double height, double depth, double tareWeight, double masPayload, ProductType productType, double maintainedTemperature)
        : base(height, depth, tareWeight, masPayload, 'C')
    {
        if (!TemperatureValidator.IsValid(productType, maintainedTemperature)) 
            return;

        ProductType = productType;
        MaintainedTemperature = maintainedTemperature;
    }
}