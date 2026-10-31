using System.Linq;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// License plate value object. Normalized to upper case without spaces or dashes,
    /// so "1234-ABC" and "1234 abc" are the same plate.
    /// </summary>
    public sealed record LicensePlate
    {
        /// <summary>
        /// Minimum number of characters of a normalized plate.
        /// </summary>
        public const int MinLength = 4;

        /// <summary>
        /// Maximum number of characters of a normalized plate.
        /// </summary>
        public const int MaxLength = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensePlate"/> class.
        /// </summary>
        /// <param name="value">Raw plate as typed by the user.</param>
        public LicensePlate(string value)
        {
            var normalized = string.Concat((value ?? string.Empty).Where(c => c is not (' ' or '-')))
                .ToUpperInvariant();

            if (normalized.Length is < MinLength or > MaxLength || !normalized.All(char.IsAsciiLetterOrDigit))
            {
                throw new DomainException(
                    $"The license plate '{value}' is not valid. It must have between {MinLength} and {MaxLength} letters or digits.");
            }

            Value = normalized;
        }

        /// <summary>
        /// Gets the normalized plate.
        /// </summary>
        public string Value { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }
    }
}
