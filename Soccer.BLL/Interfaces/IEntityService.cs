namespace Soccer.BLL.Interfaces
{
    /// <summary>
    /// Генеричний інтерфейс для базових CRUD-операцій над DTO будь-якої сутності
    /// </summary>
    /// <typeparam name="TDto">Тип DTO, для якого реалізується сервіс</typeparam>
    public interface IEntityService<TDto> where TDto : class
    {
        Task Create(TDto dto);
        Task Update(TDto dto);
        Task Delete(int id);
        Task<TDto> Get(int id);
        Task<IEnumerable<TDto>> GetAll();
    }
}

