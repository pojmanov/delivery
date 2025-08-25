namespace DeliveryApp.Infrastructure.Adapters.Postgres.Entities
{
    /// <summary>
    /// Класс для хранения исходящих уведомлений
    /// </summary>
    public class OutboxMessage
    {
        /// <summary>
        /// ID уведомления
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Содержимое уведомления
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime OccurredOnUtc { get; set; }

        /// <summary>
        /// Дата обработки/отправки
        /// </summary>
        public DateTime? ProcessedOnUtc { get; set; }
    }
}
