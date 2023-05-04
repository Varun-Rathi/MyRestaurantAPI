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

        Task<Menu> IService<Menu, int>.CreateWithId(Menu obj, int id)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IService<Menu, int>.DeleteAsync(int Id)
        {
            //throw new NotImplementedException();

            var result=await ctx.Menus.FindAsync(Id);   


            // menu record not found..
            if (result == null) return false;


            var categories = await ctx.MenuCategories.Where(c => c.MenuId == Id).ToListAsync();
             ctx.MenuCategories.RemoveRange(categories);

            // delete the menu record from database..

            ctx.Menus.Remove(result);
            await ctx.SaveChangesAsync();

            // delete operation was successful...
            return true;    

        }

        async Task<Menu> IService<Menu, int>.Get(int id)
        {
            var result=await ctx.Menus.FindAsync(id);

            if (result == null) return null;
            return result;

        }

        async Task<IEnumerable<Menu>> IService<Menu, int>.Get()
        {
            var results = await ctx.Menus.ToListAsync();
            return results; 
        }

        Task<IEnumerable<Menu>> IService<Menu, int>.SearchByTerm(string searchTerm)
        {
            throw new NotImplementedException();
        }

        async Task<Menu> IService<Menu, int>.UpdateAsync(Menu obj,int id)
        {
            BlobContainerClient container = new BlobContainerClient(configuration.GetConnectionString("AzureContainerString"), configuration["ContainerName"]);

            var menuToUpdate = await ctx.Menus.FindAsync(id);

            if (menuToUpdate == null) return null;

            // Update menu properties with values from obj parameter
            menuToUpdate.MenuName = obj.MenuName;
            menuToUpdate.MenuDescription = obj.MenuDescription;

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
