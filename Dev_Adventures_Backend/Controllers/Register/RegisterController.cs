using Dev_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly Dev_DbContext _context;  // Add this

    public RegisterController(UserManager<User> userManager, Dev_DbContext context)  // Add context parameter
    {
        _userManager = userManager;
        _context = context;  // Add this
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
                user.userCart = new Dev_Models.Cart(user.Id);
                return Ok(new { message = "Registration successful." });
            }

        return BadRequest(result.Errors);
    }
}