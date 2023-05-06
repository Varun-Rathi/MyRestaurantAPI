using CodeFirstRestaurantAPI.Models;
using CodeFirstRestaurantAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstRestaurantAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IService<Menu, int> ctx;

        public MenuController(IService<Menu, int> ctx)
        {
            this.ctx = ctx;
        }



        [HttpGet]
        [ActionName("GetAllRecords")]
        public async Task<IActionResult> Get()
        {
            var results = await ctx.Get();
            return Ok(results);
        }

        [HttpPost]
        [ActionName("CreateMenu")]
        public async Task<IActionResult> Post([FromForm] Menu obj)
        {
            var menu = await ctx.Create(obj);
            return Ok(menu);
        }


        [HttpGet("{id}")]
        [ActionName("GetById")]

        public async Task<IActionResult> Get(int id)
        {
            var menu = await ctx.Get(id);

            if (menu == null)
            {
                return NotFound();
            }
            return Ok(menu);
        }


        [HttpPut("{id}")]
        [ActionName("UpdateById")]


        public async Task<IActionResult> Update([FromForm] Menu obj, int id)
        {
            var menu = await ctx.UpdateAsync(obj, id);

            if (menu == null)
                return NotFound();

            return Ok(menu);




        }

        [HttpDelete("{id}")]
        [ActionName("deletebyid")]


        public async Task<IActionResult> Delete(int id)
        {


            var menu=await ctx.DeleteAsync(id);


            ////The if (!menu) condition checks whether the result of the DeleteAsync
            ////operation in the MenuService was successful or not...........
            if (menu == false)
            {

                // return 404 if record not found..
                return NotFound();
            }
            //else if (menu == true) return Forbid("Menu already deleted");            


            if (menu == true)
                return NoContent();

            return BadRequest();

        }
    }

}

