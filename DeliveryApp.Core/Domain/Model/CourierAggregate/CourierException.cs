using DeliveryApp.Core.Exceptions;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    /// Исключение возникающее при нарушении логики Courier
    /// </summary>
    public class CourierException : DeliveryException
    {
        public CourierException(string message) : base(message)
        {
        }
    }
}
