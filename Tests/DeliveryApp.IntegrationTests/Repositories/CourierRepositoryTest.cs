using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class CourierRepositoryTest : IAsyncLifetime
    {
        /// <summary>
        ///     Настройка Postgres из библиотеки TestContainers
        /// </summary>
        /// <remarks>По сути это Docker контейнер с Postgres</remarks>
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("courier")
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
        public async Task AddAndGetByIdNewCourier()
        {
            var courierId = Guid.NewGuid();
            var courier = new Courier(courierId, "Сергей", 1, Location.GetRandomLocation());

            var repository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            await repository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();

            Courier getCourier = await repository.GetAsync(courier.Id);
            getCourier.Should().NotBeNull();
            courier.Should().BeEquivalentTo(getCourier);
        }

        [Fact]
        public async Task UpdateCourierByAddStoragePlace()
        {
            var courierId = Guid.NewGuid();
            var courier = new Courier(courierId, "Сергей", 1, Location.GetRandomLocation());

            var repository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            await repository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();

            // теперь добавляем еще одно место хранения
            courier.AddStoragePlace("Прицеп", 25);
            repository.Update(courier);
            await unitOfWork.SaveChangesAsync();

            Courier getCourier = await repository.GetAsync(courier.Id);
            getCourier.Should().NotBeNull();
            getCourier.StoragePlaces.Count.Should().Be(2);
            courier.Should().BeEquivalentTo(getCourier);
        }

        [Fact]
        public async Task UpdateCourierByTakeOrder()
        {
            var courierId = Guid.NewGuid();
            var courier = new Courier(courierId, "Сергей", 1, Location.GetRandomLocation());

            var repository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            await repository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();

            var orderId = Guid.NewGuid();
            var order = new Order(orderId, Location.GetRandomLocation(), 5);
            courier.TakeOrder(order);
            await unitOfWork.SaveChangesAsync();

            Courier courierFromDb = await repository.GetAsync(courier.Id);
            courierFromDb.Should().NotBeNull();
            courierFromDb.StoragePlaces.Count.Should().Be(1);
            courierFromDb.StoragePlaces[0].IsOccupied().Should().BeTrue();
            courierFromDb.StoragePlaces[0].OrderId.Should().Be(orderId);
            courier.Should().BeEquivalentTo(courierFromDb);
        }


        [Fact]
        public async Task GetAllAvailable()
        {
            var courier1 = new Courier(Guid.NewGuid(), "Сергей", 1, Location.GetRandomLocation());
            var courier2 = new Courier(Guid.NewGuid(), "Петр", 2, Location.GetRandomLocation());
            var courier3 = new Courier(Guid.NewGuid(), "Терентий", 3, Location.GetRandomLocation());
            var courier4 = new Courier(Guid.NewGuid(), "Иван", 5, Location.GetRandomLocation());
            var courier5 = new Courier(Guid.NewGuid(), "Соломон", 2, Location.GetRandomLocation());

            var order1 = new Order(Guid.NewGuid(), Location.GetRandomLocation(), 2);
            var order2 = new Order(Guid.NewGuid(), Location.GetRandomLocation(), 4);
            courier2.TakeOrder(order1);
            courier4.TakeOrder(order2);
            // свободны все кроме courier2 и courier4, т.к. они взяли заказы
            var availableCouriersExpect = new List<Courier>()
            { courier1, courier3, courier5 };

            var repository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            await repository.AddAsync(courier1);
            await repository.AddAsync(courier2);
            await repository.AddAsync(courier3);
            await repository.AddAsync(courier4);
            await repository.AddAsync(courier5);
            await unitOfWork.SaveChangesAsync();

            IEnumerable<Courier> availableCouriersFromDb =  repository.GetAllAvailable();
            availableCouriersExpect.Should().BeEquivalentTo(availableCouriersFromDb);
        }

    }
}
