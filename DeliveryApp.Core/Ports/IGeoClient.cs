using DeliveryApp.Core.Domain.Model.SharedKernel;

namespace DeliveryApp.Core.Ports
{
    /// <summary>
    /// Интерфейс клиента к службе GEO
    /// </summary>
    public interface IGeoClient
    {
        /// <summary>
        /// Получить координаты по адресу
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Location> GetLocationAsync(string address, CancellationToken cancellationToken);
    }
}
