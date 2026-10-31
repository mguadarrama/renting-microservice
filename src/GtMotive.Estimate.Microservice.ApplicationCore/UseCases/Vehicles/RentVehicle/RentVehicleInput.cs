using GtMotive.Estimate.Microservice.Domain.Customers;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle
{
    /// <summary>
    /// Input message of the Rent Vehicle use case.
    /// </summary>
    /// <param name="VehicleId">Vehicle to rent.</param>
    /// <param name="CustomerId">Customer renting the vehicle.</param>
    public sealed record RentVehicleInput(VehicleId VehicleId, CustomerId CustomerId) : IUseCaseInput;
}
