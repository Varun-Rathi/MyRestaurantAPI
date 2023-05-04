using CodeFirstRestaurantAPI.Models;
using CodeFirstRestaurantAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstRestaurantAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService serv;

        public AuthController(AuthService serv)
        {
            this.serv = serv; 
        }

        [HttpPost]
        [ActionName("registerUser")]
        public async Task<IActionResult> RegisterUser(RegisterUser user)
        {
            if (ModelState.IsValid)
            {

                var isCreated = await serv.RegisterUserAsync(user);
                if (isCreated == false)
                    return Conflict($"{user.UserName} already exists!");

                return Ok(true);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [ActionName("loginUser")]
        public async Task<IActionResult> LoginUser(LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                var result = await serv.AuthenticateUserAsync(loginUser);
                if (result == false)
                    return Unauthorized(result);

                return Ok(result);

            }
            return BadRequest(ModelState);
        }
    }
}
