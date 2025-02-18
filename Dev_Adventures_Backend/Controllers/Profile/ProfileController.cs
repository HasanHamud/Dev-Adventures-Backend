using Dev_Db.Data;
using Dev_Models;
using Dev_Models.Mappers.Profile;
using Dev_Models.Models;
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

                Console.WriteLine($"User found: {user.Id}, Fullname: {user.Fullname}, Email: {user.Email}");

                var profileDto = new ProfileDto
                {
                    Id = user.Id.ToString(),
                    Fullname = string.IsNullOrWhiteSpace(user.Fullname) ? "Full Name Not Provided" : user.Fullname,
                    Email = user.Email,
                    ProfileImage = user.ProfileImage ?? "default-profile.png",
                    PhoneNumber = user.PhoneNumber
                };

                return Ok(profileDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching profile: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching the profile.", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] ProfileDto profileDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id) ?? await _userManager.FindByEmailAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID/Email '{id}' not found." });
                }

                user.Fullname = profileDto.Fullname;
                user.PhoneNumber = profileDto.PhoneNumber;
                user.ProfileImage = profileDto.ProfileImage ?? user.ProfileImage;

                if (!string.IsNullOrEmpty(profileDto.Email) && profileDto.Email != user.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(profileDto.Email);
                    if (existingUser != null)
                    {
                        return BadRequest(new { message = "Email is already in use." });
                    }

                    user.Email = profileDto.Email;
                    user.UserName = profileDto.Email;
                }

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Failed to update profile.",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                var updatedProfileDto = new ProfileDto
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    ProfileImage = user.ProfileImage ?? "default-profile.png",
                    PhoneNumber = user.PhoneNumber
                };

                return Ok(updatedProfileDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the profile.", error = ex.Message });
            }
        }

        [HttpPost("{id}/profile-image")]
        public async Task<IActionResult> UpdateProfileImage(string id, IFormFile profileImage)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id) ?? await _userManager.FindByEmailAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID/Email '{id}' not found." });
                }

                var imagePath = await ProfileMappers.SaveProfileImageAsync(profileImage);
                if (imagePath == null)
                {
                    return BadRequest(new { message = "Invalid image file." });
                }

                if (!string.IsNullOrEmpty(user.ProfileImage) &&
                    user.ProfileImage != "default-profile.png" &&
                    System.IO.File.Exists(Path.Combine("wwwroot", user.ProfileImage.TrimStart('/'))))
                {
                    System.IO.File.Delete(Path.Combine("wwwroot", user.ProfileImage.TrimStart('/')));
                }

                user.ProfileImage = imagePath;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Failed to update profile image.",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new { profileImage = imagePath });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the profile image.", error = ex.Message });
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