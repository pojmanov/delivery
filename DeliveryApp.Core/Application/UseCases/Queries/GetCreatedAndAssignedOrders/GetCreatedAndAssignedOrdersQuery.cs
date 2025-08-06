using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    /// <summary>
    /// Запрос на получение всех незавершенных заказов (статус Created и Assigned)
    /// </summary>
    public class GetCreatedAndAssignedOrdersQuery : IRequest<GetCreatedAndAssignedOrdersResponse>
    {
    }
}
