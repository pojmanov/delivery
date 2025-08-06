using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;

namespace DeliveryApp.Core.Domain.Services
{
    /// <summary>
    /// Сервис диспетчеризации (подбора) курьеров
    /// </summary>
    public class DispatchService : IDispatchService
    {
        /// <summary>
        /// Подбор подходящего курьера для указанного заказа
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <param name="couriers">Список курьеров - претендентов</param>
        /// <returns></returns>
        public Courier Dispatch(Order order, IList<Courier> couriers)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            if ((couriers == null) || couriers.Count == 0)
            {
                throw new ArgumentNullException(nameof(couriers));
            }
            if (order.Status != OrderStatus.Created)
            {
                throw new DispatchServiceException(Errors.OrderWrongState);
            }

            Courier winner = null;
            double winnerTimeToOrderLocation = 0;
            foreach (Courier courier in couriers)
            {
                if ((winner == null) && courier.CanTakeOrder(order))
                {
                    winner = courier;
                    winnerTimeToOrderLocation = courier.CalculateTimeToLocation(order.Location);
                }
                else
                {
                    double courierTimeToOrderLocation = courier.CalculateTimeToLocation(order.Location);
                    if (courier.CanTakeOrder(order) && (courierTimeToOrderLocation < winnerTimeToOrderLocation))
                    {
                        winner = courier;
                        winnerTimeToOrderLocation = courierTimeToOrderLocation;
                    }
                }
            }
            return winner;
        }

        private static class Errors
        {
            public const string OrderWrongState = "The order is in an invalid state.";
            public const string CourierNotFound = "Courier for order not found.";
        }
    }
}
