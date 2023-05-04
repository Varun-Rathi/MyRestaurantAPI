using Azure.Storage.Blobs;
using CodeFirstRestaurantAPI.Models;
using CodeFirstRestaurantAPI.Utilities;
using Microsoft.EntityFrameworkCore;

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

        Task<Dish> IDishService<Dish, int>.DeleteDish(int DishId)
        {
            throw new NotImplementedException();
        }



        /*      async  task<dish> idishservice<dish, int>.deletedish(int dishid)
                {

                        var dishtodelete = await ctx.dishes.include(d => d.category).firstordefaultasync(d => d.id == dishid);
                        if (dishtodelete == null)
                        {
                            throw new exception($"dish with id {dishid} not found");
                        }

                        dishtodelete.category.dishes.remove(dishtodelete);
                       ctx.dishes.remove(dishtodelete);
                        await ctx.savechangesasync();

                        return dishtodelete;
                    }*/



        async Task<Dish> IDishService<Dish, int>.GetDishById(int DishId)
        {
            return await ctx.Dishes.FindAsync(DishId) ?? null;
        }

        async Task<List<Dish>> IDishService<Dish, int>.GetDishesByCategory(int CategoryId)
        {
            var results = (from dish in ctx.Dishes
                           join category in ctx.CategoryDishes
                           on CategoryId equals category.CategoryId
                           where category.DishId == dish.DishId
                           select dish).ToList();
            

            return results ?? null;
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
