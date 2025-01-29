using Dev_Db.Data;
using Dev_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dev_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseDetailsController : ControllerBase
    {
        private readonly Dev_DbContext _context;

        public CourseDetailsController(Dev_DbContext context)
        {
            _context = context;
        }

        [HttpGet("{courseId}/requirements")]
        public IActionResult GetRequirements(int courseId)
        {
            var course = _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => c.Requirements)
                .FirstOrDefault();

            if (course == null) return NotFound("Course not found.");

            return Ok(course.Select(r => new { id = r.Id, description = r.Description }).ToList());

        }

        [HttpGet("{courseId}/learning-objectives")]
        public IActionResult GetLearningObjectives(int courseId)
        {
            var course = _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => c.LearningObjectives)
                .FirstOrDefault();

            if (course == null) return NotFound("Course not found.");

            return Ok(course.Select(o => new { id = o.Id, description = o.Description }).ToList());
        }



        [HttpPost("{courseId}/requirements")]
        public async Task<IActionResult> AddRequirement(int courseId, [FromBody] RequirementDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound($"Course with ID {courseId} not found.");

            var requirement = new CourseRequirement { Description = dto.Description, CourseId = courseId };
            course.Requirements.Add(requirement);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRequirements),
                new { courseId },
                new { id = requirement.Id, description = requirement.Description }
            );
        }




        [HttpPost("{courseId}/learning-objectives")]
        public IActionResult AddLearningObjective(int courseId, [FromBody] ObjectivesDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.description))
                return BadRequest("Description is required.");

            var course = _context.Courses.Find(courseId);
            if (course == null) return NotFound("Course not found.");

            var learningObjective = new CourseLearningObjective
            {
                Description = dto.description,
                CourseId = courseId
            };
            course.LearningObjectives.Add(learningObjective);
            _context.SaveChanges();

            return Ok("Learning objective added successfully.");
        }



        [HttpDelete("{courseId}/requirements/{requirementId}")]
        public IActionResult DeleteRequirement(int courseId, int requirementId)
        {
            var requirement = _context.CourseRequirements.FirstOrDefault(r => r.Id == requirementId && r.CourseId == courseId);
            if (requirement == null) return NotFound("Requirement not found.");

            _context.CourseRequirements.Remove(requirement);
            _context.SaveChanges();

            return Ok("Requirement deleted successfully.");
        }


        [HttpDelete("{courseId}/learning-objectives/{objectiveId}")]
        public async Task<IActionResult> DeleteLearningObjective(int courseId, int objectiveId)
        {
            var objective = _context.CourseLearningObjectives.FirstOrDefault(r => r.Id == objectiveId && r.CourseId == courseId);
            if (objective == null) return NotFound("Objective not found.");

            _context.CourseLearningObjectives.Remove(objective);
            _context.SaveChanges();

            return Ok("Objective deleted successfully.");
        }


        [HttpPut("{courseId}/requirements/{requirementId}")]
        public async Task<IActionResult> UpdateRequirement(int courseId, int requirementId, [FromBody] string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return BadRequest("Description is required.");

            var requirement = await _context.CourseRequirements
                .FirstOrDefaultAsync(r => r.Id == requirementId && r.CourseId == courseId);
            if (requirement == null) return NotFound($"Requirement with ID {requirementId} not found for course {courseId}.");

            requirement.Description = description;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Requirement updated successfully." });
        }


        [HttpPut("{courseId}/learning-objectives/{objectiveId}")]
        public async Task<IActionResult> UpdateLearningObjective(int courseId, int objectiveId, [FromBody] ObjectivesDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.description))
                return BadRequest("Description is required.");

            var objective = await _context.CourseLearningObjectives
                .FirstOrDefaultAsync(o => o.Id == objectiveId && o.CourseId == courseId);

            if (objective == null)
                return NotFound($"Learning Objective with ID {objectiveId} not found for Course ID {courseId}.");

            objective.Description = dto.description;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Learning objective updated successfully.", objectiveId, courseId });
        }
    }


    public class RequirementDto
    {
        public string Description { get; set; }
    }

    public class ObjectivesDto
    {
        public string description { get; set; }
    }
}
