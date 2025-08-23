using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

namespace DeliveryApp.Core.Ports
{
    /// <summary>
    /// Интерфейс для отправки уведомления о завершении заказа
    /// </summary>
    public interface INotificationBusProducer
    {
        /// <summary>
        /// Опубликовать уведомление о завершении заказа
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Publish(OrderCompletedDomainEvent notification, CancellationToken cancellationToken);
    }
}
