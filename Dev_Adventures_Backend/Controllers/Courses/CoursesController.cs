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

        [HttpPost("{courseId}/complete")]
        public async Task<IActionResult> CompleteCourse([FromRoute] int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _context.Courses.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null) return NotFound("Course not found.");

            var userLessons = await _context.UserLessonProgresses
                .Where(lp => lp.UserId == userId && lp.CourseId == courseId && lp.IsCompleted)
                .CountAsync();

            if (userLessons != course.Lessons.Count)
            {
                return BadRequest("You must complete all lessons to finish the course.");
            }

            var certificate = new Certificate
            {
                UserId = userId,
                CourseId = courseId,
                IssuedDate = DateTime.UtcNow
            };
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course completed! Certificate issued." });
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
            var course = await _context.Courses
                .Include(c => c.LearningObjectives)
                .Include(c => c.Requirements)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound(new { message = "Course not found." });
            }

            return Ok(course.ToCourseDto());
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCourse([FromForm] CreateCourseRequestDTO courseDTO)
        {
            if (!IsAdmin())
            {
                return Unauthorized(new { message = "You are not authorized to add courses." });
            }

            var courseModel = await courseDTO.ToCourseFromCreateDTO();
            await _context.Courses.AddAsync(courseModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = courseModel.Id }, courseModel.ToCourseDto());
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse([FromRoute] int id, [FromForm] UpdateCourseRequestDTO updateDTO)
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

            await courseModel.UpdateCourseFromDTO(updateDTO);
            await _context.SaveChangesAsync();

            return Ok(courseModel.ToCourseDto());
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Search query cannot be empty." });
            }

            var courses = await _context.Courses
                .Where(c => c.Title.Contains(query) || c.Description.Contains(query))
                .Select(c => new { c.Id, c.Title })
                .ToListAsync();

            return Ok(courses);
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