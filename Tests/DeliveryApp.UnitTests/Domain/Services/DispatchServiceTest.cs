using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services
{
    public class DispatchServiceTest
    {
        [Fact]
        public void DispatchNullOrder()
        {
            DispatchService dispatchService = new DispatchService();
            List<Courier> couriers = new List<Courier>()
            {
                new Courier(Guid.NewGuid(), "Иван", speed: 1, Location.GetRandomLocation()),
                new Courier(Guid.NewGuid(), "Сергей", speed: 2, Location.GetRandomLocation()),
                new Courier(Guid.NewGuid(), "Пётр", speed: 3, Location.GetRandomLocation()),
            };
            Action act = () => dispatchService.Dispatch(null, couriers);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DispatchOrderWithNullCouriers()
        {
            DispatchService dispatchService = new DispatchService();
            Order order = new Order(Guid.NewGuid(), Location.GetRandomLocation(), volume: 10);
            Action act = () => dispatchService.Dispatch(order, null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DispatchOrderWithNoCouriers()
        {
            DispatchService dispatchService = new DispatchService();
            Order order = new Order(Guid.NewGuid(), Location.GetRandomLocation(), volume: 10);
            Action act = () => dispatchService.Dispatch(order, new List<Courier>());
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DispatchOrderAllCouriersSameSpeed()
        {
            int speed = 2;
            Courier c1 = new Courier(Guid.NewGuid(), "Иван", speed, new Location(8, 8));
            Courier c2 = new Courier(Guid.NewGuid(), "Сергей", speed, new Location(3, 3)); // ближе всех, должен быть выбран
            Courier c3 = new Courier(Guid.NewGuid(), "Пётр", speed, new Location(6, 6));

            Order order = new Order(Guid.NewGuid(), new Location(1, 1), volume: 2); // объем < 10 который по умолчанию есть у курьеров
            List<Courier> couriers = new List<Courier>() { c1, c2, c3 };
            DispatchService dispatchService = new DispatchService();
            Courier winner = dispatchService.Dispatch(order, couriers);
            winner.Should().Be(c2);
        }

        [Fact]
        public void DispatchOrderToFasterCourier()
        {
            Courier c1 = new Courier(Guid.NewGuid(), "Иван", speed: 1, new Location(4, 4));
            Courier c2 = new Courier(Guid.NewGuid(), "Сергей", speed: 2, new Location(6, 6)); // чуть дальше чем первый, но быстрее и должен быть выбран
            Courier c3 = new Courier(Guid.NewGuid(), "Пётр", speed: 2, new Location(7, 7));

            Order order = new Order(Guid.NewGuid(), new Location(1, 1), volume: 2); // объем < 10 который по умолчанию есть у курьеров
            List<Courier> couriers = new List<Courier>() { c1, c2, c3 };
            DispatchService dispatchService = new DispatchService();
            Courier winner = dispatchService.Dispatch(order, couriers);
            winner.Should().Be(c2);
        }

        [Fact]
        public void DispatchVeryBigOrderButCourierHaveSmallBag()
        {
            List<Courier> couriers = new List<Courier>()
            {
                new Courier(Guid.NewGuid(), "Иван", speed: 1, Location.GetRandomLocation()),
                new Courier(Guid.NewGuid(), "Сергей", speed: 2, Location.GetRandomLocation()),
                new Courier(Guid.NewGuid(), "Пётр", speed: 3, Location.GetRandomLocation()),
            };

            Order order = new Order(Guid.NewGuid(), new Location(1, 1), volume: 20); // объем > 10 который по умолчанию есть у курьеров
            DispatchService dispatchService = new DispatchService();
            Courier winner = dispatchService.Dispatch(order, couriers);
            winner.Should().BeNull();
        }

        [Fact]
        public void DispatchVeryBigOrderToCourierWithBigBag()
        {
            Courier c1 = new Courier(Guid.NewGuid(), "Иван", speed: 1, new Location(3, 3));
            Courier c2 = new Courier(Guid.NewGuid(), "Сергей", speed: 3, new Location(5, 5));
            Courier c3 = new Courier(Guid.NewGuid(), "Пётр", speed: 1, new Location(3, 3));
            Courier c4 = new Courier(Guid.NewGuid(), "Николай", speed: 3, new Location(5, 5)); // быстрый и с нужным местом для заказа

            int orderVolume = 20;
            c3.AddStoragePlace("Прицеп", orderVolume + 1);
            c4.AddStoragePlace("Прицеп", orderVolume + 1);

            Order order = new Order(Guid.NewGuid(), new Location(1, 1), orderVolume); // объем > 10 который по умолчанию есть у курьеров
            List<Courier> couriers = new List<Courier>() { c1, c2, c3, c4 };

            DispatchService dispatchService = new DispatchService();
            Courier winner = dispatchService.Dispatch(order, couriers);
            winner.Should().Be(c4);
        }

        [Fact]
        public void DispatchAlreadyDispatchedOrder()
        {
            Courier c1 = new Courier(Guid.NewGuid(), "Иван", speed: 1, new Location(8, 8));
            Courier c2 = new Courier(Guid.NewGuid(), "Сергей", speed : 2, new Location(3, 3));
            Courier c3 = new Courier(Guid.NewGuid(), "Пётр", speed : 3, new Location(6, 6));
            List<Courier> couriers = new List<Courier>() { c1, c2, c3 };

            Order order = new Order(Guid.NewGuid(), new Location(1, 1), volume : 2);
            order.Status.Should().Be(OrderStatus.Created);
            c2.CanTakeOrder(order).Should().BeTrue();
            c2.TakeOrder(order);
            order.Status.Should().Be(OrderStatus.Assigned);

            DispatchService dispatchService = new DispatchService();
            Action act = () => dispatchService.Dispatch(order, couriers);
            act.Should().Throw<DispatchServiceException>().WithMessage("The order is in an invalid state.");
        }
    }
}
