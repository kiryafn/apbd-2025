using ContainerShipment.Core.AbstractClasses;

namespace ContainerShipment.Domain;

public class LiquidContainer : HazardousContainer
{
    private readonly double _allowedMaxPayload;

    public bool IsCargoHazardous { get; }

    public LiquidContainer(double height, double depth, double tareWeight, double masPayload, bool isCargoHazardous)
        : base(height, depth, tareWeight, masPayload, 'L')
    {
        IsCargoHazardous = isCargoHazardous;
        var capacityLimit = IsCargoHazardous ? 0.5 : 0.9;
        _allowedMaxPayload = MaxPayload * capacityLimit;
    }

    protected override bool CanLoadCargo(double mass) => CargoMass + mass <= _allowedMaxPayload;
}