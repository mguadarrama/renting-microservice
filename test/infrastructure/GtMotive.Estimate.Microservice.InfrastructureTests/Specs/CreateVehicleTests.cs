using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Specs
{
    public sealed class CreateVehicleTests(GenericInfrastructureTestServerFixture fixture) : InfrastructureTestBase(fixture)
    {
        [Fact]
        public async Task PostVehicleWithMissingFieldsReturnsBadRequest()
        {
            using var client = Fixture.Server.CreateClient();

            var response = await client.PostAsJsonAsync(new Uri("/api/vehicles", UriKind.Relative), new { brand = "Seat" });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            problem.Errors.Keys.Should().Contain(["LicensePlate", "Model", "ManufactureDate"]);
        }
    }
}
