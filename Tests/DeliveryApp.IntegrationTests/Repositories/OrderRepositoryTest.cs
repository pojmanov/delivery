using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class OrderRepositoryTest : IAsyncLifetime
    {
        /// <summary>
        ///     Настройка Postgres из библиотеки TestContainers
        /// </summary>
        /// <remarks>По сути это Docker контейнер с Postgres</remarks>
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("order")
            .WithUsername("username")
            .WithPassword("secret")
            .WithCleanUp(true)
            .Build();

        private ApplicationDbContext _context;

        /// <summary>
        ///     Инициализируем окружение
        /// </summary>
        /// <remarks>Вызывается перед каждым тестом</remarks>
        public async Task InitializeAsync()
        {
            //Стартуем БД (библиотека TestContainers запускает Docker контейнер с Postgres)
            await _postgreSqlContainer.StartAsync();

            // string cs = "Host=localhost;Port=5432;Database=order;Username=postgres;Password=postgres";

            //Накатываем миграции и справочники
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                    _postgreSqlContainer.GetConnectionString(),
                    sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); })
                .Options;
            _context = new ApplicationDbContext(contextOptions);
            _context.Database.EnsureCreated();
            // _context.Database.Migrate(); // при Migrate почему-то не создаются таблицы
        }

        /// <summary>
        ///     Уничтожаем окружение
        /// </summary>
        /// <remarks>Вызывается после каждого теста</remarks>
        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public async Task AddNewOrder()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, Location.GetRandomLocation(), 5);

            //Act
            var orderRepository = new OrderRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            await orderRepository.AddAsync(order);
            await unitOfWork.SaveChangesAsync();

            //Assert
            var orderFromDb = await orderRepository.GetAsync(order.Id);
            orderFromDb.Should().NotBeNull();
            order.Should().BeEquivalentTo(orderFromDb);
        }

        [Fact]
        public async Task UpdateOrder()
        {
            //Arrange
            var courierId = Guid.NewGuid();
            var courier = new Courier(courierId, "Pedestrian", 1, new Location(1, 1));

            var orderId = Guid.NewGuid();
            var order = new Order(orderId, Location.GetRandomLocation(), 5);

            var orderRepository = new OrderRepository(_context);
            await orderRepository.AddAsync(order);

            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveChangesAsync();

            //Act
            Action orderAssignToCourierResult = () => order.Assign(courier);
            orderAssignToCourierResult.Should().NotThrow();
            orderRepository.Update(order);
            await unitOfWork.SaveChangesAsync();

            //Assert
            var orderFromDb = await orderRepository.GetAsync(order.Id);
            orderFromDb.Should().NotBeNull();
            order.Should().BeEquivalentTo(orderFromDb);
        }

        [Fact]
        public async Task CanGetById()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(orderId, Location.GetRandomLocation(), 5);

            //Act
            var orderRepository = new OrderRepository(_context);
            await orderRepository.AddAsync(order);

            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveChangesAsync();

            //Assert
            var orderFromDb = await orderRepository.GetAsync(order.Id);
            orderFromDb.Should().NotBeNull();
            order.Should().BeEquivalentTo(orderFromDb);
        }

        [Fact]
        public async Task CanGetFirstInCreatedStatus()
        {
            //Arrange
            var courierId = Guid.NewGuid();
            var courier = new Courier(courierId, "Pedestrian", 1, Location.GetRandomLocation());

            var order1Id = Guid.NewGuid();
            var order1 = new Order(order1Id, Location.GetRandomLocation(), 5);
            Action orderAssignToCourierResult = () => order1.Assign(courier);
            orderAssignToCourierResult.Should().NotThrow();

            var order2Id = Guid.NewGuid();
            var order2 = new Order(order2Id, Location.GetRandomLocation(), 5);

            var orderRepository = new OrderRepository(_context);
            await orderRepository.AddAsync(order1);
            await orderRepository.AddAsync(order2);

            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveChangesAsync();

            //Act
            Order orderFromDb = await orderRepository.GetFirstInCreatedStatusAsync();

            //Assert
            orderFromDb.Should().NotBeNull();
            order2.Should().BeEquivalentTo(orderFromDb);
        }

        [Fact]
        public async Task WhenSaveOrdersWithSameLocationThenShouldCorrectSavingAtDatabase()
        {
            // Arrange
            var firstOrderId = Guid.NewGuid();
            var secondOrderId = Guid.NewGuid();

            var firstOrder = new Order(firstOrderId, new Location(1, 1), 5);
            var secondOrder = new Order(secondOrderId, new Location(1, 1), 5);
            var orderRepository = new OrderRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            await orderRepository.AddAsync(firstOrder);
            await orderRepository.AddAsync(secondOrder);
            await unitOfWork.SaveChangesAsync();

            // Act
            var first = await orderRepository.GetAsync(firstOrderId);
            var second = await orderRepository.GetAsync(secondOrderId);

            // Assert
            var location = new Location(1, 1);
            first.Location.Should().Be(location);
            second.Location.Should().Be(location);
        }
    }
}
