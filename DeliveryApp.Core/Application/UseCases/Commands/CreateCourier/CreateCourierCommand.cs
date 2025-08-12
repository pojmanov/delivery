using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateCourier
{
    /// <summary>
    /// Команда создания курьера
    /// </summary>
    public class CreateCourierCommand : IRequest
    {
        private const int MinSpeed = 1;

        /// <summary>
        /// Имя курьера
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Скорость
        /// </summary>
        public int Speed { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="speed"></param>
        public CreateCourierCommand(string name, int speed)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
            Speed = speed >= MinSpeed ? speed : throw new ArgumentOutOfRangeException(nameof(speed));
        }
    }
}
