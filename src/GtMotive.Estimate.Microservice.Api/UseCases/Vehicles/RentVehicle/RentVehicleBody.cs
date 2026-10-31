using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Body of the request to rent a vehicle.
    /// </summary>
    public sealed class RentVehicleBody
    {
        /// <summary>
        /// Gets or sets the identifier of the person renting the vehicle (e.g. national id number).
        /// </summary>
        [Required]
        [StringLength(Domain.Customers.CustomerId.MaxLength)]
        public string CustomerId { get; set; }
    }
}
