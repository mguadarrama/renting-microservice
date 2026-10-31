using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ListAvailableVehicles
{
    public sealed record ListAvailableVehiclesRequest : IRequest<IWebApiPresenter>;
}
