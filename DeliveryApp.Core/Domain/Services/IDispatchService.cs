using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services
{
    /// <summary>
    /// Интерфейс сервиса
    /// </summary>
    public interface IDispatchService
    {
        /// <summary>
        /// Подбор подходящего курьера для указанного заказа
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="couriers">Список курьеров - претендентов</param>
        /// <returns></returns>
        Courier Dispatch(Order order, IList<Courier> couriers);
    }
}
