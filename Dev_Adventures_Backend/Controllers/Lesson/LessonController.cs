using Dev_Db.Data;
using Dev_Models.DTOs.LessonDTO;
using Dev_Models.Mappers.Lessons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dev_Adventures_Backend.Controllers.LessonController
{

    [ApiController]
    [Route("/api/[controller]")]
    public class LessonController : ControllerBase
    {
       private readonly Dev_DbContext _context;

        public LessonController(Dev_DbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("Courses/{courseId}")]
        public async Task<IActionResult> AddLesson([FromBody] CreateLessonRequestDTO lessondto, [FromRoute] int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound($"Course with ID {courseId} not found");
            }


            var lessonmodel = lessondto.ToLessonFromCreateDTO();
            lessonmodel.CourseId = courseId;
            await _context.AddAsync(lessonmodel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLessonById), new { id = lessonmodel.Id}, lessonmodel.ToLessonDto());

        }



        [HttpGet]
        [Route("Courses/{courseID}")]
        public async Task<IActionResult> GetAllLessons([FromRoute]int courseID)
        {
            var course = await _context.Courses.FindAsync(courseID);

            if (course == null)
            {

                return NotFound();
            }

            var lessons = course.Lessons;

            if (lessons == null)
            {
                return NotFound();
            }

            return Ok(lessons);

        }


        [HttpGet]
        [Route("courses/{courseID}/lesson/{lessonID}")]

        public async Task<IActionResult> GetLessonById([FromRoute] int courseID, [FromRoute] int lessonID)
        {

            var course = await _context.Courses.FindAsync(courseID);

            if (course == null)
            {

                return NotFound();
            }
            var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonID);

            if(lesson == null)
            {
                return NotFound();
            }

            return Ok(lesson);


        }

        [HttpDelete]
        [Route("courses/{courseID}/lesson/{lessonID}")]

        public async Task<IActionResult> DeleteLesson([FromRoute] int courseID, [FromRoute] int lessonID) {

            var course = await _context.Courses.FindAsync(courseID);

            if (course == null)
            {

                return NotFound();
            }
            var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonID);

            if (lesson == null)
            {
                return NotFound();
            }

            _context.Lessons.Remove(lesson);
           await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpPut]
        [Route("courses/{courseID}/lesson/{lessonID}")]

        public async Task<IActionResult> UpdateLesson([FromRoute] int courseID, [FromRoute] int lessonID, [FromBody] UpdateLessonRequestDTO lessondto)
        {
            var course = await _context.Courses.FindAsync(courseID);

            if (course == null)
            {

                return NotFound();
            }
            var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonID);

            if (lesson == null)
            {
                return NotFound();
            }

            lesson.UpdateLessonFromDTO(lessondto);
            await _context.SaveChangesAsync();

            return Ok(lesson.ToLessonDto());

        }


    }
}
