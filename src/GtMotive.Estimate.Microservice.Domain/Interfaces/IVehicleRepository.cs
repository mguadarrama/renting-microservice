using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Customers;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Fleet persistence. Reads are immediate; writes are staged and applied by <see cref="IUnitOfWork.Save"/>.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Gets a vehicle by its identifier.
        /// </summary>
        /// <param name="id">Vehicle identifier.</param>
        /// <returns>The vehicle, or null when it does not exist.</returns>
        Task<Vehicle> GetById(VehicleId id);

        /// <summary>
        /// Gets the vehicles that are not rented and were manufactured on or after the given date.
        /// </summary>
        /// <param name="minManufactureDate">Earliest manufacture date allowed in the fleet.</param>
        /// <returns>The available vehicles.</returns>
        Task<IReadOnlyCollection<Vehicle>> GetAvailable(DateOnly minManufactureDate);

        /// <summary>
        /// Checks whether a vehicle with the given license plate already exists.
        /// </summary>
        /// <param name="licensePlate">License plate.</param>
        /// <returns>True when the plate is already registered.</returns>
        Task<bool> ExistsWithLicensePlate(LicensePlate licensePlate);

        /// <summary>
        /// Checks whether the customer is currently renting any vehicle.
        /// </summary>
        /// <param name="customerId">Customer identifier.</param>
        /// <returns>True when the customer has an active rental.</returns>
        Task<bool> HasActiveRental(CustomerId customerId);

        /// <summary>
        /// Stages a new vehicle to be inserted.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        void Add(Vehicle vehicle);

        /// <summary>
        /// Stages the changes of an existing vehicle to be saved.
        /// </summary>
        /// <param name="vehicle">Vehicle.</param>
        void Update(Vehicle vehicle);
    }
}
