using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers
{
    /// <summary>
    /// Обработчик запроса на получение занятых курьеров
    /// </summary>
    public class GetBusyCouriersHandler : IRequestHandler<GetBusyCouriersQuery, GetCouriersResponse>
    {
        private readonly IGetBusyCouriersQuerySelector _querySelector;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="querySelector"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GetBusyCouriersHandler(IGetBusyCouriersQuerySelector querySelector)
        {
            _querySelector = querySelector ?? throw new ArgumentNullException(nameof(querySelector));
        }

        /// <summary>
        /// Выполнить запрос
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<GetCouriersResponse> Handle(GetBusyCouriersQuery request, CancellationToken cancellationToken)
        {
            IList<CourierDto> couriers = await _querySelector.GetBusyCouriersAsync(cancellationToken);
            return new GetCouriersResponse(couriers);
        }
    }
}
