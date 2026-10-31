using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Output message of the Rent Vehicle use case.
    /// </summary>
    /// <param name="Vehicle">The rented vehicle, including its active rental.</param>
    public sealed record RentVehicleOutput(Vehicle Vehicle) : IUseCaseOutput;
}
