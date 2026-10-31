using System;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.RentVehicle
{
    public sealed record RentVehicleRequest(Guid VehicleId, string CustomerId) : IRequest<IWebApiPresenter>;
}
