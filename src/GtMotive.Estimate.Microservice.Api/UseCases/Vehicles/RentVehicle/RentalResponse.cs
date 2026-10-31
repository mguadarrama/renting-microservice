using System;
using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Rental of a vehicle.
    /// </summary>
    /// <param name="vehicleId">Vehicle identifier.</param>
    /// <param name="licensePlate">License plate.</param>
    /// <param name="customerId">Customer renting the vehicle.</param>
    /// <param name="startedAt">UTC date and time the rental started.</param>
    /// <param name="returnedAt">UTC date and time the vehicle was returned, or null while it is rented.</param>
    public sealed class RentalResponse(Guid vehicleId, string licensePlate, string customerId, DateTime startedAt, DateTime? returnedAt)
    {
        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        [Required]
        public Guid VehicleId { get; } = vehicleId;

        /// <summary>
        /// Gets the license plate.
        /// </summary>
        [Required]
        public string LicensePlate { get; } = licensePlate;

        /// <summary>
        /// Gets the customer renting the vehicle.
        /// </summary>
        [Required]
        public string CustomerId { get; } = customerId;

        /// <summary>
        /// Gets the UTC date and time the rental started.
        /// </summary>
        [Required]
        public DateTime StartedAt { get; } = startedAt;

        /// <summary>
        /// Gets the UTC date and time the vehicle was returned, or null while it is rented.
        /// </summary>
        public DateTime? ReturnedAt { get; } = returnedAt;
    }
}
