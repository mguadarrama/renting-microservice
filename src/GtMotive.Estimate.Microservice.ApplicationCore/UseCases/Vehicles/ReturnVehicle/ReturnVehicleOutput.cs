using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ReturnVehicle
{
    /// <summary>
    /// Output message of the Return Vehicle use case.
    /// </summary>
    /// <param name="Vehicle">The returned vehicle.</param>
    /// <param name="EndedRental">The rental that has just ended.</param>
    /// <param name="ReturnedAt">UTC date and time of the return.</param>
    public sealed record ReturnVehicleOutput(Vehicle Vehicle, Rental EndedRental, DateTime ReturnedAt) : IUseCaseOutput;
}
