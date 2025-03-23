using System.Text;
using ContainerShipment.Domain.Exceptions;

namespace ContainerShipment.Core.AbstractClasses;

public abstract class Container
{
    private const string ContainerPrefix = "KON";
    private static int _nextSerialNumber = 1;

    public double Height { get; set; }
    public double Depth { get; set; }
    public double TareWeight { get; set; }
    public double MaxPayload { get; set; }
    public string SerialNumber { get; set; }
    public double CargoMass { get; set; }

    protected Container(double height, double depth, double tareWeight, double masPayload, char containerType)
    {
        Height = height > 0 ? height : throw new ArgumentException("Height must be greater than zero");
        Depth = depth > 0 ? depth : throw new ArgumentException("Depth must be greater than zero");
        TareWeight = tareWeight > 0 ? tareWeight : throw new ArgumentException("TareWeight must be greater than zero");
        MaxPayload = masPayload > 0 ? masPayload : throw new ArgumentException("MaxPayload must be greater than zero");
        SerialNumber = $"{ContainerPrefix}-{containerType}-{_nextSerialNumber++}";
    }

    public virtual void LoadCargo(double mass)
    {
        if (mass <= 0) throw new ArgumentException("Mass must be greater than zero");

        if (CargoMass + mass > MaxPayload) throw new OverfillException("Cargo Mass must be less than MaxPayload");

        CargoMass += mass;
    }

    public virtual void Unload() => CargoMass = 0;

    protected virtual bool CanLoadCargo(double mass) => CargoMass + mass > MaxPayload;

    public virtual double GetCompleteWeight() => CargoMass + TareWeight;

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append($"Container {SerialNumber}\n");
        builder.Append(
            $"height: {Height}, depth: {Depth}, tare weight: {TareWeight}, maximum payload: {MaxPayload}, cargo mass: {CargoMass}\n");

        return builder.ToString();
    }
}