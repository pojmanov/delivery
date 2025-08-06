using DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.SharedDto;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.QuerySelectors
{
    /// <summary>
    /// Реализация запроса к БД за занятыми курьерами
    /// </summary>
    public class GetBusyCouriersQuerySelector : IGetBusyCouriersQuerySelector
    {
        private const string Sql = @"
            select c.id, c.name, c.location_x, c.location_y
            from couriers c
            inner join storage_places sp ON sp.courier_id = c.id
            where sp.order_id is not null";

        private readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="applicationDbContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GetBusyCouriersQuerySelector(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }

        /// <summary>
        /// Получить занятых курьеров
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<CourierDto>> GetBusyCouriersAsync(CancellationToken cancellationToken)
        {
            var results = new List<CourierDto>();
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = Sql;

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (reader.Read())
                {
                    results.Add(new CourierDto()
                    {
                        Id = reader.GetGuid(0),
                        Name =reader.GetString(1),
                        Location = new LocationDto
                        {
                            X = reader.GetInt32(2),
                            Y = reader.GetInt32(3)
                        }
                    });
                }
            }
            finally
            {
                connection.Close();
            }

            return results;
        }
    }
}
