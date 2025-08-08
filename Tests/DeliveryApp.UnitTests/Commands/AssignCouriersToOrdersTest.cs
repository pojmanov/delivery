using DeliveryApp.Core.Application.UseCases.Commands.AssignCouriersToOrders;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;


namespace DeliveryApp.UnitTests.Commands
{
    public class AssignCouriersToOrdersTest
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;

        public AssignCouriersToOrdersTest()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _orderRepository = Substitute.For<IOrderRepository>();
            _courierRepository = Substitute.For<ICourierRepository>();
        }

        [Fact]
        public async Task AllOrdersAreAssigned()
        {
            List<Courier> alaliableCouriers = new List<Courier>()
            {
                new Courier(Guid.NewGuid(), "Иван", 1, Location.GetRandomLocation())
            };

            IDispatchService dispatchService = new DispatchService();

            _orderRepository.GetFirstInCreatedStatusAsync().Returns((Order)null);
            _courierRepository.GetAllAvailable().Returns(alaliableCouriers);
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));

            var command = new AssignCouriersToOrdersCommand();
            var handler = new AssignCouriersToOrdersHandler(_unitOfWork, _orderRepository, _courierRepository, dispatchService);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task NotAvaliableCouriers()
        {
            IDispatchService dispatchService = new DispatchService();

            Order order = new Order(Guid.NewGuid(), Location.GetRandomLocation(), 2);

            _orderRepository.GetFirstInCreatedStatusAsync().Returns(order);
            _courierRepository.GetAllAvailable().Returns((IList<Courier>)null);
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));

            var command = new AssignCouriersToOrdersCommand();
            var handler = new AssignCouriersToOrdersHandler(_unitOfWork, _orderRepository, _courierRepository, dispatchService);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task AssignOrderToCourier()
        {
            IDispatchService dispatchService = new DispatchService();

            Order order = new Order(Guid.NewGuid(), Location.GetRandomLocation(), 2);

            List<Courier> alaliableCouriers = new List<Courier>()
            {
                new Courier(Guid.NewGuid(), "Иван", 1, Location.GetRandomLocation())
            };

            _orderRepository.GetFirstInCreatedStatusAsync().Returns(order);
            _courierRepository.GetAllAvailable().Returns(alaliableCouriers);
            _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));

            var command = new AssignCouriersToOrdersCommand();
            var handler = new AssignCouriersToOrdersHandler(_unitOfWork, _orderRepository, _courierRepository, dispatchService);

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().NotThrowAsync();
            _unitOfWork.Received(1);
        }
    }
}
