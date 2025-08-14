using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    /// <summary>
    /// Обработчик команды создания заказа
    /// </summary>
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IGeoClient _geoClient;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="orderRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IGeoClient geoClient)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _geoClient = geoClient ?? throw new ArgumentNullException(nameof(geoClient));
        }

        /// <summary>
        /// Исполнить команду
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // TODO
            // тут мы должны сначала по ID корзины получить ее агрегат,
            // но т.к. пока у нас этого нет, поэтому рандомно заполним Location и volume
            
            Location orderLocation = await _geoClient.GetLocationAsync(request.Street, cancellationToken);

            Order order = new Order(request.BasketId, orderLocation, request.Volume);
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
