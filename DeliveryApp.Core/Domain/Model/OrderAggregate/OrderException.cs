using DeliveryApp.Core.Exceptions;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate
{
    /// <summary>
    /// Исключение возникающее при нарушении логики Order
    /// </summary>
    public class OrderException : DeliveryException
    {
        public OrderException(string message) : base(message)
        {
        }
    }
}
