using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    public sealed class RentVehicleTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        [Fact]
        public async Task CustomerCannotRentASecondVehicleWhileHavingOne()
        {
            var customerId = NewCode();
            var first = await CreateVehicle();
            var second = await CreateVehicle();

            (await Send<RentVehicleRequest>(new(first, customerId))).Should().BeOfType<OkObjectResult>();
            (await Send<RentVehicleRequest>(new(second, customerId))).Should().BeOfType<ConflictObjectResult>();
        }

        private static string NewCode()
        {
            return Guid.NewGuid().ToString("N")[..8];
        }

        private async Task<Guid> CreateVehicle()
        {
            var result = await Send(new CreateVehicleRequest
            {
                LicensePlate = NewCode(),
                Brand = "Seat",
                Model = "Ibiza",
                ManufactureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
            });

            return ((VehicleResponse)((ObjectResult)result).Value).Id;
        }

        private async Task<IActionResult> Send<TRequest>(TRequest request)
            where TRequest : IRequest<IWebApiPresenter>
        {
            IActionResult result = null;
            await Fixture.UsingHandlerForRequestResponse<TRequest, IWebApiPresenter>(async handler =>
                result = (await handler.Handle(request, default)).ActionResult);
            return result;
        }
    }
}
