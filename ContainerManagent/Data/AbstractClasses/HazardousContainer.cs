using ContainerShipment.Core.Interfaces;
using ContainerShipment.Domain.Exceptions;

namespace ContainerShipment.Core.AbstractClasses;

public abstract class HazardousContainer : Container, IHazardNotifier
{
    protected HazardousContainer(double height, double depth, double tareWeight, double masPayload, char containerType)
        : base(height, depth, tareWeight, masPayload, containerType){}

    public override void LoadCargo(double mass)
    {
        if (mass <= 0) throw new ArgumentException();

        if (CanLoadCargo(mass)) 
            return;

        NotifyHazard("Cannot load cargo");
        throw new OverfillException("Cannot load cargo");
    }

    public void NotifyHazard(string message)
    {
        Console.WriteLine($"HazardousContainer {SerialNumber}" + $"\nMessage: {message}");
    }
}