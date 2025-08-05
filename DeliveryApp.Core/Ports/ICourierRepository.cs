using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    /// <summary>
    /// Repository для Aggregate Courier
    /// </summary>
    public interface ICourierRepository : IRepository<Courier>
    {
        /// <summary>
        /// Добавить
        /// </summary>
        /// <param name="courier">Заказ</param>
        /// <returns>Заказ</returns>
        Task AddAsync(Courier courier);

        /// <summary>
        /// Обновить
        /// </summary>
        /// <param name="courier">Заказ</param>
        void Update(Courier courier);

        /// <summary>
        /// Получить
        /// </summary>
        /// <param name="courierId">Идентификатор</param>
        /// <returns>Заказ</returns>
        Task<Courier> GetAsync(Guid courierId);

        /// <summary>
        /// Получить всех свободных курьеров (курьеры, у которых все места хранения свободны)
        /// </summary>
        /// <returns></returns>
        IEnumerable<Courier> GetAllAvailable();

    }
}
