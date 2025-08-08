
namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    /// <summary>
    /// Ответ на запрос
    /// </summary>
    public class GetCreatedAndAssignedOrdersResponse
    {
        /// <summary>
        /// Не завершшенные заказы
        /// </summary>
        public List<OrderDto> Orders { get; init; } = new();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="orders"></param>
        public GetCreatedAndAssignedOrdersResponse(IList<OrderDto> orders)
        {
            if (orders?.Count > 0)
            {
                Orders.AddRange(orders);
            }
        }
    }
}
