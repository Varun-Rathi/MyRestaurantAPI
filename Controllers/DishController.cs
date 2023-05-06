using CodeFirstRestaurantAPI.Models;
using CodeFirstRestaurantAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstRestaurantAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService<Dish, int> serv;


        public DishController(IDishService<Dish, int> serv) 
        {
            this.serv = serv;
        }

        [HttpPost]
        [ActionName("createDishByCategory")]
        public async Task<IActionResult> CreateDish(int CategoryId, [FromForm] Dish dish)
        {
            var result = await serv.CreateDishByCategory(CategoryId, dish);
            if (result == null)
                return NoContent();
            return Ok(result);
        }

        [HttpGet("{CategoryId}")]
        [ActionName("GetDishesByCategory")]
        public async Task<IActionResult> GetDishesByCategoryID(int CategoryId)
        {
            var results = await serv.GetDishesByCategory(CategoryId);
            if (results == null || results.Count == 0)
                return NotFound();
            return Ok(results);
        }

        [HttpGet("{DishId}")]
        [ActionName("GetDishesById")]

        public async Task<IActionResult> GetDishesById(int DishId)
        {
            var results = await serv.GetDishById(DishId);

            if(results == null)
                return NotFound();
            return Ok(results);
        }


        [HttpPut("{id}")]
        [ActionName("UpdateByDishId")]


        public async Task<IActionResult> Update([FromForm] Dish obj, int id)
        {
            var result = await serv.UpdateDish(obj,id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        /*public async Task<IActionResult> Delete(int id)
         * 
         
*/






        [HttpGet("{search}")]
        [ActionName("SearchByDishName")]
        public async Task<IActionResult> SearchByDishName(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return BadRequest("Search term cannot be empty.");
            }

            // Use a service or repository to search for dishes by name.
            var results = await serv.SearchDish(search);

            if (results.Any())
            {
                return Ok(results);
            }
            else
            {
                return NotFound("No dishes found with the given search term.");
            }
        }


        [HttpDelete("{DishId}")]
        [ActionName("DeleteDish")]
        public async Task<IActionResult> DeleteDishById(int DishId)
        {
            var result = await serv.DeleteDish(DishId);
            if (result == null || result != true) return NotFound();
            return NoContent();
        }




        //[HttpGet("{dishId}")]
        //[ActionName("getRedictionUrlForSeachedDish")]
        //public async Task<IActionResult> ReturnRedictionUrl(int dishId)
        //{
        //    var result = await serv.GetDishWithCategoryAndMenuId(dishId);
        //    if (result == null) return NotFound("Dish not found or dish not assigned to category or menu");
        //    return RedirectPermanent(result);
        //}

    }
}
