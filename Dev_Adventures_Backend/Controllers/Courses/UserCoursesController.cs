using Dev_Db.Data;
using Dev_Models.DTOs.UserCourseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserCoursesController : ControllerBase
{
    private readonly Dev_DbContext _context;

    public UserCoursesController(Dev_DbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserCourses()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var courses = await _context.UserCourses
            .Where(uc => uc.UserId == userId)
            .Select(uc => new UserCourseDto
            {
                CourseId = uc.CourseId,
                Title = uc.Course.Title,
                Description = uc.Course.Description,
                Progress = uc.Progress,
                EnrollmentDate = uc.EnrollmentDate,
                CompletionDate = uc.CompletionDate,
                ImgURL = uc.Course.ImgURL,
                Rating = uc.Course.Rating,
                Price = uc.Course.Price,
                Duration = uc.Course.Duration.ToString(),
                Level = uc.Course.Level.ToString(),
                Language = uc.Course.Language
            })
            .ToListAsync();

        return Ok(courses);
    }



    [HttpPut("{courseId}/progress")]
    public async Task<IActionResult> UpdateProgress(int courseId, [FromBody] decimal progress)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var userCourse = await _context.UserCourses
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        if (userCourse == null)
            return NotFound();

        userCourse.Progress = progress;
        if (progress >= 100)
        {
            userCourse.CompletionDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }


    [HttpDelete("{courseId}")]
    public async Task<IActionResult> RemoveEnrollment(int courseId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var userCourse = await _context.UserCourses
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        if (userCourse == null)
            return NotFound();

        _context.UserCourses.Remove(userCourse);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
