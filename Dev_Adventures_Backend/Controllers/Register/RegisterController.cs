using Dev_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid registration request." });
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email is already in use." });
            }

            var user = new User
            {
                UserName = model.Email,
                Fullname = model.Fullname,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok(new { message = "Registration successful.", user = new { user.Id, user.Fullname, user.Email } });
            }

            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = errorMessages });
        }
    }

    public class RegisterModel
    {
        [Required]
        public string Fullname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
