using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Model.SharedKernel
{
    public class Location : ValueObject
    {
        private const int MinCoordValue = 1;
        private const int MaxCoordValue = 10;

        public int X { get; }

        public int Y { get; }

        public Location(int x, int y)
        {
            if (x < MinCoordValue || x > MaxCoordValue)
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Value must be between {MinCoordValue} and {MaxCoordValue} inclusive.");
            }
            if (y < MinCoordValue || y > MaxCoordValue)
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"Value must be between {MinCoordValue} and {MaxCoordValue} inclusive.");
            }
            X = x;
            Y = y;
        }

        public int Distance(Location other)
        {
            _ = other ?? throw new ArgumentNullException(nameof(other));
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public static Location GetRandomLocation() => new (
            x: Random.Shared.Next(MinCoordValue, MaxCoordValue + 1),
            y: Random.Shared.Next(MinCoordValue, MaxCoordValue + 1));

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }
}
