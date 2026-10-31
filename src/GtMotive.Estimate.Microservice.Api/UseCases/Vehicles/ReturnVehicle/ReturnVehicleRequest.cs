using System;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ReturnVehicle
{
    public sealed record ReturnVehicleRequest(Guid VehicleId) : IRequest<IWebApiPresenter>;
}
