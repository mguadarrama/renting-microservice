using System;
using GtMotive.Estimate.Microservice.Domain.Customers;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MongoDB.Bson.Serialization.Attributes;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents
{
    /// <summary>
    /// Persistence model of a vehicle. Keeps the domain model free of database concerns.
    /// </summary>
    internal sealed class VehicleDocument
    {
        [BsonId]
        public Guid Id { get; set; }

        public string LicensePlate { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime ManufactureDate { get; set; }

        public string RentedBy { get; set; }

        public DateTime? RentedAt { get; set; }

        /// <summary>
        /// Gets or sets the optimistic concurrency token, incremented on every update.
        /// </summary>
        public int Version { get; set; }

        public static VehicleDocument FromDomain(Vehicle vehicle, int version)
        {
            return new VehicleDocument
            {
                Id = vehicle.Id.Value,
                LicensePlate = vehicle.LicensePlate.Value,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                ManufactureDate = vehicle.ManufactureDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                RentedBy = vehicle.CurrentRental?.CustomerId.Value,
                RentedAt = vehicle.CurrentRental?.StartedAt,
                Version = version,
            };
        }

        public Vehicle ToDomain()
        {
            var rental = RentedBy is null
                ? null
                : new Rental(new CustomerId(RentedBy), RentedAt.GetValueOrDefault());

            return Vehicle.Restore(
                new VehicleId(Id),
                new LicensePlate(LicensePlate),
                Brand,
                Model,
                DateOnly.FromDateTime(ManufactureDate),
                rental);
        }
    }
}
