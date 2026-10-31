using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Rents a vehicle to a customer. A customer cannot rent more than one vehicle at the same time.
    /// </summary>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Time provider.</param>
    public sealed class RentVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        IRentVehicleOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<RentVehicleInput>
    {
        /// <inheritdoc />
        public async Task Execute(RentVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicle = await vehicleRepository.GetById(input.VehicleId);
            if (vehicle is null)
            {
                outputPort.NotFoundHandle($"The vehicle {input.VehicleId} does not exist.");
                return;
            }

            if (await vehicleRepository.HasActiveRental(input.CustomerId))
            {
                outputPort.ConflictHandle($"The customer {input.CustomerId} already has a rented vehicle.");
                return;
            }

            try
            {
                vehicle.Rent(input.CustomerId, timeProvider.GetUtcNow().UtcDateTime);
                vehicleRepository.Update(vehicle);
                await unitOfWork.Save();
            }
            catch (ConflictException ex)
            {
                // Vehicle already rented, or a concurrent request won the race (enforced by the database).
                outputPort.ConflictHandle(ex.Message);
                return;
            }

            outputPort.StandardHandle(new RentVehicleOutput(vehicle));
        }
    }
}
