namespace CodeFirstRestaurantAPI.Services
{
    public interface IDishService<TEntity, in TPk> where TEntity : class
    {
        Task<TEntity> GetDishById(TPk DishId);

        Task<List<TEntity>> GetDishesByCategory(TPk CategoryId);

        Task<TEntity> CreateDishByCategory(TPk CategoryId, TEntity dish);
        Task<TEntity> UpdateDish(TEntity obj, TPk DishId);
        Task<dynamic> DeleteDish(TPk DishId);

        Task<IEnumerable<TEntity>> SearchDish(string SearchTerm);

        //Task<string> GetDishWithCategoryAndMenuId(TPk DishId);


        
    }
}
