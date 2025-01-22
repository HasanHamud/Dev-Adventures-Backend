using Dev_Db.Data;
using Dev_Models.DTOs.Courses;
using Dev_Models.Mappers.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dev_Adventures_Backend.Controllers.Courses
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly Dev_DbContext _context;

        public CoursesController(Dev_DbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return userRole == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _context.Courses.ToListAsync();
            var courseDtos = courses.Select(c => c.ToCourseDto());
            return Ok(courseDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound(new { message = "Course not found." });
            }

            return Ok(course.ToCourseDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCourse([FromBody] CreateCourseRequestDTO coursedto)
        {
            if (!IsAdmin())
            {
                return Unauthorized(new { message = "You are not authorized to add courses." });
            }

            var courseModel = coursedto.ToCourseFromCreateDTO();
            await _context.Courses.AddAsync(courseModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = courseModel.Id }, courseModel.ToCourseDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse([FromRoute] int id, [FromBody] UpdateCourseRequestDTO updateDTO)
        {
            if (!IsAdmin())
            {
                return Unauthorized(new { message = "You are not authorized to update courses." });
            }

            var courseModel = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);

            if (courseModel == null)
            {
                return NotFound(new { message = "Course not found." });
            }

            courseModel.UpdateCourseFromDTO(updateDTO);
            await _context.SaveChangesAsync();

            return Ok(courseModel.ToCourseDto());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse([FromRoute] int id)
        {
            if (!IsAdmin())
            {
                return Unauthorized(new { message = "You are not authorized to delete courses." });
            }

            var courseModel = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);

            if (courseModel == null)
            {
                return NotFound(new { message = "Course not found." });
            }

            _context.Courses.Remove(courseModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}