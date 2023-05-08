using Azure.Storage.Blobs;
using CodeFirstRestaurantAPI.Models;
using CodeFirstRestaurantAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstRestaurantAPI.Services
{
    public class CategoryService : ICategoryService<Category, int>
    {

        private readonly RestaurantAppContext ctx;
        private readonly IConfiguration configuration;

        public CategoryService(RestaurantAppContext ctx, IConfiguration configuration)
        {
            this.ctx = ctx;
            this.configuration = configuration;
        }

        // Done
        async  Task<Category> ICategoryService<Category, int>.CreateWithId(Category obj, int MenuId)
        {

            var menuExists = await ctx.Menus.FindAsync(MenuId);
            if (menuExists == null)
                return null;

            obj.CategoryImage = GenerateFilePath.GenerateFileName(obj.CategoryImageFile.FileName);
            
            if (obj.CategoryImage == null)
            {
                Console.WriteLine("Path generation failed");
            }
            else
            {
                // Helps connecting and managing this .net appcliation with azure storage account.
                BlobContainerClient container = new BlobContainerClient(configuration.GetConnectionString("AzureContainerString"), configuration["ContainerName"]);
                try
                {
                    BlobClient blobDisplayImg = container.GetBlobClient(obj.CategoryImage);

                    using (Stream stream = obj.CategoryImageFile.OpenReadStream())
                    {
                        // THis is where you push the selected image from local to azure container inside the storage account. 
                        blobDisplayImg.Upload(stream);
                    }

                    obj.CategoryImage = blobDisplayImg.Uri.AbsoluteUri;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            try
            {

                var result = await ctx.Categories.AddAsync(obj);
                await ctx.SaveChangesAsync();
                obj.CategoryId = result.Entity.CategoryId;
                var lastMenuCategory = await ctx.MenuCategories.ToListAsync();


                var menuCategoryEntity = new MenuCategory()
                {
                    MenuId = MenuId,
                    CategoryId = obj.CategoryId,
                    DisplayOrder = lastMenuCategory[^1].DisplayOrder + 1,
                };
                var assignCategoryToMenu = await ctx.MenuCategories.AddAsync(menuCategoryEntity);
                await ctx.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }

            return obj;
        }


        async Task<bool> ICategoryService<Category, int>.DeleteAsync(int CategoryId)
        {
            var ifCategoryExists = await ctx.Categories.FindAsync(CategoryId);
            if (ifCategoryExists == null) return false;
            
            var menuCategoryMappings = (from menuCategory in ctx.MenuCategories
                                        where menuCategory.CategoryId == CategoryId
                                        select menuCategory).ToListAsync();

            if (menuCategoryMappings.Result.Any())
            {
                menuCategoryMappings.Result.ForEach(obj => obj.IsDeleted = true);
                ctx.SaveChanges(); 
            }
            var categoryDishMappings = (from categoryDish in ctx.CategoryDishes
                                        where categoryDish.CategoryId == CategoryId
                                        select categoryDish).ToListAsync();
            if(categoryDishMappings.Result.Any())
            {
                categoryDishMappings.Result.ForEach(obj =>
                {
                    obj.IsDeleted = true;
                    var dishesToDelete = (from dish in ctx.Dishes
                                          where dish.DishId == obj.DishId
                                          select dish).ToList();
                    dishesToDelete.ForEach(obj => obj.IsDeleted=true);

                });
            }

            ifCategoryExists.IsDeleted = true; 
            await ctx.SaveChangesAsync();
            return true; 

        }

        List<Category> ICategoryService<Category, int>.GetCategoriesByMenuId(int MenuId)
        {
            var results = (from category in ctx.Categories
                           join menu in ctx.MenuCategories
                           on MenuId equals menu.MenuId
                           where menu.CategoryId == category.CategoryId
                           && (menu.IsDeleted == false && category.IsDeleted == false)
                           select category).ToList();
            return results; 

        }

        async Task<Category> ICategoryService<Category, int>.GetCategoryById(int CategoryId)
        {
            var result = await ctx.Categories.Where(obj => obj.CategoryId == CategoryId && obj.IsDeleted == false).FirstOrDefaultAsync();
            if (result == null) return null;
            return result;
        }


        // TODO.
        async Task<Category> ICategoryService<Category, int>.UpdateAsync(Category obj, int id)
        {
            BlobContainerClient container = new BlobContainerClient(configuration.GetConnectionString("AzureContainerString"), configuration["ContainerName"]);

            var categoryToUpdate = await ctx.Categories.FindAsync(id);

            if (categoryToUpdate == null) return null;

            // Update category properties with values from obj parameter
            if(!string.IsNullOrEmpty(categoryToUpdate.CategoryName))
                categoryToUpdate.CategoryName = obj.CategoryName;
            if (!string.IsNullOrEmpty(categoryToUpdate.CategoryDescription))
                categoryToUpdate.CategoryDescription = obj.CategoryDescription;
            if (obj.IsDeleted)
                categoryToUpdate.IsDeleted = obj.IsDeleted;
            else
                categoryToUpdate.IsDeleted = obj.IsDeleted;

            // Update categoryImage if it is not null
            if (obj.CategoryImageFile?.FileName != null)
            {
                var tempImageFilename = categoryToUpdate.CategoryImage.Split($"{configuration["ContainerName"]}/")[1];
                BlobClient blobCategoryImg = container.GetBlobClient(tempImageFilename);

                using Stream stream = obj.CategoryImageFile.OpenReadStream();
                await blobCategoryImg.UploadAsync(stream, overwrite: true);
                categoryToUpdate.CategoryImage = blobCategoryImg.Uri.AbsoluteUri;
            }
            await ctx.SaveChangesAsync();
            return categoryToUpdate;
        }

    }
}