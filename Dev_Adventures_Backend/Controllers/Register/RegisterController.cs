using Dev_Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dev_Adventures_Backend.Controllers.Register
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null)
                return BadRequest(new { message = "Invalid request." });

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid registration data." });

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email is already in use." });
            }

            var user = new User
            {
                Fullname = model.Fullname,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var defaultRole = "User";
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                }

                var roleResult = await _userManager.AddToRoleAsync(user, defaultRole);
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to assign the default role." });
                }

                user.userCart = new Dev_Models.Models.Cart(user.Id);
                return Ok(new { message = "Registration successful." });
            }

            return BadRequest(result.Errors);
        }
    }
}
