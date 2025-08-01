using DeliveryApp.Core.Exceptions;

namespace DeliveryApp.Core.Domain.Services
{
    public class DispatchServiceException : DeliveryException
    {
        public DispatchServiceException(string message) : base(message)
        {
        }
    }
}
