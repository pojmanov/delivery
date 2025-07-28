using DeliveryApp.Core.Exceptions;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    /// Исключение возникающее при нарушении логики StoragePlace
    /// </summary>
    public class StoragePlaceException : DeliveryException
    {
        public StoragePlaceException(string message) : base(message)
        {
        }
    }
}
