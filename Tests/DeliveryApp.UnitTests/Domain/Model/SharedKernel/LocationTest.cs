using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.SharedKernel
{
    public class LocationTest
    {

        /// <summary>
        /// Некорректные диапазаноы, с учетом граничных случаев
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        #region InlineData
        // x: -1,0,11,20
        [InlineData(-1,-20)]
        [InlineData(-1, 0)] 
        [InlineData(-1, 1)]
        [InlineData(-1, 5)]
        [InlineData(-1, 10)]
        [InlineData(-1, 11)]
        [InlineData(-1, 50)]
        [InlineData(0, -20)]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 5)]
        [InlineData(0, 10)]
        [InlineData(0, 11)]
        [InlineData(0, 50)]
        [InlineData(11, -20)]
        [InlineData(11, 0)]
        [InlineData(11, 1)]
        [InlineData(11, 5)]
        [InlineData(11, 10)]
        [InlineData(11, 11)]
        [InlineData(11, 50)]
        [InlineData(20, -20)]
        [InlineData(20, 0)]
        [InlineData(20, 1)]
        [InlineData(20, 5)]
        [InlineData(20, 10)]
        [InlineData(20, 11)]
        [InlineData(20, 50)]

        // y: -1,0,11,20
        [InlineData(-20, -1)]
        [InlineData(0, -1)]
        [InlineData(1, -1)]
        [InlineData(5, -1)]
        [InlineData(10, -1)]
        [InlineData(11, -1)]
        [InlineData(20, -1)]
        [InlineData(-20, 0)]
        [InlineData(1, 0)]
        [InlineData(5, 0)]
        [InlineData(10, 0)]
        [InlineData(-20, 11)]
        [InlineData(1, 11)]
        [InlineData(5, 11)]
        [InlineData(10, 11)]
        [InlineData(-20, 20)]
        [InlineData(0, 20)]
        [InlineData(1, 20)]
        [InlineData(5, 20)]
        [InlineData(10, 20)]
        [InlineData(11, 20)]
        [InlineData(20, 20)]
        #endregion InlineData
        public void OutOfRangeXOrY(int x, int y)
        {
            Action act = () => new Location(x, y);
            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("Value must be between 1 and 10 inclusive. (Parameter '?')");
        }

        /// <summary>
        /// Корректные диапазоны
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        #region InlineData
        [InlineData(1, 1)]
        [InlineData(1, 5)]
        [InlineData(1, 10)]
        [InlineData(5, 1)]
        [InlineData(5, 5)]
        [InlineData(5, 10)]
        [InlineData(10, 1)]
        [InlineData(10, 5)]
        [InlineData(10, 10)]
        #endregion InlineData
        public void CorrectXAndYRange(int x, int y)
        {
            // в теории можно для таких небольших допустимых значений прогнать через цикл все возможные комбинации
            Location loc = new (x, y);
            loc.X.Should().BeInRange(1, 10);
            loc.Y.Should().BeInRange(1, 10);
        }

        [Fact]
        public void GetRandomLocation()
        {
            Location loc = Location.GetRandomLocation();
            loc.X.Should().BeInRange(1, 10);
            loc.Y.Should().BeInRange(1, 10);
        }

        [Fact]
        public void LocationsIsEqual()
        {
            Location first = new (2,7);
            Location second = new (2,7);
            bool result = first == second;
            result.Should().BeTrue();
        }

        [Fact]
        public void LocationsIsNotEqual()
        {
            Location first = new(2, 7);
            Location second = new(2, 5);
            bool result = first == second;
            result.Should().BeFalse();
        }

        [Fact]
        public void LocationsDistanceIsZero()
        {
            Location first = new(2, 4);
            Location second = new(2, 4);
            int distance = first.Distance(second);
            distance.Should().Be(0);
        }

        [Fact]
        public void LocationsDistance()
        {
            // тест аналогичный изображению из урока
            Location first = new(2, 6);
            Location second = new(4, 9);
            int distance = first.Distance(second);
            distance.Should().Be(5);
        }

        [Fact]
        public void LocationsMaxDistance()
        {
            Location first = new(1, 1);
            Location second = new(10, 10);
            int distance = first.Distance(second);
            distance.Should().Be(18);
        }
    }
}
