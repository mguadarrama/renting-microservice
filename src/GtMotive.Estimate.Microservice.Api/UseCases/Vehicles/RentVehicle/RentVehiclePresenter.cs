using System;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle
{
    public sealed class RentVehiclePresenter : IWebApiPresenter, IRentVehicleOutputPort
    {
        public IActionResult ActionResult { get; private set; }

        public void StandardHandle(RentVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            var vehicle = response.Vehicle;
            var rental = vehicle.CurrentRental;

            ActionResult = new OkObjectResult(new RentalResponse(
                vehicle.Id.Value,
                vehicle.LicensePlate.Value,
                rental.CustomerId.Value,
                rental.StartedAt,
                returnedAt: null));
        }

        public void NotFoundHandle(string message)
        {
            ActionResult = ProblemResults.NotFound(message);
        }

        public void ConflictHandle(string message)
        {
            ActionResult = ProblemResults.Conflict(message);
        }
    }
}
