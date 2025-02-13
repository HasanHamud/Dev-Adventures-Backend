using Dev_Db.Data;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dev_Adventures_Backend.Controllers.Cart
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly Dev_DbContext _context;

        public CartController(Dev_DbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            string userId = GetUserId();
            if (userId == null) return Unauthorized("User not authenticated");

            var userCart = await _context.Carts
                .Include(c => c.courses)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (userCart == null)
            {
                return NotFound("Cart not found for this user.");
            }

            return Ok(userCart.courses);
        }

        [HttpGet("price")]
        public async Task<IActionResult> GetTotalPrice()
        {
            string userId = GetUserId();
            if (userId == null) return Unauthorized("User not authenticated");

            var userCart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (userCart == null)
            {
                return NotFound("Cart not found for this user.");
            }

            return Ok(userCart.totalPrice);
        }

        [HttpPost("Course/{id}")]
        public async Task<IActionResult> AddCourse([FromRoute] int id)
        {
            string userId = GetUserId();
            if (userId == null) return Unauthorized("User not authenticated");

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound("Course not found");

            var userCart = await _context.Carts
                .Include(c => c.courses)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (userCart == null)
            {
                userCart = new Dev_Models.Models.Cart { UserId = userId };
                _context.Carts.Add(userCart);
            }


            if (userCart.courses.Any(c => c.Id == course.Id))
            {
                return BadRequest("Course already in cart");
            }

            userCart.courses.Add(course);
            userCart.totalPrice += course.Price;
            await _context.SaveChangesAsync();

            return Ok("Course successfully added to cart");
        }

        [HttpDelete("Course/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            string userId = GetUserId();
            if (userId == null) return Unauthorized("User not authenticated");

            var userCart = await _context.Carts
                .Include(c => c.courses)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (userCart == null) return NotFound("Cart not found");

            var course = userCart.courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return BadRequest("Course not found in cart");

            userCart.courses.Remove(course);
            userCart.totalPrice -= course.Price;
            await _context.SaveChangesAsync();

            return Ok("Course successfully removed from cart");
        }
    }
}
