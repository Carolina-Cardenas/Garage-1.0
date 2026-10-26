
    using System;
    using GarageApp.Models;

    namespace GarageApp.Services;

    public class Garage
    // ↑ CLASE: representa el garaje.
    {
        private Vehicle?[] _vehicles;
        // ↑ CAMPO PRIVADO: guarda el array de vehículos.
      
        // ? permite posiciones vacías, representadas por null.


        public int Capacity
        // ↑ PROPIEDAD: permite consultar la capacidad total.
        {
            get
            {
                return _vehicles.Length;
                // ↑ Entrega la cantidad de posiciones del array.
            }
        }


        public Garage(int capacity)
        // ↑ CONSTRUCTOR: mismo nombre que la clase.
        // capacity recibe la cantidad de espacios solicitada.
        {
            if (capacity <= 0)
            {
                throw new ArgumentException(
                    "Garage capacity must be greater than zero.");
            }

            _vehicles = new Vehicle?[capacity];
            // ↑ Crea los espacios. Todos empiezan vacíos.
        }


        public Vehicle? FindByRegistrationNumber(string registrationNumber)
        // ↑ MÉTODO: busca por matrícula.
        // Devuelve el vehículo encontrado o null si no existe.
        {
            for (int i = 0; i < _vehicles.Length; i++)
            // ↑ Recorre las posiciones del array, empezando en cero.
            {
                Vehicle? vehicle = _vehicles[i];
                // ↑ Variable temporal que apunta al vehículo en la posición i.
                // ? permite que la variable pueda ser null, si la posición está vacía.
                

                if (vehicle != null)
                // ↑ Comprueba que la posición tenga un vehículo.
                {
                    if (string.Equals(
                        vehicle.RegistrationNumber,
                        registrationNumber.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    // ↑ Compara sin distinguir mayúsculas y minúsculas.
                    {
                        return vehicle;
                    }
                }
            }

            return null;
            // ↑ No encontró la matrícula.
        }


        public bool TryPark(Vehicle vehicle)
        // ↑ MÉTODO: intenta estacionar.
        // Devuelve true si lo consigue y false si no.
        {
            Vehicle? existingVehicle = FindByRegistrationNumber(
                // vehicle? permite que la variable pueda ser null, si la posición está vacía.
                vehicle.RegistrationNumber);

            if (existingVehicle != null)
             // existingvehicle != null significa que ya hay un vehículo con esa matrícula.
            {
                return false;
                // ↑ Rechaza una matrícula que ya está estacionada.
            }

            for (int i = 0; i < _vehicles.Length; i++)
             
            {
                if (_vehicles[i] == null)
                // ↑ Encontró un espacio vacío.
                {
                    _vehicles[i] = vehicle;
                    // ↑ Guarda la referencia al vehículo.

                    return true;
                }
            }

            return false;
            // ↑ No hay espacios disponibles.
        }


        public bool TryRemove(string registrationNumber)
        // ↑ MÉTODO: intenta retirar un vehículo por matrícula.
        {
            for (int i = 0; i < _vehicles.Length; i++)
            {
                Vehicle? vehicle = _vehicles[i];

                if (vehicle != null)
                {
                    if (string.Equals(
                        vehicle.RegistrationNumber,
                        registrationNumber.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _vehicles[i] = null;
                        // ↑ Libera el espacio encontrado.

                        return true;
                    }
                }
            }

            return false;
            // ↑ No encontró el vehículo solicitado.
        }


        public Vehicle[] GetVehicles()
        // ↑ MÉTODO: entrega los vehículos estacionados
        // en otro array, sin exponer el array privado.
        {
            int count = 0;

            // Cuenta los espacios ocupados.
            for (int i = 0; i < _vehicles.Length; i++)
            {
                if (_vehicles[i] != null)
                {
                    count++;
                }
            }

            Vehicle[] parkedVehicles = new Vehicle[count];

            int position = 0;

            // Copia las referencias a los vehículos encontrados.
            foreach (Vehicle? vehicle in _vehicles)
            {
                if (vehicle != null)
                {
                    parkedVehicles[position] = vehicle;
                    position++;
                }
            }

            return parkedVehicles;
        }


        public int CountByType(string vehicleType)
        // ↑ NUEVO MÉTODO: cuenta los vehículos del tipo recibido.
        // vehicleType puede recibir "Car", "Boat", "Bus", etc.
        // int indica que devuelve un número entero.
        {
            int count = 0;
            // ↑ Contador que empieza en cero.

            foreach (Vehicle? vehicle in _vehicles)
            // ↑ Examina cada posición del array.
            {
                if (vehicle != null)
                // ↑ Solo examina el tipo si hay un vehículo.
                {
                    string typeName = vehicle.GetType().Name;
                    // ↑ GetType() obtiene el tipo real del objeto.
                    // Name entrega su nombre: Car, Boat, Bus...

                    if (string.Equals(
                        typeName,
                        vehicleType,
                        StringComparison.OrdinalIgnoreCase))
                    // ↑ Comprueba si coincide con el tipo solicitado.
                    {
                        count++;
                        // ↑ Suma uno por cada coincidencia.
                    }
                }
            }

            return count;
            // ↑ Entrega la cantidad encontrada.
        }
     public Vehicle[] Search(
            string registrationNumber,
            string color,
            int? numberOfWheels,
            string vehicleType)
        // ↑ NUEVO MÉTODO: recibe los cuatro filtros.
        // Vehicle[] indica que devuelve un array de vehículos.
        //
        // Un texto vacío significa "no aplicar este filtro".
        // int? permite un número o null.
        // Para ruedas, null significa "no filtrar por ruedas".
        // Cero sí es un filtro: buscar vehículos con cero ruedas.
        {
            Vehicle[] results = new Vehicle[Capacity];
            // ↑ Prepara espacio para los posibles resultados.
            // Este array es diferente del array privado _vehicles.

            int count = 0;
            // ↑ Cuenta cuántos vehículos cumplen los filtros.

            foreach (Vehicle? vehicle in _vehicles)
            {
                if (vehicle == null)
                {
                    continue;
                    // ↑ Espacio vacío: pasa al siguiente elemento.
                }


                if (!string.IsNullOrWhiteSpace(registrationNumber))
                // ↑ Solo aplica este filtro si recibimos una matrícula.
                {
                    if (!string.Equals(
                        vehicle.RegistrationNumber,
                        registrationNumber.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                        // ↑ No coincide: descarta este vehículo
                        // y pasa al siguiente.
                    }
                }


                if (!string.IsNullOrWhiteSpace(color))
                // ↑ Solo aplica este filtro si recibimos un color.
                {
                    if (!string.Equals(
                        vehicle.Color,
                        color.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }


                if (numberOfWheels.HasValue)
                // ↑ HasValue pregunta si recibimos un número.
                // Si recibimos null, no comprobamos las ruedas.
                {
                    if (vehicle.NumberOfWheels != numberOfWheels.Value)
                    // ↑ Value obtiene el número del filtro.
                
                    {
                        continue;
                    }
                }


                if (!string.IsNullOrWhiteSpace(vehicleType))
                // ↑ Solo aplica este filtro si recibimos un tipo.
                {
                    if (!string.Equals(
                        vehicle.GetType().Name,
                        vehicleType.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }


                results[count] = vehicle;
                // ↑ Solo llegamos aquí si pasó TODOS los filtros activos.
                // Guarda el vehículo en el array de resultados.

                count++;
            }

            Array.Resize(ref results, count);
            // ↑ Prepara un array con el tamaño exacto de los resultados,
            // conservando los vehículos encontrados.
            // ref permite que Resize actualice la variable results.
            //
            // No modifica la capacidad ni el array privado del garaje.

           
              return results[..count];
            // ↑ Devuelve un array con los vehículos encontrados.
        }
    }

