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

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="orderRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
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
            Location orderLocation = Location.GetRandomLocation();
            int orderVolume = Random.Shared.Next(1, 21); // от одного до 20

            Order order = new Order(request.BasketId, orderLocation, orderVolume);
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
