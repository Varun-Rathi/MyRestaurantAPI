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
*/
    }
}
