using Dev_Db.Data;
using Dev_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dev_Adventures_Backend.Controllers.Profile
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly Dev_DbContext _context;

        public ProfileController(UserManager<User> userManager, Dev_DbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id) ?? await _userManager.FindByEmailAsync(id);

                if (user == null)
                {
                    return NotFound(new { message = $"User with ID/Email '{id}' not found." });
                }

                var profileDto = new ProfileDto
                {
                    Id = user.Id.ToString(),
                    Fullname = user.Fullname,
                    Email = user.Email,
                    ProfileImage = user.ProfileImage ?? "default-profile.png",
                    PhoneNumber = user.PhoneNumber
                };

                return Ok(profileDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the profile.", error = ex.Message });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userManager.Users
                .Select(u => new ProfileDto
                {
                    Id = u.Id.ToString(),
                    Email = u.Email,
                    Fullname = u.Fullname,
                    ProfileImage = u.ProfileImage ?? "default-profile.png",
                    PhoneNumber = u.PhoneNumber
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
