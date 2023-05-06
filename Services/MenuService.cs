using Azure.Storage.Blobs;
using CodeFirstRestaurantAPI.Models;
using CodeFirstRestaurantAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstRestaurantAPI.Services
{
    public class MenuService : IService<Menu, int>
    {
        private readonly RestaurantAppContext ctx;
        private readonly IConfiguration configuration;

        public MenuService(RestaurantAppContext ctx, IConfiguration configuration)
        {
            this.ctx = ctx; 
            this.configuration = configuration; 
        }

        async Task<Menu> IService<Menu, int>.Create(Menu obj)
        {
            obj.MenuImagePath = GenerateFilePath.GenerateFileName(obj.MenuImage.FileName);  

            if(obj.MenuImagePath == null)
            {
                Console.WriteLine("Path generation failed");
            }
            else
            {
                // Helps connecting and managing this .net appcliation with azure storage account.
                BlobContainerClient container = new BlobContainerClient(configuration.GetConnectionString("AzureContainerString"), configuration["ContainerName"]);
                try
                {
                    BlobClient blobDisplayImg = container.GetBlobClient(obj.MenuImagePath);

                    using (Stream stream  = obj.MenuImage.OpenReadStream())
                    {
                        // THis is where you push the selected image from local to azure container inside the storage account. 
                        blobDisplayImg.Upload(stream);
                    }

                    obj.MenuImagePath = blobDisplayImg.Uri.AbsoluteUri;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            var result = await ctx.Menus.AddAsync(obj);
            if (result == null) return null;
            await ctx.SaveChangesAsync(); 
            return result.Entity;
        }


        async Task<dynamic> IService<Menu, int>.DeleteAsync(int Id)
        {
            //throw new NotImplementedException();

            var result=await ctx.Menus.FindAsync(Id);   


            // menu record not found..
            if (result == null) return false;

            // delete from menucategories
            var categoryMenus = await ctx.MenuCategories.Where(c => c.MenuId == Id && c.IsDeleted == false).ToListAsync();
            if (categoryMenus.Any())
            {
                categoryMenus.ForEach(async obj =>
                {
                    obj.IsDeleted = true;
                    // category deleted from category table; 
                    var categoriesToDelete = (from category in ctx.Categories
                                              where category.CategoryId == obj.CategoryId
                                              select category).ToListAsync();
                    categoriesToDelete.Result.ForEach(category => category.IsDeleted = true);

                    // delete from categoryDish table 
                    var categoryDishesToDelete = (from categoryDish in ctx.CategoryDishes
                                                  where categoryDish.CategoryId == obj.CategoryId
                                                  select categoryDish).ToListAsync();
                    categoryDishesToDelete.Result.ForEach(async catDish =>
                    {
                        catDish.IsDeleted = true;
                        // dishes to delete 
                        var dishesToDelete = (from dish in ctx.Dishes
                                              where dish.DishId == catDish.DishId
                                              select dish).ToListAsync();
                        dishesToDelete.Result.ForEach(dish => dish.IsDeleted = true);
                    });

                });
            }



            result.IsDeleted = true;
            await ctx.SaveChangesAsync();

            // delete operation was successful...
            return true;    

        }

        async Task<Menu> IService<Menu, int>.Get(int id)
        {
            var result=await ctx.Menus.FindAsync(id);

            if (result == null) return null;
            if (result.IsDeleted) return null;
            return result;

        }

        async Task<IEnumerable<Menu>> IService<Menu, int>.Get()
        {
            var results = await ctx.Menus.Where(obj => obj.IsDeleted == false).ToListAsync();
            return results; 
        }


        async Task<Menu> IService<Menu, int>.UpdateAsync(Menu obj,int id)
        {
            BlobContainerClient container = new BlobContainerClient(configuration.GetConnectionString("AzureContainerString"), configuration["ContainerName"]);

            var menuToUpdate = await ctx.Menus.FindAsync(id);

            if (menuToUpdate == null) return null;

            // Update menu properties with values from obj parameter
            menuToUpdate.MenuName = obj.MenuName;
            menuToUpdate.MenuDescription = obj.MenuDescription;

            if (obj.IsDeleted)
                menuToUpdate.IsDeleted = obj.IsDeleted;
            else
                menuToUpdate.IsDeleted = obj.IsDeleted;

            // Update menuImage if it is not null
            if (obj.MenuImage?.FileName != null)
            {
                var tempImageFilename = menuToUpdate.MenuImagePath.Split($"{configuration["ContainerName"]}/")[1];
                BlobClient blobMenuImg = container.GetBlobClient(tempImageFilename);
                
                using Stream stream = obj.MenuImage.OpenReadStream();   
                await blobMenuImg.UploadAsync(stream, overwrite: true);
                menuToUpdate.MenuImagePath = blobMenuImg.Uri.AbsoluteUri;               
            }
            else
            {
                return null; 
            }
            await ctx.SaveChangesAsync();
            return menuToUpdate;
        }
    }
}
