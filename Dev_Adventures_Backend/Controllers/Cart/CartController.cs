using Dev_Db.Data;
using Dev_Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dev_Adventures_Backend.Controllers.Cart
{
    [Route("api/[controller]")]
    [ApiController]

    public class CartController : ControllerBase
    {
        private readonly Dev_DbContext _context;
        private readonly UserManager<User> _userManager;
        public CartController(Dev_DbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("Course/{id}")]
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromRoute] int ID)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var course = await _context.Courses.FirstOrDefaultAsync(s => s.Id == ID);
            var userCart = await _context.Carts.Include(c => c.courses)
                                               .FirstOrDefaultAsync(s => s.UserId.Equals(UserId));

            if (course == null)
            {
                return NotFound();
            }

            if (userCart == null)
            {
                return BadRequest("User cart not found.");
            }

            if (userCart.courses.Any(c => c.Id == course.Id))
            {
                return BadRequest("Course Already in Cart");
            }

            userCart.courses.Add(course);
            userCart.totalPrice += course.Price;
            await _context.SaveChangesAsync();

            return Ok("Course successfully added");
        }


        [Route("Course/{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] int ID)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var course = await _context.Courses.FirstOrDefaultAsync(s => s.Id == ID);
            var userCart = await _context.Carts
                    .Include(c => c.courses)
                    .FirstOrDefaultAsync(s => s.UserId.Equals(UserId));
            if (course == null)
            {

                return NotFound();
            }
            else if (!userCart.courses.Contains(course))
            {
                return BadRequest("Course deoesn't exist in Cart");
            }
            else
            {
                userCart.courses.Remove(course);
                userCart.totalPrice -= course.Price;
                await _context.SaveChangesAsync();

                return Ok("Course succesfully deleted");
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {

                return Unauthorized();

            }
            var userCart = _context.Carts
                .Include(c => c.courses)
                .FirstOrDefault(c => c.UserId.Equals(UserId));
            if (userCart == null)
            {
                return NotFound();


            }


            return Ok(userCart.courses);

        }



        [HttpGet("price")]

        public async Task<IActionResult> GetTotalPrice()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {

                return Unauthorized();

            }
            var userCart = _context.Carts
                .Include(c => c.courses)
                .FirstOrDefault(c => c.UserId.Equals(UserId));
            if (userCart == null)
            {
                return NotFound();


            }


            return Ok(userCart.totalPrice);
        }

    }
}

