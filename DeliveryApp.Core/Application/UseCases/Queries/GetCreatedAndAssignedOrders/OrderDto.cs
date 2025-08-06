using DeliveryApp.Core.Application.UseCases.Queries.SharedDto;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    public class OrderDto
    {
        /// <summary>
        ///     Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Геопозиция (X,Y)
        /// </summary>
        public LocationDto Location { get; set; }
    }
}
