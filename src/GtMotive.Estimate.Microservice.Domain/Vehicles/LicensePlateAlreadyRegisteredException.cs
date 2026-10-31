using System;
using System.Diagnostics.CodeAnalysis;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Raised when a vehicle with the same license plate already belongs to the fleet.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LicensePlateAlreadyRegisteredException : ConflictException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlateAlreadyRegisteredException"/> class.
        /// </summary>
        public LicensePlateAlreadyRegisteredException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlateAlreadyRegisteredException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public LicensePlateAlreadyRegisteredException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlateAlreadyRegisteredException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public LicensePlateAlreadyRegisteredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
