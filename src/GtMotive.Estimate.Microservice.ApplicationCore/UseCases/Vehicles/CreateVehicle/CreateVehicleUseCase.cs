using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Adds a new vehicle to the fleet.
    /// </summary>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Time provider.</param>
    public sealed class CreateVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        ICreateVehicleOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<CreateVehicleInput>
    {
        /// <inheritdoc />
        public async Task Execute(CreateVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (await vehicleRepository.ExistsWithLicensePlate(input.LicensePlate))
            {
                outputPort.ConflictHandle($"A vehicle with license plate {input.LicensePlate} already exists in the fleet.");
                return;
            }

            var vehicle = Vehicle.Create(
                input.LicensePlate,
                input.Brand,
                input.Model,
                input.ManufactureDate,
                timeProvider.GetUtcNow().UtcDateTime);

            vehicleRepository.Add(vehicle);

            try
            {
                await unitOfWork.Save();
            }
            catch (ConflictException ex)
            {
                // Another request registered the same plate between the check and the save.
                outputPort.ConflictHandle(ex.Message);
                return;
            }

            outputPort.StandardHandle(new CreateVehicleOutput(vehicle));
        }
    }
}
