using Dev_Db.Data;
using Dev_Models.Models;
using Microsoft.AspNetCore.Mvc;

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

            return Ok(course.Select(o => o.Description).ToList());
        }



        [HttpPost("{courseId}/requirements")]
        public IActionResult AddRequirement(int courseId, [FromBody] RequirementDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest("Description is required.");

            var course = _context.Courses.Find(courseId);
            if (course == null) return NotFound("Course not found.");

            var requirement = new CourseRequirement { Description = dto.Description, CourseId = courseId };
            course.Requirements.Add(requirement);
            _context.SaveChanges();

            return Ok("Requirement added successfully.");
        }



        [HttpPost("{courseId}/learning-objectives")]
        public IActionResult AddLearningObjective(int courseId, [FromBody] ObjectivesDto dto)
        {
            var course = _context.Courses.Find(courseId);
            if (course == null) return NotFound("Course not found.");

            var learningObjective = new CourseLearningObjective { Description = dto.description, CourseId = courseId };
            course.LearningObjectives.Add(learningObjective);
            _context.SaveChanges();

            return Ok("Learning objective added successfully.");
        }

        public class ObjectivesDto
        {
            public string description { get; set; }
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
        public IActionResult DeleteLearningObjective(int courseId, int objectiveId)
        {
            var objective = _context.CourseLearningObjectives.FirstOrDefault(o => o.Id == objectiveId && o.CourseId == courseId);
            if (objective == null) return NotFound("Learning Objective not found.");

            _context.CourseLearningObjectives.Remove(objective);
            _context.SaveChanges();

            return Ok("Learning objective deleted successfully.");
        }

        [HttpPut("{courseId}/requirements/{requirementId}")]
        public IActionResult UpdateRequirement(int courseId, int requirementId, [FromBody] string description)
        {
            var requirement = _context.CourseRequirements.FirstOrDefault(r => r.Id == requirementId && r.CourseId == courseId);
            if (requirement == null) return NotFound("Requirement not found.");

            requirement.Description = description;
            _context.SaveChanges();

            return Ok("Requirement updated successfully.");
        }

        [HttpPut("{courseId}/learning-objectives/{objectiveId}")]
        public IActionResult UpdateLearningObjective(int courseId, int objectiveId, [FromBody] string description)
        {
            var objective = _context.CourseLearningObjectives.FirstOrDefault(o => o.Id == objectiveId && o.CourseId == courseId);
            if (objective == null) return NotFound("Learning Objective not found.");

            objective.Description = description;
            _context.SaveChanges();

            return Ok("Learning objective updated successfully.");
        }
    }


    public class RequirementDto
    {
        public string Description { get; set; }
    }
}
