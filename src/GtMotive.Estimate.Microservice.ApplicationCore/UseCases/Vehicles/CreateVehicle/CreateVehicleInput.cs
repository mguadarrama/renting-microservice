using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle
{
    /// <summary>
    /// Input message of the Create Vehicle use case.
    /// </summary>
    /// <param name="LicensePlate">License plate.</param>
    /// <param name="Brand">Brand.</param>
    /// <param name="Model">Model.</param>
    /// <param name="ManufactureDate">Manufacture date.</param>
    public sealed record CreateVehicleInput(
        LicensePlate LicensePlate,
        string Brand,
        string Model,
        DateOnly ManufactureDate) : IUseCaseInput;
}
