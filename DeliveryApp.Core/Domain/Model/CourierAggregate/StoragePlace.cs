using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    /// Место хранения курьера
    /// </summary>
    public class StoragePlace : Entity<Guid>
    {
        /// <summary>
        /// Минимально допустимый объем
        /// </summary>
        private const int MinVolume = 1;

        /// <summary>
        /// Название места хранения
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Допустимый объем
        /// </summary>
        public int TotalVolume { get; }

        /// <summary>
        /// Идентификатор заказа, который храниться в месте хранения
        /// </summary>
        public Guid? OrderId { get; private set; }

        /// <summary>
        /// Создать место хранения
        /// </summary>
        /// <param name="name">Название места хранения</param>
        /// <param name="totalVolume">Допустимый объем</param>
        public StoragePlace(string name, int totalVolume)
        {
            Id = Guid.NewGuid();
            // надо ли ограничивать длинну имени?
            // Ибо в случае каких-либо fuzzing тестов сюда вполне может дойти строка объемом несколько сот мегабайт
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
            if (totalVolume < MinVolume)
            {
                throw new ArgumentOutOfRangeException(nameof(totalVolume), $"The value cannot be less than {MinVolume}.");
            }
            TotalVolume = totalVolume;
        }

        /// <summary>
        /// Проверка, можем можно ли в текущее место поместить заказ указанного объема
        /// </summary>
        /// <param name="volume">Требуемый объем</param>
        /// <returns></returns>
        public bool CanStore(int volume)
        {
            // тут интересное место, ведь volume так же может быть '<=0'
            // на такой случай можно:
            //   1) сразу возвращать false, т.к. некорретный объем нельзя поместить в хранилище
            //   2) вместо false генерировать исключение, т.к. некорретный значение в value это свидетельство о том, что в логике приложения допущена ошибка 
            // выберу способ с генерацией исключения
            if (volume < MinVolume)
            {
                throw new ArgumentOutOfRangeException(nameof(volume), $"The value cannot be less than {MinVolume}.");
            }

            // нет другого и заказа в влезаем по объему
            return !IsOccupied() && TotalVolume >= volume;
        }

        /// <summary>
        /// Поместить заказ в текущее место хранения
        /// </summary>
        /// <param name="orderId">Id заказа</param>
        /// <param name="volume">Требуемый объем</param>
        public void Store(Guid orderId, int volume)
        {
            // нужно ли проверять на Guid.Empty?
            // с одной стороны это вполне допустимое значение (придержусь этого варианта),
            // с другой стороны, сам по себе как ID он особо не используется
            if (!CanStore(volume)) // volume повторно не проверяю на допустимые значение, т.к. это делается в CanStore
            {
                throw new StoragePlaceException(Errors.CantStore);
            }
            OrderId = orderId;
        }

        /// <summary>
        /// Извлечение (удаление) заказа из места хранения
        /// </summary>
        /// <param name="orderId"></param>
        public void Clear()
        {
            // к этому методу есть вопросы
            // надо ли перед удалением проверять номер заказа, или просто очищаем место и все
            // Если надо, что генериуем в случае если хранилище и так было пустое?
            // Выберу вариант когда не надо проверять, и в случае повторных вызовов просто ничего не происходит
            OrderId = null;
        }

        /// <summary>
        /// Есть ли в текущем месте хранилища какой-либо заказ
        /// </summary>
        /// <returns></returns>
        public bool IsOccupied() => OrderId.HasValue;

        private static class Errors
        {
            public const string CantStore = "The order could not be placed into storage.";
        }
    }
}
