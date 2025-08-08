using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    /// <summary>
    /// Репозитория для Courier
    /// </summary>
    public class CourierRepository : ICourierRepository
    {
        private readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="dbContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CourierRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Добавить
        /// </summary>
        /// <param name="courier">Заказ</param>
        /// <returns>Заказ</returns>
        public async Task AddAsync(Courier courier)
        {
            await _dbContext.Couriers.AddAsync(courier);
        }

        /// <summary>
        /// Обновить
        /// </summary>
        /// <param name="courier">Заказ</param>
        public void Update(Courier courier)
        {
            _dbContext.Couriers.Update(courier);
        }

        /// <summary>
        /// Получить
        /// </summary>
        /// <param name="courierId">Идентификатор</param>
        /// <returns>Заказ</returns>
        public async Task<Courier> GetAsync(Guid courierId)
        {
            return await _dbContext.Couriers
                .SingleOrDefaultAsync(c => c.Id == courierId);
        }

        /// <summary>
        /// Получить всех свободных курьеров (курьеры, у которых все места хранения свободны)
        /// </summary>
        /// <returns></returns>
        public IList<Courier> GetAllAvailable()
        {
            return _dbContext.Couriers
               .Where(c => c.StoragePlaces.All(sp => sp.OrderId == null)).ToList();
        }
    }
}
