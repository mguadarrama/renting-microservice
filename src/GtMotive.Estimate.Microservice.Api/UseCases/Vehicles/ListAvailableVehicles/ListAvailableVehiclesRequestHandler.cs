using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.ListAvailableVehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.Vehicles.ListAvailableVehicles
{
    public sealed class ListAvailableVehiclesRequestHandler(
        IUseCase<ListAvailableVehiclesInput> useCase,
        ListAvailableVehiclesPresenter presenter) : IRequestHandler<ListAvailableVehiclesRequest, IWebApiPresenter>
    {
        public async Task<IWebApiPresenter> Handle(ListAvailableVehiclesRequest request, CancellationToken cancellationToken)
        {
            await useCase.Execute(new ListAvailableVehiclesInput());

            return presenter;
        }
    }
}
