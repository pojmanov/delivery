using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateCourier
{
    /// <summary>
    /// Обработчик команды создания курьера
    /// </summary>
    public class CreateCourierHandler : IRequestHandler<CreateCourierCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourierRepository _courierRepository;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="orderRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CreateCourierHandler(IUnitOfWork unitOfWork, ICourierRepository courierRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        }

        /// <summary>
        /// Исполнить команду
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Handle(CreateCourierCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            Guid courierId = Guid.NewGuid();
            Courier courier = new(courierId, request.Name, request.Speed, Location.GetRandomLocation());
            await _courierRepository.AddAsync(courier);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
