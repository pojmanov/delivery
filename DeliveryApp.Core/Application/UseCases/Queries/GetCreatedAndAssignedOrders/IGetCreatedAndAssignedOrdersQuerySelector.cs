namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    /// <summary>
    /// Интерфейс к прямому запросу в БД
    /// </summary>
    public interface IGetCreatedAndAssignedOrdersQuerySelector
    {
        /// <summary>
        /// Получение всех незавершенных заказов (статус Created и Assigned)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<OrderDto>> GetCreatedAndAssignedOrders(CancellationToken cancellationToken);
    }
}
