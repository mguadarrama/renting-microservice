using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ListAvailableVehicles
{
    /// <summary>
    /// Lists the vehicles of the fleet that can be rented right now.
    /// Vehicles that have aged out of the fleet are excluded even if they were younger when added.
    /// </summary>
    /// <param name="vehicleRepository">Vehicle repository.</param>
    /// <param name="outputPort">Output port.</param>
    /// <param name="timeProvider">Time provider.</param>
    public sealed class ListAvailableVehiclesUseCase(
        IVehicleRepository vehicleRepository,
        IListAvailableVehiclesOutputPort outputPort,
        TimeProvider timeProvider) : IUseCase<ListAvailableVehiclesInput>
    {
        /// <inheritdoc />
        public async Task Execute(ListAvailableVehiclesInput input)
        {
            var minManufactureDate = Vehicle.MinManufactureDate(timeProvider.GetUtcNow().UtcDateTime);
            var vehicles = await vehicleRepository.GetAvailable(minManufactureDate);

            outputPort.StandardHandle(new ListAvailableVehiclesOutput(vehicles));
        }
    }
}
