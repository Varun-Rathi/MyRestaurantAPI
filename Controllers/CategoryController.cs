using CodeFirstRestaurantAPI.Models;
using CodeFirstRestaurantAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstRestaurantAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        
            private readonly ICategoryService<Category, int> ctx;

            public CategoryController(ICategoryService<Category, int> ctx)
            {
                this.ctx = ctx;
            }



            [HttpGet("{MenuId}")]
            [ActionName("GetAllRecords")]
            public async Task<IActionResult> GetCategoriesByMenuId(int MenuId)
            {
                var results = ctx.GetCategoriesByMenuId(MenuId);
                return Ok(results);
            }

            [HttpPost]
            [ActionName("CreateCategory")]
            public async Task<IActionResult> Post([FromForm] Category obj, int MenuId)
            {
                var category = await ctx.CreateWithId(obj, MenuId);
                if (category == null)
                    return NotFound("Menu not found or error with conflicting composite key");
                return Ok(category);
            }


            [HttpGet("{id}")]
            [ActionName("GetCategoryById")]

            public async Task<IActionResult> GetCategoryById(int id)
            {
                var category = await ctx.GetCategoryById(id);

                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }


            [HttpPut("{id}")]
            [ActionName("UpdateById")]
            public async Task<IActionResult> Update([FromForm] Category obj, int id)
            {
            var category = await ctx.UpdateAsync(obj, id);

            if (category == null)
                return NotFound();

            return Ok(category);
            return Ok("Categoruupdated");

            }

        [HttpDelete("{id}")]
        [ActionName("deletebyid")]


        public async Task<IActionResult> Delete(int id)
        {


            var menu = await ctx.DeleteAsync(id);


            //The if (!menu) condition checks whether the result of the DeleteAsync
            //operation in the MenuService was successful or not...........
            if (menu == false)
            {

                // return 404 if record not found..
                return NotFound();
            }


            // return 204 record deleted successfully....
            return NoContent();


        }
    }
}
