using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers
{

    /// <summary>
    /// Обработчик команды на перемещение курьеров
    /// </summary>
    public class MoveCouriersHandler : IRequestHandler<MoveCouriersCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="courierRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MoveCouriersHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICourierRepository courierRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        }

        /// <summary>
        /// Исполнить команду
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task Handle(MoveCouriersCommand request, CancellationToken cancellationToken)
        {
            // т.к. команда без параметров, видимо проверять ее на null в таком случае смысла особо не имеет

            // по хорошему мы можем за один раз получить всех занятых курьеров из их репозитория,
            // но у нас нет такого метода, поэтому через "костыль"
            // т.е. сначала получаем назначенные заказы, а уже через них получаем курьера

            IList<Order> assignedOrders = _orderRepository.GetAllInAssignedStatus();
            foreach (Order order in assignedOrders)
            {
                if (!order.CourierId.HasValue)
                {
                    throw new InvalidOperationException($"Order '{order.Id}' in invalid state");
                }
                Courier courier = await _courierRepository.GetAsync(order.CourierId.Value);
                courier.Move(order.Location);

                if (order.Location == courier.Location)
                {
                    courier.СompleteOrder(order);
                    order.Complete();
                }
                _courierRepository.Update(courier);
                _orderRepository.Update(order);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
