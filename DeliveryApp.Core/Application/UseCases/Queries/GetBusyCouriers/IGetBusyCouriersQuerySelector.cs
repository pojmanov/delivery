namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers
{
    /// <summary>
    /// Интерфейс к прямому запросу в БД за занятыми курьерами
    /// </summary>
    public interface IGetBusyCouriersQuerySelector

    {
        /// <summary>
        /// Получить занятых курьеров
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IList<CourierDto>> GetBusyCouriersAsync(CancellationToken cancellationToken);
    }
}
