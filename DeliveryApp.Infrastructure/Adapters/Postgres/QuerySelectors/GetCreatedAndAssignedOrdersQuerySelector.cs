using DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;
using DeliveryApp.Core.Application.UseCases.Queries.SharedDto;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using OrderStatus = DeliveryApp.Core.Domain.Model.OrderAggregate.OrderStatus;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.QuerySelectors
{
    /// <summary>
    /// Реализация запроса к БД за незавершенными заказами
    /// </summary>
    public class GetCreatedAndAssignedOrdersQuerySelector : IGetCreatedAndAssignedOrdersQuerySelector
    {
        private const string Sql = @"
            select o.id, o.location_x, o.location_y
            from orders o
            where o.status in (@p1, @p2)";

        private readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="applicationDbContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GetCreatedAndAssignedOrdersQuerySelector(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }

        /// <summary>
        /// Получить незавершенные заказы (Created и Assigned)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<OrderDto>> GetCreatedAndAssignedOrders(CancellationToken cancellationToken)
        {
            var results = new List<OrderDto>();
            var connection = _dbContext.Database.GetDbConnection();

            try
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = Sql;
                command.Parameters.Add(new NpgsqlParameter("p1", OrderStatus.Created) { NpgsqlDbType = NpgsqlDbType.Text });
                command.Parameters.Add(new NpgsqlParameter("p2", OrderStatus.Assigned) { NpgsqlDbType = NpgsqlDbType.Text });

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (reader.Read())
                {
                    results.Add(new OrderDto()
                    {
                        Id = reader.GetGuid(0),
                        Location = new LocationDto
                        {
                            X = reader.GetInt32(1),
                            Y = reader.GetInt32(2)
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
