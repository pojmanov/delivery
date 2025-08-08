using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignCouriersToOrders
{
    /// <summary>
    /// Обработчик команды назначения еще не распределенных заказов на свободных курьеров
    /// </summary>
    public class AssignCouriersToOrdersHandler : IRequestHandler<AssignCouriersToOrdersCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly IDispatchService _dispatchService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="orderRepository"></param>
        /// <param name="courierRepository"></param>
        /// <param name="dispatchService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AssignCouriersToOrdersHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICourierRepository courierRepository, IDispatchService dispatchService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
            _dispatchService = dispatchService ?? throw new ArgumentNullException(nameof(dispatchService));
        }

        /// <summary>
        /// Исполнить команду
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(AssignCouriersToOrdersCommand request, CancellationToken cancellationToken)
        {
            // т.к. команда без параметров, видимо проверять ее на null в таком случае смысла особо не имеет

            // Из описания UseCase в задании
            // Система сама распределяет заказы, она берёт __первый неназначенный заказ__ и ищет самого подходящего курьера.
            Order order = await _orderRepository.GetFirstInCreatedStatusAsync();
            if (order != null)
            {
                IList<Courier> availableCouriers = _courierRepository.GetAllAvailable();
                if (availableCouriers?.Count > 0)
                {
                    Courier selectedCourier = _dispatchService.Dispatch(order, availableCouriers);
                    if (selectedCourier != null)
                    {
                        selectedCourier.TakeOrder(order);
                        // на заказ курьер присваивается внутри TakeOrder
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }
    }
}
