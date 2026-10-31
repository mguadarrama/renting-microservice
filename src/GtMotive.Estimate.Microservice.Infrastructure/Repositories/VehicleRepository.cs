using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Customers;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.Repositories
{
    internal sealed class VehicleRepository(VehicleCollection vehicles, MongoUnitOfWork unitOfWork) : IVehicleRepository
    {
        // Version of every vehicle read in this scope, used for optimistic concurrency on update.
        private readonly Dictionary<Guid, int> _loadedVersions = [];

        public async Task<Vehicle> GetById(VehicleId id)
        {
            ArgumentNullException.ThrowIfNull(id);

            var collection = await vehicles.Get();
            var document = await collection.Find(v => v.Id == id.Value).SingleOrDefaultAsync();

            return Track(document);
        }

        public async Task<IReadOnlyCollection<Vehicle>> GetAvailable(DateOnly minManufactureDate)
        {
            var minDate = minManufactureDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            var collection = await vehicles.Get();
            var documents = await collection
                .Find(v => v.RentedBy == null && v.ManufactureDate >= minDate)
                .SortBy(v => v.Brand)
                .ThenBy(v => v.Model)
                .ToListAsync();

            return [.. documents.Select(Track)];
        }

        public async Task<bool> ExistsWithLicensePlate(LicensePlate licensePlate)
        {
            ArgumentNullException.ThrowIfNull(licensePlate);

            var collection = await vehicles.Get();
            return await collection.Find(v => v.LicensePlate == licensePlate.Value).AnyAsync();
        }

        public async Task<bool> HasActiveRental(CustomerId customerId)
        {
            ArgumentNullException.ThrowIfNull(customerId);

            var collection = await vehicles.Get();
            return await collection.Find(v => v.RentedBy == customerId.Value).AnyAsync();
        }

        public void Add(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            var document = VehicleDocument.FromDomain(vehicle, version: 1);

            unitOfWork.Enqueue(async () =>
            {
                var collection = await vehicles.Get();
                await TranslateDuplicateKey(() => collection.InsertOneAsync(document));
            });
        }

        public void Update(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            if (!_loadedVersions.TryGetValue(vehicle.Id.Value, out var loadedVersion))
            {
                throw new InvalidOperationException($"The vehicle {vehicle.Id} must be read through this repository before updating it.");
            }

            var document = VehicleDocument.FromDomain(vehicle, loadedVersion + 1);

            unitOfWork.Enqueue(async () =>
            {
                var collection = await vehicles.Get();

                ReplaceOneResult result = null;
                await TranslateDuplicateKey(async () =>
                    result = await collection.ReplaceOneAsync(v => v.Id == document.Id && v.Version == loadedVersion, document));

                if (result.MatchedCount == 0)
                {
                    throw new ConflictException($"The vehicle {vehicle.LicensePlate} was modified by another request. Please try again.");
                }

                _loadedVersions[document.Id] = document.Version;
            });
        }

        private static async Task TranslateDuplicateKey(Func<Task> write)
        {
            try
            {
                await write();
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                if (ex.WriteError.Message.Contains(VehicleCollection.RentedByIndex, StringComparison.Ordinal))
                {
                    throw new CustomerAlreadyHasRentalException("The customer already has a rented vehicle.", ex);
                }

                throw new LicensePlateAlreadyRegisteredException("A vehicle with the same license plate already exists in the fleet.", ex);
            }
        }

        private Vehicle Track(VehicleDocument document)
        {
            if (document is null)
            {
                return null;
            }

            _loadedVersions[document.Id] = document.Version;
            return document.ToDomain();
        }
    }
}
