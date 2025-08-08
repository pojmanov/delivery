using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignCouriersToOrders
{
    /// <summary>
    /// Команда назначения еще не распределенных заказов на свободных курьеров
    /// </summary>
    public class AssignCouriersToOrdersCommand : IRequest
    {
    }
}
