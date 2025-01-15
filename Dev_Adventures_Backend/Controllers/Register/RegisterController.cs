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

        public RegisterController(UserManager<User> userManager)
        {
            _userManager = userManager;
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
                user.userCart = new Dev_Models.Models.Cart(user.Id);
                return Ok(new { message = "Registration successful." });
            }

            return BadRequest(result.Errors);
        }
    }
}
