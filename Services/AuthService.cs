using CodeFirstRestaurantAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace CodeFirstRestaurantAPI.Services
{
    public class AuthService
    {
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AuthService(IConfiguration configuration, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<bool> RegisterUserAsync(RegisterUser user)
        {
            bool isCreated = false;
            var newUser = new IdentityUser() { UserName = user.UserName };
            var createUser = await userManager.CreateAsync(newUser, user.Password);
            if (createUser.Succeeded)
            {
                isCreated = true;
            }
            return isCreated;
        }

        public async Task<bool> AuthenticateUserAsync(LoginUser user)
        {
            var result = await signInManager.PasswordSignInAsync(user.UserName, user.Password, false, true);
            if (result.Succeeded)
                return true;
            return false;
        }

    }
}
