using DeliveryApp.Core.Domain.Model.CourierAggregate;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class StoragePlaceTest
    {
        [Theory]
        [InlineData(null, 5)]
        [InlineData("", 5)]
        [InlineData(" ", 5)]
        [InlineData("  ", 5)]
        public void InvalidNameButCorrectVolume(string name, int totalVolume)
        {
            Action act = () => new StoragePlace(name, totalVolume);
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("Bag", -5)]
        [InlineData("Bag", -1)]
        [InlineData("Bag", 0)]
        public void CorrectNameButInvalidVolume(string name, int totalVolume)
        {
            Action act = () => new StoragePlace(name, totalVolume);
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("The value cannot be less than 1. (Parameter 'totalVolume')");
        }

        [Theory]
        [InlineData("Bag", 1)]
        [InlineData("Backpack", 2)]
        [InlineData("Roof rack", 10)]
        [InlineData("Trailer", 25)]
        public void CorrectNameAndVolume(string name, int totalVolume)
        {
            StoragePlace place = new (name, totalVolume);
            place.Name.Should().Be(name);
            place.TotalVolume.Should().Be(totalVolume);
            place.OrderId.Should().NotHaveValue();
            place.IsOccupied().Should().BeFalse();
        }

        [Fact]
        public void CompareTwoStoragePlaceWithSameNameAndVolume()
        {
            StoragePlace first = new("Bag", 7);
            StoragePlace second = new("Bag", 7);
            bool result = first == second;
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(-1)]
        [InlineData(0)]
        public void CanStoreWithInvalidValue(int value)
        {
            StoragePlace place = new("Bag", 7);
            Action act = () => place.CanStore(value);
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("The value cannot be less than 1. (Parameter 'volume')");
        }

        [Fact]
        public void CanStoreWhenValueGraterTotalValue()
        {
            StoragePlace place = new("Bag", 7);
            place.OrderId.Should().NotHaveValue();
            place.IsOccupied().Should().BeFalse();
            bool result = place.CanStore(8);
            result.Should().BeFalse();
        }

        [Fact]
        public void CanStoreWhenAllCorrect()
        {
            int orderVolume = 5;
            StoragePlace place = new("Bag", 7);

            place.OrderId.Should().NotHaveValue();
            place.IsOccupied().Should().BeFalse();
            place.CanStore(orderVolume).Should().BeTrue();
        }

        [Fact]
        public void CanStoreWhenOccupied()
        {
            Guid orderId = Guid.NewGuid();
            int orderVolume = 5;
            StoragePlace place = new("Bag", 70);

            place.CanStore(orderVolume).Should().BeTrue();
            place.Store(orderId, orderVolume);

            place.OrderId.Should().HaveValue();
            place.OrderId.Should().Be(orderId);
            place.IsOccupied().Should().BeTrue();
            place.CanStore(orderVolume).Should().BeFalse();
        }

        [Fact]
        public void TryStoreTwice()
        {
            Guid orderId = Guid.NewGuid();
            int orderVolume = 5;
            StoragePlace place = new("Bag", 70);

            place.CanStore(orderVolume).Should().BeTrue();
            place.Store(orderId, orderVolume);
            place.OrderId.Should().HaveValue();
            place.OrderId.Should().Be(orderId);
            place.IsOccupied().Should().BeTrue();
            Action act = () => place.Store(orderId, orderVolume);
            act.Should().Throw<Exception>()
                .WithMessage("The order could not be placed into storage.");
        }

        [Fact]
        public void StoreAndClear()
        {
            Guid orderId = Guid.NewGuid();
            int orderVolume = 5;
            StoragePlace place = new("Bag", 70);
            place.Store(orderId, orderVolume); // <==
            place.OrderId.Should().Be(orderId);
            place.IsOccupied().Should().BeTrue();
            place.Clear();  // <==
            place.OrderId.Should().NotHaveValue();
            place.IsOccupied().Should().BeFalse();
        }
    }
}
