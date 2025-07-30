using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

/// <summary>
///     Заказ
/// </summary>
public sealed class Order : Aggregate<Guid>
{
    private const int MinVolume = 1;

    /// <summary>
    ///     Местоположение, куда нужно доставить заказ
    /// </summary>
    public Location Location { get; private set; }

    /// <summary>
    ///     Объем
    /// </summary>
    public int Volume { get; private set; }

    /// <summary>
    ///     Статус
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    ///     Идентификатор исполнителя (курьера)
    /// </summary>
    public Guid? CourierId { get; private set; }

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="orderId">Идентификатор заказа</param>
    /// <param name="location">Геопозиция</param>
    /// <param name="volume">Объем</param>
    public Order(Guid orderId, Location location, int volume)
    {
        Id = orderId != Guid.Empty ? orderId : throw new ArgumentNullException(nameof(orderId));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        Volume = volume >= MinVolume ? volume : throw new ArgumentOutOfRangeException(nameof(volume), Errors.InvalidVolumeValue);
        Status = OrderStatus.Created;
    }

    /// <summary>
    /// Назначить заказ на курьера
    /// </summary>
    /// <param name="courier">Курьер</param>
    /// <returns>Результат</returns>
    public void Assign(Courier courier)
    {
        CourierId = courier != null ? courier.Id : throw new ArgumentNullException(nameof(courier));
        if (Status != OrderStatus.Created)
        {
            throw new OrderException(Errors.CantAssignAlreadyAssignedOrder);
        }
        Status = OrderStatus.Assigned;
    }

    /// <summary>
    /// Завершить выполнение заказа
    /// </summary>
    /// <returns>Результат</returns>
    public void Complete()
    {
        if ((Status != OrderStatus.Assigned) || (CourierId == null))
        {
            throw new OrderException(Errors.CantCompleteNotAssignedOrder);
        }
        Status = OrderStatus.Completed;
    }

    private static class Errors
    {
        public const string InvalidVolumeValue = "The volume must be positive.";
        public const string CantAssignAlreadyAssignedOrder = "You cannot assign an order that is already assigned.";
        public const string CantCompleteNotAssignedOrder = "Unable to complete unassigned order";
    }
}