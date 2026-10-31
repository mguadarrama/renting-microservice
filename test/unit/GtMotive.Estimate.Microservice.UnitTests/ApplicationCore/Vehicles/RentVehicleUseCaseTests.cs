using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Vehicles.RentVehicle;
using GtMotive.Estimate.Microservice.Domain.Customers;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore.Vehicles
{
    public sealed class RentVehicleUseCaseTests
    {
        private static readonly DateTimeOffset Now = new(2026, 10, 28, 10, 0, 0, TimeSpan.Zero);

        private readonly Mock<IVehicleRepository> _repository = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IRentVehicleOutputPort> _outputPort = new();
        private readonly RentVehicleUseCase _useCase;

        public RentVehicleUseCaseTests()
        {
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.Setup(t => t.GetUtcNow()).Returns(Now);

            _useCase = new RentVehicleUseCase(_repository.Object, _unitOfWork.Object, _outputPort.Object, timeProvider.Object);
        }

        [Fact]
        public async Task ExecuteWhenVehicleIsAvailableRentsItToTheCustomerAndSaves()
        {
            var vehicle = NewVehicle();
            var customer = new CustomerId("12345678Z");
            _repository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, customer));

            vehicle.CurrentRental.Should().Be(new Rental(customer, Now.UtcDateTime));
            _repository.Verify(r => r.Update(vehicle), Times.Once);
            _unitOfWork.Verify(u => u.Save(), Times.Once);
            _outputPort.Verify(o => o.StandardHandle(It.Is<RentVehicleOutput>(output => output.Vehicle == vehicle)), Times.Once);
        }

        [Fact]
        public async Task ExecuteWhenCustomerAlreadyHasARentalReportsConflictAndDoesNotSave()
        {
            var vehicle = NewVehicle();
            var customer = new CustomerId("12345678Z");
            _repository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);
            _repository.Setup(r => r.HasActiveRental(customer)).ReturnsAsync(true);

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, customer));

            vehicle.IsRented.Should().BeFalse();
            _outputPort.Verify(o => o.ConflictHandle(It.IsAny<string>()), Times.Once);
            _outputPort.Verify(o => o.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Never);
            _unitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public async Task ExecuteWhenVehicleIsAlreadyRentedReportsConflictAndDoesNotSave()
        {
            var vehicle = NewVehicle();
            vehicle.Rent(new CustomerId("OTHER1"), Now.UtcDateTime.AddDays(-1));
            _repository.Setup(r => r.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, new CustomerId("12345678Z")));

            vehicle.CurrentRental.CustomerId.Should().Be(new CustomerId("OTHER1"));
            _outputPort.Verify(o => o.ConflictHandle(It.IsAny<string>()), Times.Once);
            _unitOfWork.Verify(u => u.Save(), Times.Never);
        }

        [Fact]
        public async Task ExecuteWhenVehicleDoesNotExistReportsNotFound()
        {
            var vehicleId = VehicleId.New();
            _repository.Setup(r => r.GetById(vehicleId)).ReturnsAsync((Vehicle)null);

            await _useCase.Execute(new RentVehicleInput(vehicleId, new CustomerId("12345678Z")));

            _outputPort.Verify(o => o.NotFoundHandle(It.IsAny<string>()), Times.Once);
            _repository.Verify(r => r.HasActiveRental(It.IsAny<CustomerId>()), Times.Never);
            _unitOfWork.Verify(u => u.Save(), Times.Never);
        }

        private static Vehicle NewVehicle()
        {
            return Vehicle.Create(new LicensePlate("1234ABC"), "Seat", "Ibiza", new DateOnly(2024, 3, 1), Now.UtcDateTime);
        }
    }
}
