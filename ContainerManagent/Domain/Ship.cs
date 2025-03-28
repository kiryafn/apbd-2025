using System;
using System.Collections.Generic;
using System.Linq;
using ContainerShipment.Core.AbstractClasses;

namespace ContainerShipApp
{
    public class Ship
    {
        public string Name { get; }
        public double MaxSpeed { get; }
        public int MaxContainerCount { get; }
        public double MaxWeight { get; }
        private readonly List<Container> _containers;

        public Ship(string name, double maxSpeed, int maxContainerCount, double maxWeight)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("The name of the ship cannot be empty.");
            MaxSpeed = maxSpeed > 0 ? maxSpeed : throw new ArgumentException("The speed of the ship should be more than 0.");
            MaxContainerCount = maxContainerCount > 0 ? maxContainerCount : throw new ArgumentException("The maximum number of containers should be more than 0.");
            MaxWeight = maxWeight > 0 ? maxWeight : throw new ArgumentException("The maximum weight should be more than 0.");
            _containers = new List<Container>();
        }
        

        public double CurrentWeight => _containers.Sum(c => c.CargoMass + c.TareWeight);
        public int CurrentContainerCount => _containers.Count;
        
        public void LoadContainer(Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container), "The container cannot be null.");

            if (_containers.Count >= MaxContainerCount)
                throw new InvalidOperationException("It is impossible to load the container: the limit of the number of containers on the ship is exceeded.");

            if (CurrentWeight + container.CargoMass + container.TareWeight > MaxWeight)
                throw new InvalidOperationException("It is impossible to load the container: the maximum permissible weight is exceeded.");

            _containers.Add(container);
        }

        public Container UnloadContainer(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                throw new ArgumentException("The serial number cannot be empty.", nameof(serialNumber));

            var container = _containers.FirstOrDefault(c => c.SerialNumber == serialNumber);

            if (container == null)
                throw new InvalidOperationException("The container with the specified serial number was not found.");

            _containers.Remove(container);
            return container;
        }
        

        public override string ToString()
        {
            return $"Ship: {Name}\n" +
                   $"Speed: {MaxSpeed}\n" +
                   $"The maximum number of containers: {MaxContainerCount}\n" +
                   $"Weight Limit: {MaxWeight} tons\n" +
                   $"Current weight: {CurrentWeight} tons\n" +
                   $"Containers on board: {CurrentContainerCount}\n" +
                   "List of containers:\n" +
                   string.Join("\n", _containers.Select(c => $"- {c.SerialNumber} (The weight of the cargo: {c.CargoMass} tons.)"));
        }
    }
}