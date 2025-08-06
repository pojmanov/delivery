using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers
{
    /// <summary>
    /// Запрос на получение занятых курьеров
    /// </summary>
    public class GetBusyCouriersQuery : IRequest<GetCouriersResponse>
    {
    }
}
