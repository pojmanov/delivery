namespace DeliveryApp.Core.Exceptions
{
    /// <summary>
    /// Базовое исключение при нарушении бизнес-логики продукта
    /// </summary>
    public abstract class DeliveryException : Exception
    {
        public DeliveryException()
        {
        }

        public DeliveryException(string message) : base(message)
        {
        }
    }
}
