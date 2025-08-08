namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers
{
    /// <summary>
    /// Ответ на запрос
    /// </summary>
    public class GetCouriersResponse
    {
        /// <summary>
        /// Курьеры
        /// </summary>
        public List<CourierDto> Couriers { get; init; } = new();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="couriers"></param>
        public GetCouriersResponse(IList<CourierDto> couriers)
        {
            if (couriers?.Count > 0)
            {
                Couriers.AddRange(couriers);
            }
        }
    }
}
