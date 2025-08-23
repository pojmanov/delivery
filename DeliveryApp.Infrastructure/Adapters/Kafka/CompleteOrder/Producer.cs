using Confluent.Kafka;
using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderStatusChanged;

namespace DeliveryApp.Infrastructure.Adapters.Kafka.CompleteOrder
{
    /// <summary>
    /// Реализация продюсера сообщений о готовности заказа в кафку
    /// </summary>
    public class Producer : INotificationBusProducer
    {
        private readonly string _topic;
        private readonly ProducerConfig _config;

        public Producer(IOptions<Settings> options)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(options.Value.MessageBrokerHost));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(options.Value.OrderStatusChangedTopic));
            _config = new ProducerConfig
            {
                BootstrapServers = options.Value.MessageBrokerHost,
            };
            _topic = options.Value.OrderStatusChangedTopic;
        }

        public async Task Publish(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            OrderStatusChangedIntegrationEvent orderCompletedIntegrationEvent = new ()
            {
                OrderId = notification.OrderId.ToString(),
                OrderStatus = OrderStatus.Completed,
            };

            Message<string, string> message = new Message<string, string>
            {
                Key = notification.OrderId.ToString(),
                Value = JsonConvert.SerializeObject(orderCompletedIntegrationEvent)
            };

            using (var producer = new ProducerBuilder<string, string>(_config).Build())
            {
                await producer.ProduceAsync(_topic, message);
            }
        }
    }
}
