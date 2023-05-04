﻿namespace CodeFirstRestaurantAPI.Services
{
    public interface ICategoryService<TEntity, in TPk> where TEntity : class
    {
        // create a category with it's menu id defined in the MenuCategory table;
        Task<TEntity> CreateWithId(TEntity obj, TPk MenuId);
        List<TEntity> GetCategoriesByMenuId(TPk MenuId); // get categories by it's menu id;
        
        Task<TEntity> UpdateAsync(TEntity obj, TPk MenuId, TPk CategoryId); // update the category by it's menu and category id; 
        Task DeleteAsync(TPk MenuId, TPk CategoryId);

    }
}