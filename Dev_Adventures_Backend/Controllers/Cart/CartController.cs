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
                .Include(c => c.PlansCarts)
                    .ThenInclude(pc => pc.Plan)
                        .ThenInclude(p => p.PlansCourses)
                            .ThenInclude(pc => pc.Course)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (userCart == null)
            {
                return NotFound("Cart not found for this user.");
            }

            var cartContent = new
            {
                Courses = userCart.courses,
                Plans = userCart.PlansCarts?.Select(pc => new
                {
                    Plan = pc.Plan,
                    AppliedPrice = pc.AppliedPrice,
                    DateAdded = pc.DateAdded
                }),
                TotalPrice = userCart.totalPrice
            };

            return Ok(cartContent);
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

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            string userId = GetUserId();
            if (userId == null) return Unauthorized("User not authenticated");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userCart = await _context.Carts
                    .Include(c => c.courses)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (userCart == null || !userCart.courses.Any())
                    return BadRequest("Cart is empty");

                var now = DateTime.UtcNow;
                var userCourses = userCart.courses.Select(course => new UserCourse
                {
                    UserId = userId,
                    CourseId = course.Id,
                    EnrollmentDate = now,
                    Progress = 0,
                    CompletionDate = null
                }).ToList();

                var existingEnrollments = await _context.UserCourses
                    .Where(uc => uc.UserId == userId &&
                           userCart.courses.Select(c => c.Id).Contains(uc.CourseId))
                    .ToListAsync();

                if (existingEnrollments.Any())
                {
                    var duplicateCourses = existingEnrollments
                        .Select(e => userCart.courses.First(c => c.Id == e.CourseId).Title)
                        .ToList();

                    return BadRequest($"Already enrolled in: {string.Join(", ", duplicateCourses)}");
                }

                await _context.UserCourses.AddRangeAsync(userCourses);

                userCart.courses.Clear();
                userCart.totalPrice = 0;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Enrollment successful");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Checkout failed: {ex.Message}");
            }
        }

        [HttpGet("isEnrolled/{id}")]
        public async Task<IActionResult> IsEnrolled([FromRoute] int id)
        {
            string userId = GetUserId();
            if (userId == null) return Unauthorized("User not authenticated");

            var enrollment = await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == id);

            if (enrollment == null)
            {
                return Ok(new { isEnrolled = false });
            }

            return Ok(new { isEnrolled = true });
        }

    }
}
