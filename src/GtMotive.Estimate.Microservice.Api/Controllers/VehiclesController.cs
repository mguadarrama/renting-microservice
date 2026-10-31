using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ListAvailableVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ReturnVehicle;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// Fleet of the renting company.
    /// </summary>
    [ApiController]
    [Route("api/vehicles")]
    [Produces("application/json")]
    public sealed class VehiclesController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Adds a vehicle to the fleet. Vehicles manufactured more than 5 years ago are rejected.
        /// </summary>
        /// <param name="request">Vehicle data.</param>
        /// <returns>The vehicle added to the fleet.</returns>
        [HttpPost]
        [ProducesResponseType<VehicleResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody][Required] CreateVehicleRequest request)
        {
            var presenter = await mediator.Send(request);
            return presenter.ActionResult;
        }

        /// <summary>
        /// Lists the vehicles of the fleet that can be rented right now.
        /// </summary>
        /// <returns>The available vehicles.</returns>
        [HttpGet("available")]
        [ProducesResponseType<IReadOnlyCollection<VehicleResponse>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAvailable()
        {
            var presenter = await mediator.Send(new ListAvailableVehiclesRequest());
            return presenter.ActionResult;
        }

        /// <summary>
        /// Rents a vehicle to a customer. A customer cannot rent more than one vehicle at the same time.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <param name="body">Customer renting the vehicle.</param>
        /// <returns>The rental.</returns>
        [HttpPost("{vehicleId:guid}/rental")]
        [ProducesResponseType<RentalResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Rent(Guid vehicleId, [FromBody][Required] RentVehicleBody body)
        {
            ArgumentNullException.ThrowIfNull(body);

            var presenter = await mediator.Send(new RentVehicleRequest(vehicleId, body.CustomerId));
            return presenter.ActionResult;
        }

        /// <summary>
        /// Returns a rented vehicle to the fleet.
        /// </summary>
        /// <param name="vehicleId">Vehicle identifier.</param>
        /// <returns>The rental that has just ended.</returns>
        [HttpDelete("{vehicleId:guid}/rental")]
        [ProducesResponseType<RentalResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Return(Guid vehicleId)
        {
            var presenter = await mediator.Send(new ReturnVehicleRequest(vehicleId));
            return presenter.ActionResult;
        }
    }
}
