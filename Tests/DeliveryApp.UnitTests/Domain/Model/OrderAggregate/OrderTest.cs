using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate
{
    public class OrderTest
    {
        [Fact]
        public void NewOrderWithInvalidId()
        {
            Action act = () => new Order(orderId: Guid.Empty, Location.GetRandomLocation(), 10);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NewOrderWithInvalidLocalion()
        {
            Action act = () => new Order(orderId: Guid.NewGuid(), location: null, 10);
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-1)]
        [InlineData(0)]
        public void NewOrderWithInvalidVolume(int volume)
        {
            Action act = () => new Order(Guid.NewGuid(), Location.GetRandomLocation(), volume);
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("The volume must be positive. (Parameter 'volume')");
        }

        [Fact]
        public void NewOrderWithValidParams()
        {
            Guid id = Guid.NewGuid();
            Location location = Location.GetRandomLocation();
            int volume = 10;

            Order order = new Order(id, location, volume);
            order.Should().NotBeNull();
            order.Id.Should().Be(id);
            order.Location.Should().Be(location);
            order.Volume.Should().Be(volume);
            order.Status.Should().Be(OrderStatus.Created);
        }
        
        [Fact]
        public void AssignOrderToNullCourier()
        {
            Guid id = Guid.NewGuid();
            Location location = Location.GetRandomLocation();
            int volume = 10;
            
            Order order = new Order(id, location, volume);
            Action act = () => order.Assign(courier: null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AssignOrderToCourierOnce()
        {
            Guid id = Guid.NewGuid();
            Location location = Location.GetRandomLocation();
            int volume = 10;

            Guid courierId = Guid.NewGuid();
            Courier courier = new Courier(courierId, "Иван Иванов", 10, Location.GetRandomLocation());

            Order order = new Order(id, location, volume);
            order.Status.Should().Be(OrderStatus.Created);
            order.Assign(courier);
            order.Status.Should().Be(OrderStatus.Assigned);
            order.CourierId.Should().Be(courierId);
        }

        [Fact]
        public void AssignOrderToCourierTwice()
        {
            Guid id = Guid.NewGuid();
            Location location = Location.GetRandomLocation();
            int volume = 10;

            Guid courierId = Guid.NewGuid();
            Courier courier = new Courier(courierId, "Иван Иванов", 10, Location.GetRandomLocation());

            Order order = new Order(id, location, volume);
            order.Status.Should().Be(OrderStatus.Created);
            order.Assign(courier);
            order.Status.Should().Be(OrderStatus.Assigned);
            order.CourierId.Should().Be(courierId);

            Action act = () => order.Assign(courier); // <== тут будет ошибка
            act.Should().Throw<OrderException>();
        }

        [Fact]
        public void CompleteNotAssignedOrder()
        {
            Guid id = Guid.NewGuid();
            Location location = Location.GetRandomLocation();
            int volume = 10;
            Order order = new Order(id, location, volume);
            order.Status.Should().Be(OrderStatus.Created);

            Action act = () => order.Complete();
            act.Should().Throw<OrderException>();
        }

        [Fact]
        public void CompleteAssignedOrder()
        {
            Guid id = Guid.NewGuid();
            Location location = Location.GetRandomLocation();
            int volume = 10;

            Guid courierId = Guid.NewGuid();
            Courier courier = new Courier(courierId, "Иван Иванов", 10, Location.GetRandomLocation());

            Order order = new Order(id, location, volume);
            order.Status.Should().Be(OrderStatus.Created);
            order.Assign(courier);
            order.Status.Should().Be(OrderStatus.Assigned);
            order.Complete();
            order.Status.Should().Be(OrderStatus.Completed);
        }
    }
}
