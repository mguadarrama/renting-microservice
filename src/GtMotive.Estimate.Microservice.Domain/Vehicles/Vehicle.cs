using System;
using GtMotive.Estimate.Microservice.Domain.Customers;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Vehicle of the fleet. Aggregate root that protects the rental lifecycle and the fleet age rule.
    /// </summary>
    public sealed class Vehicle
    {
        /// <summary>
        /// Maximum age, in years since manufacture, a vehicle can have to belong to the fleet.
        /// </summary>
        public const int MaxAgeInYears = 5;

        /// <summary>
        /// Maximum number of characters of the brand and model.
        /// </summary>
        public const int MaxNameLength = 50;

        private Vehicle(
            VehicleId id,
            LicensePlate licensePlate,
            string brand,
            string model,
            DateOnly manufactureDate,
            Rental currentRental)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(licensePlate);

            Id = id;
            LicensePlate = licensePlate;
            Brand = RequireName(brand, nameof(Brand));
            Model = RequireName(model, nameof(Model));
            ManufactureDate = manufactureDate;
            CurrentRental = currentRental;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        public VehicleId Id { get; }

        /// <summary>
        /// Gets the license plate.
        /// </summary>
        public LicensePlate LicensePlate { get; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        public string Brand { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        public DateOnly ManufactureDate { get; }

        /// <summary>
        /// Gets the active rental, or null when the vehicle is not rented.
        /// </summary>
        public Rental CurrentRental { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the vehicle is currently rented.
        /// </summary>
        public bool IsRented => CurrentRental is not null;

        /// <summary>
        /// Registers a new vehicle in the fleet.
        /// </summary>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <param name="utcNow">Current UTC date and time.</param>
        /// <returns>The new vehicle.</returns>
        /// <exception cref="VehicleTooOldException">The vehicle is older than <see cref="MaxAgeInYears"/>.</exception>
        public static Vehicle Create(
            LicensePlate licensePlate,
            string brand,
            string model,
            DateOnly manufactureDate,
            DateTime utcNow)
        {
            if (manufactureDate > DateOnly.FromDateTime(utcNow))
            {
                throw new DomainException("The manufacture date cannot be in the future.");
            }

            if (IsTooOld(manufactureDate, utcNow))
            {
                throw new VehicleTooOldException(
                    $"Vehicles older than {MaxAgeInYears} years cannot be added to the fleet (manufactured on {manufactureDate:yyyy-MM-dd}).");
            }

            return new Vehicle(VehicleId.New(), licensePlate, brand, model, manufactureDate, currentRental: null);
        }

        /// <summary>
        /// Rebuilds a vehicle already stored in the fleet. Does not apply creation rules.
        /// </summary>
        /// <param name="id">Vehicle identifier.</param>
        /// <param name="licensePlate">License plate.</param>
        /// <param name="brand">Brand.</param>
        /// <param name="model">Model.</param>
        /// <param name="manufactureDate">Manufacture date.</param>
        /// <param name="currentRental">Active rental, or null.</param>
        /// <returns>The vehicle.</returns>
        public static Vehicle Restore(
            VehicleId id,
            LicensePlate licensePlate,
            string brand,
            string model,
            DateOnly manufactureDate,
            Rental currentRental)
        {
            return new Vehicle(id, licensePlate, brand, model, manufactureDate, currentRental);
        }

        /// <summary>
        /// Earliest manufacture date a vehicle can have to belong to the fleet at the given moment.
        /// </summary>
        /// <param name="utcNow">Current UTC date and time.</param>
        /// <returns>The minimum manufacture date allowed.</returns>
        public static DateOnly MinManufactureDate(DateTime utcNow)
        {
            return DateOnly.FromDateTime(utcNow).AddYears(-MaxAgeInYears);
        }

        /// <summary>
        /// Checks whether the vehicle can be rented right now: not rented and still young enough for the fleet.
        /// </summary>
        /// <param name="utcNow">Current UTC date and time.</param>
        /// <returns>True when the vehicle can be rented.</returns>
        public bool IsAvailable(DateTime utcNow)
        {
            return !IsRented && !IsTooOld(ManufactureDate, utcNow);
        }

        /// <summary>
        /// Rents the vehicle to a customer.
        /// </summary>
        /// <param name="customerId">Customer renting the vehicle.</param>
        /// <param name="utcNow">Current UTC date and time.</param>
        /// <exception cref="VehicleNotAvailableException">The vehicle is already rented.</exception>
        /// <exception cref="VehicleTooOldException">The vehicle has aged out of the fleet.</exception>
        public void Rent(CustomerId customerId, DateTime utcNow)
        {
            ArgumentNullException.ThrowIfNull(customerId);

            if (IsRented)
            {
                throw new VehicleNotAvailableException($"The vehicle {LicensePlate} is already rented.");
            }

            if (IsTooOld(ManufactureDate, utcNow))
            {
                throw new VehicleTooOldException(
                    $"The vehicle {LicensePlate} is older than {MaxAgeInYears} years and can no longer be rented.");
            }

            CurrentRental = new Rental(customerId, utcNow);
        }

        /// <summary>
        /// Returns the vehicle to the fleet, ending the active rental.
        /// </summary>
        /// <returns>The rental that has just ended.</returns>
        /// <exception cref="VehicleNotRentedException">The vehicle is not rented.</exception>
        public Rental Return()
        {
            if (!IsRented)
            {
                throw new VehicleNotRentedException($"The vehicle {LicensePlate} is not rented.");
            }

            var endedRental = CurrentRental;
            CurrentRental = null;
            return endedRental;
        }

        private static bool IsTooOld(DateOnly manufactureDate, DateTime utcNow)
        {
            return manufactureDate < MinManufactureDate(utcNow);
        }

        private static string RequireName(string value, string fieldName)
        {
            var trimmed = value?.Trim();

            if (string.IsNullOrEmpty(trimmed) || trimmed.Length > MaxNameLength)
            {
                throw new DomainException($"The {fieldName} field is required and cannot exceed {MaxNameLength} characters.");
            }

            return trimmed;
        }
    }
}
