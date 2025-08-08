using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    /// <summary>
    /// Обработчик запроса на получение всех незавершенных заказов
    /// </summary>
    public class GetCreatedAndAssignedOrdersHandler : IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersResponse>
    {
        private readonly IGetCreatedAndAssignedOrdersQuerySelector _querySelector;
       
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="querySelector"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GetCreatedAndAssignedOrdersHandler(IGetCreatedAndAssignedOrdersQuerySelector querySelector)
        {
            _querySelector = querySelector ?? throw new ArgumentNullException(nameof(querySelector));
        }

        /// <summary>
        /// Выполнить запрос
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<GetCreatedAndAssignedOrdersResponse> Handle(GetCreatedAndAssignedOrdersQuery request, CancellationToken cancellationToken)
        {
            IList<OrderDto> orders = await _querySelector.GetCreatedAndAssignedOrders(cancellationToken);
            return new GetCreatedAndAssignedOrdersResponse(orders);
        }
    }
}
