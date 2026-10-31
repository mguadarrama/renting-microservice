using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    /// <summary>
    /// Gives access to the vehicles collection, creating its indexes the first time it is used.
    /// </summary>
    internal sealed class VehicleCollection : IDisposable
    {
        public const string LicensePlateIndex = "ux_licensePlate";

        public const string RentedByIndex = "ux_rentedBy";

        private const string CollectionName = "vehicles";

        private readonly IMongoCollection<VehicleDocument> _collection;
        private readonly SemaphoreSlim _indexLock = new(1, 1);
        private bool _indexesCreated;

        public VehicleCollection(MongoService mongoService)
        {
            ArgumentNullException.ThrowIfNull(mongoService);
            _collection = mongoService.Database.GetCollection<VehicleDocument>(CollectionName);
        }

        public async Task<IMongoCollection<VehicleDocument>> Get()
        {
            if (_indexesCreated)
            {
                return _collection;
            }

            await _indexLock.WaitAsync();
            try
            {
                if (!_indexesCreated)
                {
                    await CreateIndexes();
                    _indexesCreated = true;
                }
            }
            finally
            {
                _indexLock.Release();
            }

            return _collection;
        }

        public void Dispose()
        {
            _indexLock.Dispose();
        }

        private async Task CreateIndexes()
        {
            var keys = Builders<VehicleDocument>.IndexKeys;

            // Unique plate: two vehicles cannot share a license plate.
            var licensePlate = new CreateIndexModel<VehicleDocument>(
                keys.Ascending(v => v.LicensePlate),
                new CreateIndexOptions { Name = LicensePlateIndex, Unique = true });

            // Unique among rented vehicles only: the database guarantees a customer has at most
            // one active rental even if two rent requests for the same customer run at the same time.
            var rentedBy = new CreateIndexModel<VehicleDocument>(
                keys.Ascending(v => v.RentedBy),
                new CreateIndexOptions<VehicleDocument>
                {
                    Name = RentedByIndex,
                    Unique = true,
                    PartialFilterExpression = new BsonDocument(nameof(VehicleDocument.RentedBy), new BsonDocument("$type", "string")),
                });

            await _collection.Indexes.CreateManyAsync([licensePlate, rentedBy]);
        }
    }
}
