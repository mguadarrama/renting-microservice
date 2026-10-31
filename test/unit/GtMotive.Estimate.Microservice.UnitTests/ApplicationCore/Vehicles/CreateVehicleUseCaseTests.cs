using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Vehicles
{
    public sealed class CreateVehicleUseCaseTests
    {
        private static readonly DateTimeOffset Now = new(2026, 10, 28, 10, 0, 0, TimeSpan.Zero);

        private readonly Mock<IVehicleRepository> _repository = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<ICreateVehicleOutputPort> _outputPort = new();
        private readonly CreateVehicleUseCase _useCase;

        public CreateVehicleUseCaseTests()
        {
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.Setup(t => t.GetUtcNow()).Returns(Now);

            _useCase = new CreateVehicleUseCase(_repository.Object, _unitOfWork.Object, _outputPort.Object, timeProvider.Object);
        }

        [Fact]
        public async Task ExecuteWithVehicleExactlyFiveYearsOldAddsItToTheFleet()
        {
            var input = NewInput(new DateOnly(2021, 10, 28));

            await _useCase.Execute(input);

            _repository.Verify(r => r.Add(It.Is<Vehicle>(v => v.LicensePlate == input.LicensePlate)), Times.Once);
            _unitOfWork.Verify(u => u.Save(), Times.Once);
            _outputPort.Verify(o => o.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteWithVehicleOlderThanFiveYearsIsRejectedAndNothingIsSaved()
        {
            var input = NewInput(new DateOnly(2021, 10, 27));

            var act = () => _useCase.Execute(input);

            await act.Should().ThrowAsync<VehicleTooOldException>();
            _repository.Verify(r => r.Add(It.IsAny<Vehicle>()), Times.Never);
            _unitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public async Task ExecuteWithLicensePlateAlreadyInTheFleetReportsConflict()
        {
            var input = NewInput(new DateOnly(2024, 1, 1));
            _repository.Setup(r => r.ExistsWithLicensePlate(input.LicensePlate)).ReturnsAsync(true);

            await _useCase.Execute(input);

            _outputPort.Verify(o => o.ConflictHandle(It.IsAny<string>()), Times.Once);
            _repository.Verify(r => r.Add(It.IsAny<Vehicle>()), Times.Never);
            _unitOfWork.Verify(u => u.Save(), Times.Never);
        }

        private static CreateVehicleInput NewInput(DateOnly manufactureDate)
        {
            return new CreateVehicleInput(new LicensePlate("1234-abc"), "Seat", "Ibiza", manufactureDate);
        }
    }
}
