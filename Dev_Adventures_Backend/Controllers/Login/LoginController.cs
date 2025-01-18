using Dev_Models.DTOs.LoginDTO;
using Dev_Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dev_Adventures_Backend.Controllers.Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LoginController> _logger;
        private readonly IConfiguration _configuration;

        public LoginController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<LoginController> logger,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid login request.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation($"User roles: {string.Join(", ", roles)}");

                var token = GenerateJwtToken(user);
                return Ok(new
                {
                    message = "Login successful.",
                    id = user.Id,
                    token = token
                });
            }

            return Unauthorized(new { message = "Invalid email or password." });
        }

        [HttpGet("debug-token")]
        public IActionResult DebugToken()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);
                Console.WriteLine($"Received Token: {token}");

                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token))
                {
                    return BadRequest(new { error = "The token is not in a valid JWT format." });
                }

                try
                {
                    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                    return Ok(new
                    {
                        Header = jsonToken.Header,
                        Claims = jsonToken.Claims.Select(c => new { c.Type, c.Value }),
                        ValidFrom = jsonToken.ValidFrom,
                        ValidTo = jsonToken.ValidTo,
                        Issuer = jsonToken.Issuer,
                        Audience = jsonToken.Audiences
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = ex.Message });
                }
            }

            return BadRequest(new { error = "No token found" });
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };

            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}