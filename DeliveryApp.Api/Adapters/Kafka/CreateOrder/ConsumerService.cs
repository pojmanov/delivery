using BasketConfirmed;
using Confluent.Kafka;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Exceptions;
using DeliveryApp.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DeliveryApp.Api.Adapters.Kafka.CreateOrder
{
    /// <summary>
    /// Consumer для канала создания заказа
    /// </summary>
    public class ConsumerService : BackgroundService
    {
        private const string ConsumerGroup = "DeliveryConsumerGroup";

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _topic;
        private readonly TimeSpan _delayTime = TimeSpan.FromSeconds(1);

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="scopeFactory"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ConsumerService(IServiceScopeFactory scopeFactory, IOptions<Settings> options)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(options.Value.MessageBrokerHost));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(nameof(options.Value.BasketConfirmedTopic));
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = options.Value.MessageBrokerHost,
                GroupId = ConsumerGroup,
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            _topic = options.Value.BasketConfirmedTopic;
        }

        /// <summary>
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(_delayTime, stoppingToken);
                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult.IsPartitionEOF)
                    {
                        continue;
                    }

                    BasketConfirmedIntegrationEvent basketConfirmedIntegrationEvent = JsonConvert.DeserializeObject<BasketConfirmedIntegrationEvent>(consumeResult.Message.Value);

                    Guid basketId = Guid.Parse(basketConfirmedIntegrationEvent.BasketId);
                    string street = basketConfirmedIntegrationEvent.Address.Street;
                    int volume = basketConfirmedIntegrationEvent.Volume;

                    using (IServiceScope scope = _scopeFactory.CreateScope())
                    {
                        IMediator mediator = scope.ServiceProvider.GetService<IMediator>();
                        try
                        {
                            CreateOrderCommand command = new CreateOrderCommand(basketId, street, volume);
                            await mediator.Send(command);
                        }
                        catch (DeliveryException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    try
                    {
                        _consumer.StoreOffset(consumeResult);
                    }
                    catch (KafkaException e)
                    {
                        Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
