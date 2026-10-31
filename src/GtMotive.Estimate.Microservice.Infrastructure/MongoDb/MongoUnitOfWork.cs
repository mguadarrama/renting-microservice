using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    /// <summary>
    /// Collects the writes staged by the repositories during a request and applies them on <see cref="Save"/>.
    /// Every use case writes a single document, so MongoDB single-document atomicity is enough and no
    /// multi-document transaction (which would need a replica set) is required.
    /// </summary>
    internal sealed class MongoUnitOfWork : IUnitOfWork
    {
        private readonly List<Func<Task>> _pendingWrites = [];

        public void Enqueue(Func<Task> write)
        {
            ArgumentNullException.ThrowIfNull(write);
            _pendingWrites.Add(write);
        }

        public async Task<int> Save()
        {
            var writes = _pendingWrites.ToArray();
            _pendingWrites.Clear();

            foreach (var write in writes)
            {
                await write();
            }

            return writes.Length;
        }
    }
}
