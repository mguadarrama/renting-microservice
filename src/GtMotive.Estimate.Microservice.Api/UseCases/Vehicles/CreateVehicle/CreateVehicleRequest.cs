using System;
using System.ComponentModel.DataAnnotations;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Request to add a vehicle to the fleet.
    /// </summary>
    public sealed class CreateVehicleRequest : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Gets or sets the license plate, e.g. "1234-ABC".
        /// </summary>
        [Required]
        [StringLength(Domain.Vehicles.LicensePlate.MaxLength + 4, MinimumLength = Domain.Vehicles.LicensePlate.MinLength)]
        public string LicensePlate { get; set; }

        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        [Required]
        [StringLength(Vehicle.MaxNameLength)]
        public string Brand { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        [Required]
        [StringLength(Vehicle.MaxNameLength)]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the manufacture date (yyyy-MM-dd).
        /// </summary>
        [Required]
        public DateOnly? ManufactureDate { get; set; }
    }
}
