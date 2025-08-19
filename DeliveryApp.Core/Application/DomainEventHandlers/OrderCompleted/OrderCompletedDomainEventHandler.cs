using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers.OrderCompleted
{
    /// <summary>
    /// Обработчик уведомления для отправки их в шину
    /// </summary>
    public class OrderCompletedDomainEventHandler : INotificationHandler<OrderCompletedDomainEvent>
    {
        private readonly INotificationBusProducer _notificationBus;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="notificationBus"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public OrderCompletedDomainEventHandler(INotificationBusProducer notificationBus)
        {
            _notificationBus = notificationBus ?? throw new ArgumentNullException(nameof(notificationBus));
        }

        /// <summary>
        /// Отправить уведомление
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(notification);

            await _notificationBus.Publish(notification, cancellationToken);
        }
    }
}
