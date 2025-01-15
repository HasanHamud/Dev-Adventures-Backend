using Dev_Db.Data;
using Dev_Models.Models;
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
        public CartController(Dev_DbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("Course/{id}")]
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromRoute] int ID)
        {
            var UserID = _userManager.GetUserId(User);
            var course = await _context.Courses.FirstOrDefaultAsync(s => s.Id == ID);
            var userCart = await _context.Carts.FirstOrDefaultAsync(s => s.UserId.Equals(UserID));

            if (course == null)
            {

                return NotFound();
            }
            else if (userCart.courses.Contains(course))
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
            var UserID = _userManager.GetUserId(User);
            var course = await _context.Courses.FirstOrDefaultAsync(s => s.Id == ID);
            var userCart = await _context.Carts.FirstOrDefaultAsync(s => s.UserId.Equals(UserID));

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

                await _context.SaveChangesAsync();

                return Ok("Course succesfully deleted");
            }

        }

    }
}
