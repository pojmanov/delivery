using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents
{
    /// <summary>
    /// Событие завершение указанного заказа курьером
    /// </summary>
    public sealed record OrderCompletedDomainEvent : DomainEvent
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public Guid OrderId { get; }

        /// <summary>
        /// Идентификатор курьера
        /// </summary>
        public Guid CourierId {  get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="courierId"></param>
        public OrderCompletedDomainEvent(Guid orderId, Guid courierId)
        {
            OrderId = orderId;
            CourierId = courierId;
        }
    }
}
