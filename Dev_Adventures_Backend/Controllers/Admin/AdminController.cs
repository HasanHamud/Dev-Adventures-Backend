using Dev_Db.Data;
using Dev_Models.DTOs.UserDTo;
using Dev_Models.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AdminController> _logger;
    private readonly Dev_DbContext _context;
    public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminController> logger, Dev_DbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _context = context;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDTOs = new List<UserDTO>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDTOs.Add(new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList()
            });
        }

        return Ok(userDTOs);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID '{id}' not found." });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, userRoles);
            }

            await _context.ChatMessages
                .Where(m => m.SenderId == id || m.ReceiverId == id)
                .ExecuteDeleteAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await transaction.CommitAsync();
            return Ok(new { message = $"User with ID '{id}' deleted successfully." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, new
            {
                message = "An error occurred while deleting the user.",
                error = ex.Message,
                details = ex.InnerException?.Message
            });
        }
    }

    [HttpPut("user/roles/{id}")]
    public async Task<IActionResult> UpdateUserRoles(string id, [FromBody] UserDTO userDto)
    {
        if (userDto == null || userDto.Roles == null || userDto.Roles.Count == 0)
        {
            return BadRequest(new { message = "Invalid roles or request body." });
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = $"User with ID '{id}' not found." });
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return StatusCode(500, new { message = "Failed to remove existing roles." });
        }

        var addResult = await _userManager.AddToRolesAsync(user, userDto.Roles);
        if (!addResult.Succeeded)
        {
            return StatusCode(500, new { message = "Failed to add new roles." });
        }

        return Ok(new { message = $"User with ID '{id}' roles updated successfully." });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID '{id}' not found." });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName ?? "User Name Not Provided",
                Email = user.Email,
                Roles = roles.ToList()
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while fetching the user.", error = ex.Message });
        }
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("test-auth")]
    public IActionResult TestAuth()
    {
        return Ok(new
        {
            Message = "Authorization successful",
            User = User.Identity.Name,
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}