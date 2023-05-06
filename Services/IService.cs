using System.Collections;

namespace CodeFirstRestaurantAPI.Services
{
    public interface IService<TEntity, in TPk> where TEntity : class
    {
        Task<TEntity> Get(TPk id);
        Task<IEnumerable<TEntity>> Get();        
        Task<TEntity> Create(TEntity obj);
        Task<dynamic> DeleteAsync(TPk Id);
        Task<TEntity> UpdateAsync(TEntity obj, TPk id);
    }
}
