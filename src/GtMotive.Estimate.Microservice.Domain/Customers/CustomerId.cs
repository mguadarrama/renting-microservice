using System.Linq;

namespace GtMotive.Estimate.Microservice.Domain.Customers
{
    /// <summary>
    /// Identifies the person renting a vehicle (for example a national id number).
    /// Customers are owned by another bounded context, so the renting service only references them by identity.
    /// </summary>
    public sealed record CustomerId
    {
        /// <summary>
        /// Maximum number of characters of the identifier.
        /// </summary>
        public const int MaxLength = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerId"/> class.
        /// </summary>
        /// <param name="value">Identifier value.</param>
        public CustomerId(string value)
        {
            var normalized = (value ?? string.Empty).Trim().ToUpperInvariant();

            if (normalized.Length is 0 or > MaxLength || !normalized.All(char.IsAsciiLetterOrDigit))
            {
                throw new DomainException(
                    $"The customer id '{value}' is not valid. It must have up to {MaxLength} letters or digits.");
            }

            Value = normalized;
        }

        /// <summary>
        /// Gets the normalized identifier.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }
    }
}
