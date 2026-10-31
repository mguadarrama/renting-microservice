using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ReturnVehicle
{
    /// <summary>
    /// Returns a rented vehicle to the fleet.
    /// </summary>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Time provider.</param>
    public sealed class ReturnVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        IReturnVehicleOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<ReturnVehicleInput>
    {
        /// <inheritdoc />
        public async Task Execute(ReturnVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicle = await vehicleRepository.GetById(input.VehicleId);
            if (vehicle is null)
            {
                outputPort.NotFoundHandle($"The vehicle {input.VehicleId} does not exist.");
                return;
            }

            try
            {
                var endedRental = vehicle.Return();
                vehicleRepository.Update(vehicle);
                await unitOfWork.Save();

                outputPort.StandardHandle(new ReturnVehicleOutput(vehicle, endedRental, timeProvider.GetUtcNow().UtcDateTime));
            }
            catch (ConflictException ex)
            {
                outputPort.ConflictHandle(ex.Message);
            }
        }
    }
}
