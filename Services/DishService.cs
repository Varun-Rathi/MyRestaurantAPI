using Azure.Storage.Blobs;
using CodeFirstRestaurantAPI.Models;
using CodeFirstRestaurantAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using System;

namespace CodeFirstRestaurantAPI.Services
{
    public class DishService : IDishService<Dish, int>
    {
        private readonly RestaurantAppContext ctx; 
        private readonly IConfiguration configuration;

        public DishService(RestaurantAppContext ctx, IConfiguration configuration)
        {
            this.ctx = ctx;
            this.configuration = configuration;
        }


       
       

        async Task<Dish> IDishService<Dish, int>.CreateDishByCategory(int CategoryId, Dish dish)
        {
            var ifCategoryExists = await ctx.Categories.FindAsync(CategoryId);
            if (ifCategoryExists == null) return null;

            dish.DishImage = GenerateFilePath.GenerateFileName(dish.DishImageFile.FileName);

            if (dish.DishImage == null) return null;

            BlobContainerClient container = new BlobContainerClient(configuration.GetConnectionString("AzureContainerString"), configuration["ContainerName"]);

            try
            {
                BlobClient blobDishImage = container.GetBlobClient(dish.DishImage);
                using (Stream stream = dish.DishImageFile.OpenReadStream())
                {
                    // THis is where you push the selected image from local to azure container inside the storage account. 
                   await blobDishImage.UploadAsync(stream);
                }

                dish.DishImage = blobDishImage.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            try
            {
                var result = await ctx.Dishes.AddAsync(dish);
                await ctx.SaveChangesAsync();
                dish.DishId = result.Entity.DishId;
                var categoryDish= new CategoryDish()
                {
                    DishId = dish.DishId,
                    CategoryId = CategoryId,
                };
                var assignCategoryToMenu = await ctx.CategoryDishes.AddAsync(categoryDish);
                await ctx.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }

            return dish;

        }

        async Task<dynamic> IDishService<Dish, int>.DeleteDish(int DishId)
        {
            // find dish to soft delete
            var isDishPresent = await ctx.Dishes.FindAsync(DishId) ?? null;
            if (isDishPresent == null) return isDishPresent;
            if (isDishPresent.IsDeleted == true) return new {isDeleted = true};

            // soft delete dish in dishes table
            isDishPresent.IsDeleted = true; 
            await ctx.SaveChangesAsync();
            return isDishPresent.IsDeleted; // return the soft deleted item in the dish table; 


        }

        async Task<Dish> IDishService<Dish, int>.GetDishById(int DishId)
        {
            var result = await ctx.Dishes.Where(obj => obj.DishId.Equals(DishId) && obj.IsDeleted == false).FirstOrDefaultAsync();
            return result;
        }

        async Task<List<Dish>> IDishService<Dish, int>.GetDishesByCategory(int CategoryId)
        {
            var results = (from dish in ctx.Dishes
                           join category in ctx.CategoryDishes
                           on CategoryId equals category.CategoryId
                           where (dish.IsDeleted == false && category.IsDeleted == false) && (category.DishId == dish.DishId)
                           select dish).ToList();

            return results;
        }

        //async Task<string> IDishService<Dish, int>.GetDishWithCategoryAndMenuId(int DishId)
        //{
        //    var result = await ctx.Dishes.FindAsync(DishId) ?? null;
        //    if (result == null) return null;
        //    var category = ctx.CategoryDishes.Where(obj => obj.DishId == DishId).FirstOrDefault();
        //    if (category == null) return null;
        //    var menuId = ctx.MenuCategories.Where (obj => obj.CategoryId == category.CategoryId).FirstOrDefault();
        //    if(menuId == null) return null; 
        //    string redictionUrl = $"/menu/{menuId.MenuId}/category/${category.CategoryId}/dish/${result.DishId}";
        //    return redictionUrl;

        //}

        async Task<IEnumerable<Dish>> IDishService<Dish, int>.SearchDish(string SearchTerm)
        {
            var resultSet = await ctx.Dishes.ToListAsync();
            var results = resultSet.Where(obj => obj.DishName.ToLower().Contains(SearchTerm.ToLower()) && obj.IsDeleted == false);
            return results;
        }



        async Task<Dish> IDishService<Dish, int>.UpdateDish(Dish obj, int id)
        {
            BlobContainerClient container = new BlobContainerClient(configuration.GetConnectionString("AzureContainerString"), configuration["ContainerName"]);

            var dishToUpdate = await ctx.Dishes.FindAsync(id);

            if (dishToUpdate == null) return null;

            // Update category properties with values from obj parameter
            if (obj.DishName != null)
                dishToUpdate.DishName = obj.DishName;
            if (obj.DishDescription != null)
                dishToUpdate.DishDescription = obj.DishDescription;

            if(obj.DishPrice!=null)
                dishToUpdate.DishPrice = obj.DishPrice;
            if(obj.DishNature!=null)
                dishToUpdate.DishNature = obj.DishNature;
            // we shouldn't be alloweing to change flags like this to do soft delte via updation
            if (obj.IsDeleted)
                dishToUpdate.IsDeleted = obj.IsDeleted;
            else
                dishToUpdate.IsDeleted = obj.IsDeleted;

            // Update categoryImage if it is not null
            if (obj.DishImageFile?.FileName != null)
            {
                var tempImageFilename = dishToUpdate.DishImage.Split($"{configuration["ContainerName"]}/")[1];
                if (tempImageFilename == null) return null;
                BlobClient blobDishImg = container.GetBlobClient(tempImageFilename);

                using Stream stream = obj.DishImageFile.OpenReadStream();
                await blobDishImg.UploadAsync(stream, overwrite: true);
                dishToUpdate.DishImage = blobDishImg.Uri.AbsoluteUri;
            }
            else
            {
                return null;
            }
            await ctx.SaveChangesAsync();
            return dishToUpdate;
        }
    }
}
