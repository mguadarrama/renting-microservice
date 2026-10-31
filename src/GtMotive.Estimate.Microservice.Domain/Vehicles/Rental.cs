using System;
using GtMotive.Estimate.Microservice.Domain.Customers;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Active rental of a vehicle: who has it and since when.
    /// </summary>
    public sealed record Rental
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rental"/> class.
        /// </summary>
        /// <param name="customerId">Customer renting the vehicle.</param>
        /// <param name="startedAt">UTC date and time the rental started.</param>
        public Rental(CustomerId customerId, DateTime startedAt)
        {
            ArgumentNullException.ThrowIfNull(customerId);

            CustomerId = customerId;
            StartedAt = startedAt;
        }

        /// <summary>
        /// Gets the customer renting the vehicle.
        /// </summary>
        public CustomerId CustomerId { get; }

        /// <summary>
        /// Gets the UTC date and time the rental started.
        /// </summary>
        public DateTime StartedAt { get; }
    }
}
