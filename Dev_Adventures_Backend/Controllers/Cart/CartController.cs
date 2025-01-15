using Dev_Db.Data;
using Dev_Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dev_Adventures_Backend.Controllers.Cart
{

    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly Dev_DbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(Dev_DbContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }





        [Route("Course/{id}")]
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromRoute] int ID)
        {
            var UserID = GetCurrentUserId();
            var course = await _context.Courses.FirstOrDefaultAsync(s => s.Id == ID);
            var userCart = await _context.Carts.FirstOrDefaultAsync(s => s.UserId.Equals(UserID));

            if(course == null) {
            
            return NotFound();
            }
            else if(userCart.courses.Contains(course))
            {
                return BadRequest("Course Already in Cart");
            }
            else
            {
                userCart.courses.Add(course);

               await _context.SaveChangesAsync();

                return Ok("Course succesfully added");
            }
           
        }


        [Route("Course/{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] int ID)
        {
            var UserID = GetCurrentUserId();
            var course = await _context.Courses.FirstOrDefaultAsync(s => s.Id == ID);
            var userCart = await _context.Carts
                .Include(c => c.courses)
                .FirstOrDefaultAsync(s => s.UserId.Equals(UserID));

            if (userCart == null)
            {
                return NotFound("Cart not found");
            }

            if (course == null)
            {
                return NotFound();
            }

            if (!userCart.courses.Contains(course))
            {
                return BadRequest("Course deoesnt exist in Cart");
            }

            userCart.courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok("Course successfully deleted");
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var UserID = GetCurrentUserId();
            var userCart = await _context.Carts.FirstOrDefaultAsync(s => s.UserId.Equals(UserID));

            if (userCart == null)
            {
                return NotFound("Cart not found");
            }

            var courses = userCart.courses.ToArray();
            return Ok(courses);
        }
    }
}
