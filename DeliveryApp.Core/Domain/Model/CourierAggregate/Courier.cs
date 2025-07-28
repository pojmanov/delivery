using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    /// Курьер
    /// </summary>
    public sealed class Courier : Aggregate<Guid>
    {
        private const int MinSpeed = 1;
        private const string StoragePlaceNameDefault = "Сумка";
        private const int StoragePlaceVolumeDefault = 10;

        /// <summary>
        /// Имя курьера
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Скорость
        /// </summary>
        public int Speed { get; private set; }

        /// <summary>
        /// Местоположение
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Места хранения
        /// </summary>
        public List<StoragePlace> StoragePlaces { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="courierId"></param>
        /// <param name="name"></param>
        /// <param name="speed"></param>
        /// <param name="location"></param>
        public Courier(Guid courierId, string name, int speed, Location location)
        {

            Id = courierId != Guid.Empty ? courierId : throw new ArgumentNullException(nameof(courierId));
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
            Speed = speed >= MinSpeed ? speed : throw new ArgumentOutOfRangeException(nameof(speed), Errors.InvalidSpeedValue);

            StoragePlaces = new List<StoragePlace>()
            {
                new StoragePlace(StoragePlaceNameDefault, StoragePlaceVolumeDefault)
            };
        }

        /// <summary>
        /// Добавить новое место хранения
        /// </summary>
        /// <param name="name"></param>
        /// <param name="totalVolume"></param>
        public void AddStoragePlace(string name, int totalVolume)
        {
            // входяшие параметры не проверяем, они уже проверяются при создании StoragePlace
            StoragePlaces.Add(new StoragePlace(name, totalVolume));
        }

        /// <summary>
        /// Проверка, может ли курьер взять заказ указанного объема
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool CanTakeOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            return CanTakeOrderPrivate(order) != null;
        }

        /// <summary>
        /// Взять заказ указанного объема
        /// </summary>
        /// <param name="orderId">Id заказа</param>
        /// <param name="volume">Требуемый объем</param>
        public void TakeOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            StoragePlace storagePlace = CanTakeOrderPrivate(order) ?? throw new CourierException(Errors.CantStore);
            storagePlace.Store(order.Id, order.Volume);
            order.Assign(this);
        }

        private StoragePlace CanTakeOrderPrivate(Order order)
        {
            if (order.Status != OrderStatus.Created)
            {
                throw new CourierException(Errors.CantStore);
            }

            foreach (StoragePlace storagePlace in StoragePlaces)
            {
                if (storagePlace.CanStore(order.Volume))
                {
                    return storagePlace;
                }
            }
            return null;
        }

        /// <summary>
        /// Завершить заказ
        /// </summary>
        /// <param name="order"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void СompleteOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            if (order.Status != OrderStatus.Assigned)
            {
                throw new CourierException(Errors.CantСompleteOrder);
            }

            foreach (StoragePlace storagePlace in StoragePlaces)
            {
                if (storagePlace.OrderId == order.Id)
                {
                    order.Complete();
                    storagePlace.Clear();
                    return;
                }
            }
            throw new CourierException(Errors.CantСompleteOrder);
        }

        /// <summary>
        /// Предположительное время на пусть до указанного адреса
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public double CalculateTimeToLocation(Location location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            return (double) Location.Distance(location) / Speed;
        }

        private static class Errors
        {
            public const string InvalidSpeedValue = "The speed must be positive.";
            public const string CantСompleteOrder = "The specified order could not be completed.";
            public const string CantStore = "The order could not be placed into storage.";
        }
    }
}
