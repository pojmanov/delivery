using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    /// <summary>
    /// Команда создания заказа
    /// </summary>
    public class CreateOrderCommand : IRequest
    {
        /// <summary>
        /// Идентификатор корзины
        /// </summary>
        /// <remarks>Id корзины берется за основу при создании Id заказа, они совпадают</remarks>
        public Guid BasketId { get; }

        /// <summary>
        /// Улица
        /// </summary>
        public string Street { get; }

        /// <summary>
        /// Объем заказа
        /// </summary>
        public int Volume { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="basketId"></param>
        /// <param name="street"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CreateOrderCommand(Guid basketId, string street, int volume)
        {
            BasketId = basketId != Guid.Empty ? basketId : throw new ArgumentNullException(nameof(basketId));
            Street = !string.IsNullOrWhiteSpace(street) ? street : throw new ArgumentNullException(nameof(street));
            Volume = volume > 0 ? volume : throw new ArgumentOutOfRangeException(nameof(volume));
        }
    }
}
