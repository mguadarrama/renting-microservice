using System;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Vehicle identifier value object.
    /// </summary>
    public sealed record VehicleId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleId"/> class.
        /// </summary>
        /// <param name="value">Identifier value.</param>
        public VehicleId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new DomainException("The vehicle id cannot be empty.");
            }

            Value = value;
        }

        /// <summary>
        /// Gets the identifier value.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Creates a new unique identifier.
        /// </summary>
        /// <returns>A new vehicle identifier.</returns>
        public static VehicleId New()
        {
            return new VehicleId(Guid.NewGuid());
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
