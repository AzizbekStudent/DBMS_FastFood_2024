namespace FastFood.DAL.Interface
{
    // Students ID: 00013836, 00014725, 00014896
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);

        Task<int> CreateAsync(T entity);

        Task<int> UpdateAsync(T entity);

        Task<int> DeleteAsync(T entity);
    }
}
