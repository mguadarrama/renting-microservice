using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Output message of the Create Vehicle use case.
    /// </summary>
    /// <param name="Vehicle">The vehicle added to the fleet.</param>
    public sealed record CreateVehicleOutput(Vehicle Vehicle) : IUseCaseOutput;
}
