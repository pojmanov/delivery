using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using System;
using System.Xml.Linq;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class CourierTest
    {
        [Fact]
        public void NewCourierWithInvalidId()
        {
            Action act = () => new Courier(courierId: Guid.Empty, "Иван", 10, Location.GetRandomLocation());
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("   ")]
        public void NewCourierWithInvalidName(string name)
        {
            Action act = () => new Courier(Guid.NewGuid(), name, 10, Location.GetRandomLocation());
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(-10)]
        [InlineData(-1)]
        [InlineData(0)]
        public void NewCourierWithInvalidSpeed(int speed)
        {
            Action act = () => new Courier(Guid.NewGuid(), "Иван", speed, Location.GetRandomLocation());
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NewCourierWithInvalidLocalion()
        {
            Action act = () => new Courier(Guid.NewGuid(), "Иван", 10, location: null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NewCourierWithValidParams()
        {
            Guid id = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Location location = Location.GetRandomLocation();

            Courier courier = new Courier(id, name, speed, location);
            courier.Should().NotBeNull();
            courier.Id.Should().Be(id);
            courier.Name.Should().Be(name);
            courier.Speed.Should().Be(speed);
            courier.Location.Should().Be(location);
            courier.StoragePlaces.Should().NotBeNull();
            courier.StoragePlaces.Count.Should().Be(1);
        }

        [Fact]
        public void AddStoragePlace()
        {
            Guid id = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Location location = Location.GetRandomLocation();

            Courier courier = new Courier(id, name, speed, location);
            courier.StoragePlaces.Should().NotBeNull();
            courier.StoragePlaces.Count.Should().Be(1);

            string placeName = "багажник";
            int placeVolume = 25;
            courier.AddStoragePlace(placeName, placeVolume);
            courier.StoragePlaces.Count.Should().Be(2);
        }

        [Fact]
        public void CanTakeOrder()
        {
            Guid orderId = Guid.NewGuid();
            int volume = 1;
            Order order = new Order(orderId, Location.GetRandomLocation(), volume);

            Guid courierId = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Courier courier = new Courier(courierId, name, speed, Location.GetRandomLocation());

            bool result = courier.CanTakeOrder(order);
            result.Should().BeTrue();
        }

        [Fact]
        public void CantTakeBigOrder()
        {
            Guid orderId = Guid.NewGuid();
            int volume = 100; // больше чем сумка по умолчанию у курьера
            Order order = new Order(orderId, Location.GetRandomLocation(), volume);

            Guid courierId = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Courier courier = new Courier(courierId, name, speed, Location.GetRandomLocation());

            bool result = courier.CanTakeOrder(order);
            result.Should().BeFalse();
        }

        [Fact]
        public void CanTakeBigOrderAfterAddStoragePlace()
        {
            Guid orderId = Guid.NewGuid();
            int orderVolume = 100; // больше чем сумка по умолчанию у курьера
            Order order = new Order(orderId, Location.GetRandomLocation(), orderVolume);

            Guid courierId = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Courier courier = new Courier(courierId, name, speed, Location.GetRandomLocation());

            bool result = courier.CanTakeOrder(order);
            result.Should().BeFalse();

            courier.AddStoragePlace("Прицеп", orderVolume + 1);
            courier.StoragePlaces.Count.Should().Be(2);

            result = courier.CanTakeOrder(order);
            result.Should().BeTrue();
        }

        [Fact]
        public void TakeNullOrder()
        {
            Guid courierId = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Courier courier = new Courier(courierId, name, speed, Location.GetRandomLocation());
            Action act = () => courier.TakeOrder(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void TakeOrder()
        {
            Guid orderId = Guid.NewGuid();
            int orderVolume = 1;
            Order order = new Order(orderId, Location.GetRandomLocation(), orderVolume);

            Guid courierId = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Courier courier = new Courier(courierId, name, speed, Location.GetRandomLocation());

            order.Status.Should().Be(OrderStatus.Created);
            Action act = () => courier.TakeOrder(order);
            act.Should().NotThrow();

            order.CourierId.Should().Be(courierId);
            order.Status.Should().Be(OrderStatus.Assigned);
        }

        [Fact]
        public void TakeOrdeTwice()
        {
            Guid orderId = Guid.NewGuid();
            int orderVolume = 1;
            Order order = new Order(orderId, Location.GetRandomLocation(), orderVolume);

            Guid courierId = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Courier courier = new Courier(courierId, name, speed, Location.GetRandomLocation());

            order.Status.Should().Be(OrderStatus.Created);
            Action act = () => courier.TakeOrder(order);
            act.Should().NotThrow();

            order.CourierId.Should().Be(courierId);
            order.Status.Should().Be(OrderStatus.Assigned);

            Action act2 = () => courier.TakeOrder(order);
            act.Should().Throw<CourierException>();
        }

        [Fact]
        public void СompleteNullOrder()
        {
            Guid courierId = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Courier courier = new Courier(courierId, name, speed, Location.GetRandomLocation());
            Action act = () => courier.СompleteOrder(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void СompleteOrderInInvalidStatus()
        {
            Guid orderId = Guid.NewGuid();
            int orderVolume = 1;
            Order order = new Order(orderId, Location.GetRandomLocation(), orderVolume);

            Guid courierId = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Courier courier = new Courier(courierId, name, speed, Location.GetRandomLocation());
            Action act = () => courier.СompleteOrder(order);
            act.Should().Throw<CourierException>();
        }

        [Fact]
        public void СompleteOrder()
        {
            Guid orderId = Guid.NewGuid();
            int orderVolume = 1;
            Order order = new Order(orderId, Location.GetRandomLocation(), orderVolume);

            Guid courierId = Guid.NewGuid();
            string name = "Петр";
            int speed = 41;
            Courier courier = new Courier(courierId, name, speed, Location.GetRandomLocation());

            order.Status.Should().Be(OrderStatus.Created);
            Action act = () => courier.TakeOrder(order);
            act.Should().NotThrow();
            order.Status.Should().Be(OrderStatus.Assigned);
            order.CourierId.Should().Be(courierId);

            Action act2 = () => courier.СompleteOrder(order);
            act2.Should().NotThrow();
            order.Status.Should().Be(OrderStatus.Completed);
        }
    }
}
