using System.Collections.Generic;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ListAvailableVehicles
{
    /// <summary>
    /// Output message of the List Available Vehicles use case.
    /// </summary>
    /// <param name="Vehicles">Vehicles that can be rented right now.</param>
    public sealed record ListAvailableVehiclesOutput(IReadOnlyCollection<Vehicle> Vehicles) : IUseCaseOutput;
}
