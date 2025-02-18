using Dev_Db.Data;
using Dev_Models.DTOs.LessonDTO;
using Dev_Models.DTOs.VideoDTO;
using Dev_Models.Mappers.Lessons;
using Dev_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dev_Adventures_Backend.Controllers.LessonController
{

    [ApiController]
    [Route("api/[controller]")]
    public class LessonController : ControllerBase
    {
        private readonly Dev_DbContext _context;

        public LessonController(Dev_DbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("courses/{courseId}")]
        public async Task<IActionResult> AddLesson([FromBody] CreateLessonRequestDTO lessondto, [FromRoute] int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound($"Course with ID {courseId} not found");
            }


            var lessonmodel = lessondto.ToLessonFromCreateDTO();
            lessonmodel.CourseId = courseId;
            course.Lessons.Add(lessonmodel);
            await _context.Lessons.AddAsync(lessonmodel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLessonById),
                new
                {
                    courseId = courseId,
                    lessonId = lessonmodel.Id
                },
                lessonmodel.ToLessonDto()
            );
        }



        [HttpGet]
        [Route("Courses/{courseID}")]
        public async Task<IActionResult> GetAllLessons([FromRoute] int courseID)
        {
            var course = await _context.Courses
                   .Include(c => c.Lessons)
                   .FirstOrDefaultAsync(c => c.Id == courseID);
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
            var lesson = await _context.Lessons
                .Where(l => l.CourseId == courseID && l.Id == lessonID)
                .FirstOrDefaultAsync();

            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }

            return Ok(lesson);
        }


        [HttpDelete]
        [Route("{courseID}/{lessonID}")]

        public async Task<IActionResult> DeleteLesson([FromRoute] int courseID, [FromRoute] int lessonID)
        {

            var course = await _context.Courses
                   .Include(c => c.Lessons)
                   .FirstOrDefaultAsync(c => c.Id == courseID);
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
        [Route("{courseID}/{lessonID}")]
        public async Task<IActionResult> UpdateLesson([FromRoute] int courseID, [FromRoute] int lessonID, [FromBody] UpdateLessonRequestDTO lessondto)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == courseID);
            if (course == null)
            {
                return NotFound();
            }

            var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonID);

            if (lesson == null)
            {
                return NotFound();
            }

            lesson.Title = lessondto.Title;
            lesson.Description = lessondto.Description;

            await _context.SaveChangesAsync();

            return Ok(lesson);
        }



        [HttpPost]
        [Route("{courseID}/{lessonID}")]
        public async Task<IActionResult> AddVideo([FromRoute] int courseID, [FromRoute] int lessonID, [FromBody] AddVideoRequest request)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                    .ThenInclude(l => l.Videos)
                .FirstOrDefaultAsync(c => c.Id == courseID);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonID);
            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }

            var newVideo = new Video
            {
                LessonId = lessonID,
                VideoURL = request.VideoURL,
                Title = request.Title,
                Length = request.Length
            };

            lesson.Videos.Add(newVideo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllVideos),
                new { courseID, lessonID, videoId = newVideo.Id },
                newVideo);
        }

        [HttpDelete]
        [Route("{courseID}/{lessonID}/{videoID}")]
        public async Task<IActionResult> DeleteVideo([FromRoute] int lessonID, [FromRoute] int videoID, [FromRoute] int courseID)
        {
            var course = await _context.Courses
               .Include(c => c.Lessons)
                   .ThenInclude(l => l.Videos)
               .FirstOrDefaultAsync(c => c.Id == courseID);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonID);
            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }



            var video = lesson.Videos.FirstOrDefault(x => x.Id == videoID);


            if (video == null)
            {
                return NotFound();
            }

            lesson.Videos.Remove(video);
            await _context.SaveChangesAsync();

            return NoContent();



        }

        [HttpPut]
        [Route("{courseID}/{lessonID}/{videoID}")]

        public async Task<IActionResult> UpdateVideo([FromRoute] int lessonID, [FromRoute] int videoID, [FromRoute] int courseID, [FromBody] UpdateVideoRequestDTO newVideo)
        {
            var course = await _context.Courses
               .Include(c => c.Lessons)
                   .ThenInclude(l => l.Videos)
               .FirstOrDefaultAsync(c => c.Id == courseID);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonID);
            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }

            var video = lesson.Videos.FirstOrDefault(x => x.Id == videoID);


            if (video == null)
            {
                return NotFound();
            }

            video.Title = newVideo.Title;
            video.VideoURL = newVideo.VideoURL;

            await _context.SaveChangesAsync();

            return NoContent();



        }

        [HttpGet]
        [Route("{courseID}/{lessonID}")]
        public async Task<IActionResult> GetAllVideos([FromRoute] int lessonID, [FromRoute] int courseID)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                    .ThenInclude(l => l.Videos)
                .FirstOrDefaultAsync(c => c.Id == courseID);

            if (course == null)
            {
                return NotFound();
            }

            var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonID);

            if (lesson == null)
            {
                return NotFound();
            }

            return Ok(lesson.Videos);
        }

        [HttpGet]
        [Route("{courseID}/{lessonID}/{videoID}")]
        public async Task<IActionResult> GetVideoById([FromRoute] int courseID, [FromRoute] int lessonID, [FromRoute] int videoID)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                    .ThenInclude(l => l.Videos)
                .FirstOrDefaultAsync(c => c.Id == courseID);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            var lesson = course.Lessons.FirstOrDefault(l => l.Id == lessonID);
            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }

            var video = lesson.Videos.FirstOrDefault(v => v.Id == videoID);
            if (video == null)
            {
                return NotFound("Video not found.");
            }
            return Ok(video);
        }
    }
}