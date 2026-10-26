
    using System;
    using System.Globalization;
    using GarageApp.Models;
    using GarageApp.Services;

    namespace GarageApp.UI;

    public class ConsoleUI
    {
        public void Run()
        // ↑ MÉTODO que llama Program para iniciar la aplicación.
        {
            try
            {
                RunGarage();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine();
                Console.WriteLine("Application closed.");
                // ↑ Termina de forma controlada si se cierra la entrada.
            }
        }


        private void RunGarage()
        // ↑ Prepara el garaje y mantiene el menú funcionando.
        {
            Console.WriteLine("Welcome to Garage 1.0!");

            int capacity = ReadInteger("Enter the garage capacity: ", 1);

            Garage garage = new Garage(capacity);
            // ↑ OBJETO Garage: su constructor prepara el array.

            LoadSampleVehicles(garage);
            // ↑ Permite empezar con vehículos de ejemplo.

            bool running = true;

            while (running)
            // ↑ Mantiene el menú hasta que el usuario decida salir.
            {
                int occupiedSpaces = garage.GetVehicles().Length;

                Console.WriteLine();
                Console.WriteLine("=== Garage 1.0 ===");
                Console.WriteLine($"Capacity: {garage.Capacity}");
                Console.WriteLine($"Occupied spaces: {occupiedSpaces}");
                Console.WriteLine(
                    $"Free spaces: {garage.Capacity - occupiedSpaces}");

                Console.WriteLine("1. List parked vehicles");
                Console.WriteLine("2. Park a vehicle");
                Console.WriteLine("3. Remove a vehicle");
                Console.WriteLine("4. Show vehicle counts by type");
                Console.WriteLine("5. Search vehicles");
                Console.WriteLine("0. Exit");

                string choice = ReadText("Choose an option: ");

                switch (choice)
                // ↑ Ejecuta la opción seleccionada.
                {
                    case "1":
                        ShowVehicles(garage.GetVehicles());
                        break;

                    case "2":
                        ParkVehicle(garage);
                        break;

                    case "3":
                        RemoveVehicle(garage);
                        break;

                    case "4":
                        ShowVehicleCounts(garage);
                        break;

                    case "5":
                        SearchMenu(garage);
                        break;

                    case "0":
                        running = false;
                        Console.WriteLine("Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }


        private void LoadSampleVehicles(Garage garage)
        // ↑ Permite empezar con vehículos de ejemplo.
        {
            string answer;

            while (true)
            {
                answer = ReadText("Load sample vehicles? (y/n): ");

                if (answer.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                    answer.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                Console.WriteLine("Enter y or n.");
            }

            if (answer.Equals("n", StringComparison.OrdinalIgnoreCase))
            // answer.Equals("n", StringComparison.OrdinalIgnoreCase) significa que el usuario no quiere cargar vehículos de ejemplo.
            {
                return;
            }

            Vehicle[] samples =
            {
                new Car("CAR123", "Black", 4, "Gasoline"),
                new Motorcycle("MOT123", "Pink", 3, 500),
                new Airplane("AIR123", "White", 3, 2),
                new Bus("BUS123", "Red", 6, 40),
                new Boat("BOA123", "Blue", 0, 6.5m)
            };
            // ↑ ARRAY de objetos de diferentes subclases.
            // Todos pueden estar aquí porque heredan de Vehicle.

            int added = 0;

            foreach (Vehicle vehicle in samples)
            {
                if (garage.TryPark(vehicle))
                {
                    added++;
                }
            }

            Console.WriteLine($"Sample vehicles parked: {added}.");

            if (added < samples.Length)
            {
                Console.WriteLine(
                    "Some sample vehicles could not be parked " +
                    "because there were not enough spaces.");
            }
        }


        private void ParkVehicle(Garage garage)
        // ↑ Pide los datos, crea el vehículo e intenta estacionarlo.
        {
            if (garage.GetVehicles().Length == garage.Capacity)
            {
                Console.WriteLine("The garage is full.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("=== Select vehicle type ===");
            Console.WriteLine("1. Car");
            Console.WriteLine("2. Motorcycle");
            Console.WriteLine("3. Airplane");
            Console.WriteLine("4. Bus");
            Console.WriteLine("5. Boat");
            Console.WriteLine("0. Back");

            int choice = ReadInteger("Choose a vehicle type: ", 0);

            if (choice == 0)
            {
                return;
            }

            if (choice > 5)
            {
                Console.WriteLine("Invalid vehicle type.");
                return;
            }

            string registrationNumber = ReadText("Registration number: ");

            if (garage.FindByRegistrationNumber(registrationNumber) != null)
            {
                Console.WriteLine(
                    "A vehicle with this registration number is already parked.");
                return;
            }

            string color = ReadText("Color: ");
            int numberOfWheels = ReadInteger("Number of wheels: ", 0);

            try
            {
                Vehicle vehicle;
                // ↑ VARIABLE: permitirá acceder al objeto creado
                // en uno de los casos siguientes.

                switch (choice)
                {
                    case 1:
                        string fuelType = ReadText(
                            "Fuel type (for example, Gasoline or Diesel): ");

                        vehicle = new Car(
                            registrationNumber, color, numberOfWheels, fuelType);
                        break;

                    case 2:
                        int cylinderVolume = ReadInteger(
                            "Cylinder volume in cc: ", 1);

                        vehicle = new Motorcycle(
                            registrationNumber, color, numberOfWheels, cylinderVolume);
                        break;

                    case 3:
                        int numberOfEngines = ReadInteger(
                            "Number of engines: ", 1);

                        vehicle = new Airplane(
                            registrationNumber, color, numberOfWheels, numberOfEngines);
                        break;

                    case 4:
                        int numberOfSeats = ReadInteger(
                            "Number of seats: ", 1);

                        vehicle = new Bus(
                            registrationNumber, color, numberOfWheels, numberOfSeats);
                        break;

                    case 5:
                        decimal length = ReadPositiveDecimal(
                            "Length in meters (use a decimal point): ");

                        vehicle = new Boat(
                            registrationNumber, color, numberOfWheels, length);
                        break;

                    default:
                        return;
                }

                if (garage.TryPark(vehicle))
                {
                    Console.WriteLine("Vehicle parked successfully.");
                }
                else
                {
                    Console.WriteLine(
                        "Could not park: the garage is full " +
                        "or the registration number is already parked.");
                }
            }
            catch (ArgumentException error)
            {
                Console.WriteLine(error.Message);
            }
        }


        private void RemoveVehicle(Garage garage)
        // ↑ Pide la matrícula y solicita la retirada al garaje.
        {
            Console.WriteLine();
            Console.WriteLine("=== Remove a vehicle ===");

            string registrationNumber = ReadText("Registration number: ");

            if (garage.TryRemove(registrationNumber))
            {
                Console.WriteLine("Vehicle removed successfully.");
            }
            else
            {
                Console.WriteLine(
                    "No vehicle with that registration number was found.");
            }
        }


        private void ShowVehicles(Vehicle[] vehicles)
        // ↑ Recibe un ARRAY de vehículos para mostrarlos.
        // Sirve tanto para el listado completo como para las búsquedas.
        {
            if (vehicles.Length == 0)
            {
                Console.WriteLine("No vehicles found.");
                return;
            }

            foreach (Vehicle vehicle in vehicles)
            {
                ShowVehicle(vehicle);
            }
        }


        private void ShowVehicle(Vehicle vehicle)
        // ↑ Muestra los datos comunes y el dato particular del vehículo.
        {
            Console.WriteLine();
            Console.WriteLine($"Type: {vehicle.GetType().Name}");
            Console.WriteLine(
                $"Registration: {vehicle.RegistrationNumber}");
            Console.WriteLine($"Color: {vehicle.Color}");
            Console.WriteLine($"Wheels: {vehicle.NumberOfWheels}");

            if (vehicle is Car car)
            // ↑ IS comprueba si el objeto es un Car.
            // Si lo es, car permite consultar su propiedad particular.
            {
                Console.WriteLine($"Fuel type: {car.FuelType}");
            }
            else if (vehicle is Motorcycle motorcycle)
            {
                Console.WriteLine(
                    $"Cylinder volume: {motorcycle.CylinderVolume} cc");
            }
            else if (vehicle is Airplane airplane)
            {
                Console.WriteLine(
                    $"Number of engines: {airplane.NumberOfEngines}");
            }
            else if (vehicle is Bus bus)
            {
                Console.WriteLine($"Number of seats: {bus.NumberOfSeats}");
            }
            else if (vehicle is Boat boat)
            {
                Console.WriteLine($"Length: {boat.Length} meters");
            }
        }


        private void ShowVehicleCounts(Garage garage)
        // ↑ Solicita al garaje la cantidad de cada tipo.
        {
            Console.WriteLine();
            Console.WriteLine("=== Vehicle counts ===");
            Console.WriteLine($"Cars: {garage.CountByType("Car")}");
            Console.WriteLine(
                $"Motorcycles: {garage.CountByType("Motorcycle")}");
            Console.WriteLine($"Airplanes: {garage.CountByType("Airplane")}");
            Console.WriteLine($"Buses: {garage.CountByType("Bus")}");
            Console.WriteLine($"Boats: {garage.CountByType("Boat")}");
        }


        private void SearchMenu(Garage garage)
        // ↑ SUBMENÚ: permite elegir cómo buscar.
        {
            bool searching = true;

            while (searching)
            {
                Console.WriteLine();
                Console.WriteLine("=== Search vehicles ===");
                Console.WriteLine("1. Search by registration number");
                Console.WriteLine("2. Search with combined filters");
                Console.WriteLine("0. Back");

                string choice = ReadText("Choose an option: ");

                switch (choice)
                {
                    case "1":
                        string registrationNumber = ReadText(
                            "Registration number: ");

                        Vehicle? vehicle = garage.FindByRegistrationNumber(
                            registrationNumber);

                        if (vehicle == null)
                        {
                            Console.WriteLine("Vehicle not found.");
                        }
                        else
                        {
                            ShowVehicle(vehicle);
                        }

                        break;

                    case "2":
                        SearchWithFilters(garage);
                        break;

                    case "0":
                        searching = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }


        private void SearchWithFilters(Garage garage)
        // ↑ Recoge filtros opcionales y pide la búsqueda al garaje.
        {
            Console.WriteLine();
            Console.WriteLine("Leave a filter empty to skip it.");

            string registrationNumber = ReadText(
                "Registration number: ", true);

            string color = ReadText("Color: ", true);

            int? numberOfWheels = null;
            // ↑ null significa que no aplicaremos filtro de ruedas.

            while (true)
            {
                string wheelsInput = ReadText("Number of wheels: ", true);

                if (wheelsInput == "")
                {
                    break;
                }

                if (int.TryParse(wheelsInput, out int wheels) && wheels >= 0)
                {
                    numberOfWheels = wheels;
                    break;
                }

                Console.WriteLine(
                    "Enter a whole number greater than or equal to zero, " +
                    "or leave it empty.");
            }

            string vehicleType;

            while (true)
            {
                vehicleType = ReadText(
                    "Type (Car, Motorcycle, Airplane, Bus, Boat): ", true);

                if (vehicleType == "" ||
                    vehicleType.Equals("Car", StringComparison.OrdinalIgnoreCase) ||
                    vehicleType.Equals("Motorcycle", StringComparison.OrdinalIgnoreCase) ||
                    vehicleType.Equals("Airplane", StringComparison.OrdinalIgnoreCase) ||
                    vehicleType.Equals("Bus", StringComparison.OrdinalIgnoreCase) ||
                    vehicleType.Equals("Boat", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                Console.WriteLine("Enter one of the listed types.");
            }

            Vehicle[] results = garage.Search(
                registrationNumber, color, numberOfWheels, vehicleType);
            // ↑ El garaje busca los vehículos que cumplen
            // TODOS los filtros seleccionados.

            Console.WriteLine();
            Console.WriteLine($"Vehicles found: {results.Length}");
            ShowVehicles(results);
        }


        private string ReadText(string prompt, bool allowEmpty = false)
        // ↑ MÉTODO auxiliar: muestra una pregunta y recibe texto.
        // allowEmpty permite aceptar Enter sin escribir un dato.
        // Por defecto es false: normalmente exigimos texto.
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (input == null)
                {
                    throw new OperationCanceledException();
                }

                input = input.Trim();

                if (allowEmpty || input != "")
                {
                    return input;
                }

                Console.WriteLine("This field cannot be empty.");
            }
        }


        private int ReadInteger(string prompt, int minimum)
        // ↑ Pide un entero y comprueba el mínimo permitido.
        {
            while (true)
            {
                string input = ReadText(prompt);

                if (int.TryParse(input, out int number) &&
                    number >= minimum)
                {
                    return number;
                }

                Console.WriteLine(
                    $"Enter a whole number greater than or equal to {minimum}.");
            }
        }


        private decimal ReadPositiveDecimal(string prompt)
        // ↑ Pide un decimal mayor que cero.
        {
            while (true)
            {
                string input = ReadText(prompt);

                if (decimal.TryParse(
                    input,
                    NumberStyles.AllowDecimalPoint |
                    NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture,
                    out decimal number) &&
                    number > 0)
                {
                    return number;
                }

                Console.WriteLine(
                    "Enter a number greater than zero. Use a decimal point.");
            }
        }
    }
