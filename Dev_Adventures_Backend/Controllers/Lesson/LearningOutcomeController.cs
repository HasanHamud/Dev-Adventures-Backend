using Dev_Db.Data;
using Dev_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dev_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningOutcomesController : ControllerBase
    {
        private readonly Dev_DbContext _context;

        public LearningOutcomesController(Dev_DbContext context)
        {
            _context = context;
        }


        [HttpGet("lesson/{lessonId}")]
        public async Task<IActionResult> GetLearningOutcomesByLessonId(int lessonId)
        {
            var outcomes = await _context.LearningOutcomes
                .Where(lo => lo.LessonId == lessonId)
                .ToListAsync();
            return Ok(outcomes);
        }


        [HttpPost]
        public async Task<IActionResult> CreateLearningOutcome([FromBody] LearningOutcomeDto learningOutcomeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lesson = await _context.Lessons.FindAsync(learningOutcomeDto.LessonId);
            if (lesson == null)
                return NotFound(new { message = "Lesson not found." });

            var learningOutcome = new LearningOutcome
            {
                Description = learningOutcomeDto.Description,
                LessonId = learningOutcomeDto.LessonId
            };

            _context.LearningOutcomes.Add(learningOutcome);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLearningOutcomeById), new { id = learningOutcome.Id }, learningOutcome);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetLearningOutcomeById(int id)
        {
            var outcome = await _context.LearningOutcomes.FindAsync(id);
            if (outcome == null)
                return NotFound();
            return Ok(outcome);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLearningOutcome(int id, [FromBody] LearningOutcomeDto updatedOutcome)
        {
            if (id != updatedOutcome.LessonId || !ModelState.IsValid)
                return BadRequest();

            var existingOutcome = await _context.LearningOutcomes.FindAsync(id);
            if (existingOutcome == null)
                return NotFound();

            var lesson = await _context.Lessons.FindAsync(updatedOutcome.LessonId);
            if (lesson == null)
                return NotFound(new { message = "Lesson not found." });

            existingOutcome.Description = updatedOutcome.Description;
            existingOutcome.LessonId = updatedOutcome.LessonId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LearningOutcomeExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLearningOutcome(int id)
        {
            var outcome = await _context.LearningOutcomes.FindAsync(id);
            if (outcome == null)
                return NotFound();

            _context.LearningOutcomes.Remove(outcome);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool LearningOutcomeExists(int id)
        {
            return _context.LearningOutcomes.Any(e => e.Id == id);
        }
    }
}
