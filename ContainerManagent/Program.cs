using ContainerShipment.Core.AbstractClasses;
using ContainerShipment.Domain;
using ContainerShipment.Domain.Enums;
using ContainerShipment.Services;

namespace ContainerShipApp
{
    public class Program
    {
        private static readonly List<Ship> Ships = new();
        private static readonly List<Container> Containers = new();

        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\n");
                PrintMainMenu();
                
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddShip();
                        break;
                    case "2":
                        RemoveShip();
                        break;
                    case "3":
                        AddContainer();
                        break;
                    case "4":
                        PlaceContainerOnShip();
                        break;
                    case "5":
                        RemoveContainerFromShip();
                        break;
                    case "6":
                        PrintShipDetails();
                        break;
                    case "7":
                        PrintContainerDetails();
                        break;
                    case "0":
                        Console.WriteLine("Выход из программы.");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                        break;
                }
            }
        }

        private static void PrintMainMenu()
        {
            Console.WriteLine("=== Container ship management ===");
            Console.WriteLine("List of ships:");
            foreach (var ship in Ships)
                Console.WriteLine($"- {ship.Name} (Speed={ship.MaxSpeed}, Max. containers={ship.MaxContainerCount}, Max. weight={ship.MaxWeight})");

            Console.WriteLine("List of containers:");
            foreach (var container in Containers)
                Console.WriteLine($"- {container.SerialNumber}");
            
            Console.WriteLine("Available actions:");
            Console.WriteLine("1. Add a container ship");
            Console.WriteLine("2. Delete a container ship");
            Console.WriteLine("3. Add the container");
            Console.WriteLine("4. Put a container on a ship");
            Console.WriteLine("5. Delete the container from the ship");
            Console.WriteLine("6. Information about the ship");
            Console.WriteLine("7. Information about the container");
            Console.WriteLine("0. Exit");
            Console.Write("Choose the action:");
        }

        private static void AddShip()
        {
            Console.Write("Enter the name of the ship:");
            var name = Console.ReadLine();

            Console.Write("Enter the maximum speed (nodes):");
            var maxSpeed = double.Parse(Console.ReadLine()!);

            Console.Write("Enter the maximum number of containers: ");
            var maxContainerCount = int.Parse(Console.ReadLine()!);

            Console.Write("Enter the maximum weight (tons):");
            var maxWeight = double.Parse(Console.ReadLine()!);

            Ships.Add(new Ship(name, maxSpeed, maxContainerCount, maxWeight));
            Console.WriteLine("The ship is successfully added!");
        }

        private static void RemoveShip()
        {
            Console.Write("Enter the name of the ship for removal:");
            var name = Console.ReadLine();

            var ship = Ships.FirstOrDefault(s => s.Name == name);
            if (ship == null)
            {
                Console.WriteLine("The ship was not found.");
                return;
            }

            Ships.Remove(ship);
            Console.WriteLine("The ship is successfully removed!");
        }
        
        private static void AddContainer()
{
    Console.WriteLine("Select the type of container: ");
    Console.WriteLine("1. Refrigerated container");
    Console.WriteLine("2. Gas container");
    Console.WriteLine("3. Container with liquid");
    var choice = Console.ReadLine();

    Console.Write("Enter height: ");
    var height = double.Parse(Console.ReadLine()!);

    Console.Write("Enter the depth: ");
    var depth = double.Parse(Console.ReadLine()!);

    Console.Write("Enter the weight of the container: ");
    var tareWeight = double.Parse(Console.ReadLine()!);

    Console.Write("Enter the maximum weight of the cargo: ");
    var maxPayload = double.Parse(Console.ReadLine()!);

    Container container;

    switch (choice)
    {
        case "1":
            Console.WriteLine("Select the type of product: ");
            Array productTypes = Enum.GetValues(typeof(ProductType));
            foreach (var productType in productTypes)
            {
                Console.WriteLine($"{(int)productType}. {productType}");
            }

            var productChoice = int.Parse(Console.ReadLine() ?? "0");
            var productTypeSelected = (ProductType)productChoice;

            var validTemperature = TemperatureValidator.GetTemperature(productTypeSelected);
            Console.WriteLine($"Permissible temperature for {productTypeSelected}: {validTemperature}");
            Console.Write("Enter the supported temperature: ");
            var maintainedTemperature = double.Parse(Console.ReadLine()!);

            // Проверяем, чтобы температура была валидной
            if (!TemperatureValidator.IsValid(productTypeSelected, maintainedTemperature))
            {
                Console.WriteLine($"Error! Temperature {maintainedTemperature} Unacceptable for the product {productTypeSelected}.\nTry it again.");
                return;
            }

            container = new RefrigeratedContainer(height, depth, tareWeight, maxPayload, productTypeSelected, maintainedTemperature);
            break;

        case "2":
            Console.Write("Enter gas pressure:");
            var pressure = double.Parse(Console.ReadLine()!);
            container = new GasContainer(height, depth, tareWeight, maxPayload, pressure);
            break;

        case "3":
            Console.Write("Is the cargo dangerous? (Yes/No): ");
            var isHazardousInput = Console.ReadLine()?.ToLower();
            var isHazardous = isHazardousInput == "Yes";
            container = new LiquidContainer(height, depth, tareWeight, maxPayload, isHazardous);
            break;

        default:
            Console.WriteLine("The unacceptable type of container is chosen.");
            return;
    }

    Containers.Add(container);
    Console.WriteLine($"The container is added: {container.SerialNumber}");
}
        
        private static void PlaceContainerOnShip()
        {
            Console.Write("Enter the container number:");
            var containerId = Console.ReadLine();
            var container = Containers.FirstOrDefault(c => c.SerialNumber == containerId);

            if (container == null)
            {
                Console.WriteLine("The container was not found.");
                return;
            }

            Console.Write("Enter the name of the ship:");
            var shipName = Console.ReadLine();
            var ship = Ships.FirstOrDefault(s => s.Name == shipName);

            if (ship == null)
            {
                Console.WriteLine("The ship was not found.");
                return;
            }

            try
            {
                ship.LoadContainer(container);
                Containers.Remove(container);
                Console.WriteLine($"Container {container.SerialNumber} successfully placed on the ship {ship.Name}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        private static void RemoveContainerFromShip()
        {
            Console.Write("Enter the name of the ship:");
            var shipName = Console.ReadLine();
            var ship = Ships.FirstOrDefault(s => s.Name == shipName);

            if (ship == null)
            {
                Console.WriteLine("The ship was not found.");
                return;
            }

            Console.Write("Enter the number of the container for deletion:");
            var containerId = Console.ReadLine();

            var container = ship.UnloadContainer(containerId);

            if (container != null)
            {
                Containers.Add(container);
                Console.WriteLine($"Container {container.SerialNumber} successfully removed from the ship {ship.Name}");
            }
            else
            {
                Console.WriteLine("The container was not found on the ship.");
            }
        }

        private static void PrintShipDetails()
        {
            Console.Write("Enter the name of the ship: ");
            var shipName = Console.ReadLine();
            var ship = Ships.FirstOrDefault(s => s.Name == shipName);

            if (ship == null)
            {
                Console.WriteLine("The ship was not found.");
                return;
            }

            Console.WriteLine(ship);
        }

        private static void PrintContainerDetails()
        {
            Console.Write("Enter the container number:");
            var containerId = Console.ReadLine();
            var container = Containers.FirstOrDefault(c => c.SerialNumber == containerId);

            if (container == null)
            {
                Console.WriteLine("The container was not found.");
                return;
            }

            Console.WriteLine(container);
        }
    }
}