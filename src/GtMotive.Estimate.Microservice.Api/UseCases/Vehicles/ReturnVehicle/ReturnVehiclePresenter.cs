using System;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ReturnVehicle;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ReturnVehicle
{
    public sealed class ReturnVehiclePresenter : IWebApiPresenter, IReturnVehicleOutputPort
    {
        public IActionResult ActionResult { get; private set; }

        public void StandardHandle(ReturnVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            ActionResult = new OkObjectResult(new RentalResponse(
                response.Vehicle.Id.Value,
                response.Vehicle.LicensePlate.Value,
                response.EndedRental.CustomerId.Value,
                response.EndedRental.StartedAt,
                response.ReturnedAt));
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
