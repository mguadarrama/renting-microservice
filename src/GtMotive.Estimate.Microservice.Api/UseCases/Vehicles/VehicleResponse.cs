using System;
using System.ComponentModel.DataAnnotations;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles
{
    /// <summary>
    /// Vehicle of the fleet.
    /// </summary>
    public sealed class VehicleResponse
    {
        private VehicleResponse(Vehicle vehicle)
        {
            Id = vehicle.Id.Value;
            LicensePlate = vehicle.LicensePlate.Value;
            Brand = vehicle.Brand;
            Model = vehicle.Model;
            ManufactureDate = vehicle.ManufactureDate;
            IsRented = vehicle.IsRented;
        }

        /// <summary>
        /// Gets the vehicle identifier.
        /// </summary>
        [Required]
        public Guid Id { get; }

        /// <summary>
        /// Gets the normalized license plate.
        /// </summary>
        [Required]
        public string LicensePlate { get; }

        /// <summary>
        /// Gets the brand.
        /// </summary>
        [Required]
        public string Brand { get; }

        /// <summary>
        /// Gets the model.
        /// </summary>
        [Required]
        public string Model { get; }

        /// <summary>
        /// Gets the manufacture date.
        /// </summary>
        [Required]
        public DateOnly ManufactureDate { get; }

        /// <summary>
        /// Gets a value indicating whether the vehicle is currently rented.
        /// </summary>
        [Required]
        public bool IsRented { get; }

        internal static VehicleResponse From(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            return new VehicleResponse(vehicle);
        }
    }
}
