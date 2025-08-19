using DeliveryApp.Core.Application.UseCases.Commands.CreateCourier;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;
using DeliveryApp.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;


namespace DeliveryApp.Api.Adapters.Http
{
    /// <summary>
    /// Контроллер http методов
    /// </summary>
    public class DeliveryController : DefaultApiController
    {
        private const string Street = "Строителей";
        private readonly IMediator _mediator;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="mediator"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Добавить курьера
        /// </summary>
        /// <param name="newCourier"></param>
        /// <returns></returns>
        public override async Task<IActionResult> CreateCourier([FromBody] NewCourier newCourier)
        {
            CreateCourierCommand command;
            try
            {
                command = new(newCourier.Name, newCourier.Speed);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                await _mediator.Send(command);
            }
            catch (DeliveryException ex)
            {
                return Conflict(ex.Message);
            }
            return Ok();
        }

        /// <summary>
        /// Создать заказ
        /// </summary>
        /// <returns></returns>
        public override async Task<IActionResult> CreateOrder()
        {
            Guid orderId = Guid.NewGuid();
            int orderVolume = Random.Shared.Next(1, 21); // от одного до 20
            CreateOrderCommand command = new CreateOrderCommand(orderId, Street, orderVolume);
            try
            {
                await _mediator.Send(command);
            }
            catch (DeliveryException ex)
            {
                return Conflict(ex.Message);
            }
            return Ok();
        }

        /// <summary>
        /// Получить всех курьеров
        /// </summary>
        /// <returns></returns>
        public override async Task<IActionResult> GetCouriers()
        {
            GetBusyCouriersQuery query = new GetBusyCouriersQuery();
            GetCouriersResponse response;
            try
            {
                response = await _mediator.Send(query);
            }
            catch (DeliveryException ex)
            {
                return Conflict(ex.Message);
            }

            List<Courier> result = new List<Courier>();
            if (response?.Couriers.Count > 0)
            {
                foreach (var item in response?.Couriers)
                {
                    result.Add(new Courier()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Location = new Location()
                        {
                            X = item.Location.X,
                            Y = item.Location.Y,
                        }
                    });
                }
            }

            return Ok(result);
        }

        /// <summary>
        /// Получить все незавершенные заказы
        /// </summary>
        /// <returns></returns>
        public override async Task<IActionResult> GetOrders()
        {
            GetCreatedAndAssignedOrdersQuery query = new GetCreatedAndAssignedOrdersQuery();
            GetCreatedAndAssignedOrdersResponse response;
            try
            {
                response = await _mediator.Send(query);
            }
            catch (DeliveryException ex)
            {
                return Conflict(ex.Message);
            }

            List<Order> result = new List<Order>();
            if (response?.Orders.Count > 0)
            {
                foreach (var item in response?.Orders)
                {
                    result.Add(new Order()
                    {
                        Id = item.Id,
                        Location = new Location()
                        {
                            X = item.Location.X,
                            Y = item.Location.Y,
                        }
                    });
                }
            }

            return Ok(result);
        }
    }
}
