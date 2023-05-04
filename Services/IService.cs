using System.Collections;

namespace CodeFirstRestaurantAPI.Services
{
    public interface IService<TEntity, in TPk> where TEntity : class
    {
        Task<TEntity> Get(TPk id);
        Task<IEnumerable<TEntity>> Get();        
        Task<TEntity> Create(TEntity obj);
        Task<IEnumerable<TEntity>> SearchByTerm(string searchTerm);
        Task<bool> DeleteAsync(TPk Id);
        Task<TEntity> UpdateAsync(TEntity obj, TPk id);
        Task<TEntity> CreateWithId(TEntity obj, TPk id);  
    }
}
