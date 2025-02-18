using Dev_Db.Data;
using Dev_Models.DTOs.LessonDTO;
using Dev_Models.DTOs.VideoDTO;
using Dev_Models.Mappers.Lessons;
using Dev_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Dev_Adventures_Backend.Controllers.LessonController
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonController : ControllerBase
    {
        private readonly Dev_DbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _youtubeApiKey; // You'll need to set this from configuration

        public LessonController(Dev_DbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _youtubeApiKey = configuration["YouTube:ApiKey"]; // Get API key from configuration
        }

        // Helper method to extract YouTube video ID from URL
        private string ExtractYouTubeVideoId(string url)
        {
            var youtubeIdRegex = new Regex(@"(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");
            var match = youtubeIdRegex.Match(url);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        // Method to get video duration from YouTube API
        private async Task<int> GetYouTubeVideoDuration(string videoId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"https://www.googleapis.com/youtube/v3/videos?id={videoId}&part=contentDetails&key={_youtubeApiKey}");

                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    if (doc.RootElement.GetProperty("items").GetArrayLength() > 0)
                    {
                        var durationString = doc.RootElement.GetProperty("items")[0]
                                               .GetProperty("contentDetails")
                                               .GetProperty("duration").GetString();

                        // Parse ISO 8601 duration format
                        var duration = XmlConvert.ToTimeSpan(durationString);

                        // Return duration in seconds
                        return (int)duration.TotalSeconds;
                    }
                }

                return 0; // Default if duration can't be extracted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting video duration: {ex.Message}");
                return 0; // Default if there's an error
            }
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

            return Ok(lesson.ToLessonDto());
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

            Console.WriteLine("Existing videos in this lesson:");
            foreach (var video in lesson.Videos)
            {
                Console.WriteLine($"Video ID: {video.Id}, Title: {video.Title}, URL: {video.VideoURL}");
            }

            // Extract video length from YouTube URL
            int videoLength = 0;
            string videoId = ExtractYouTubeVideoId(request.VideoURL);

            if (videoId != null)
            {
                videoLength = await GetYouTubeVideoDuration(videoId);
            }

            var newVideo = new Video
            {
                LessonId = lessonID,
                VideoURL = request.VideoURL,
                Title = request.Title,
                Length = videoLength // Set the extracted length
            };

            lesson.Videos.Add(newVideo);
            int result = await _context.SaveChangesAsync();

            Console.WriteLine($"SaveChangesAsync result: {result}");
            Console.WriteLine("Videos after adding new video:");
            foreach (var video in lesson.Videos)
            {
                Console.WriteLine($"Video ID: {video.Id}, Title: {video.Title}, URL: {video.VideoURL}, Length: {video.Length} seconds");
            }

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

            if (video.VideoURL != newVideo.VideoURL)
            {
                string videoId = ExtractYouTubeVideoId(newVideo.VideoURL);
                if (videoId != null)
                {
                    video.Length = await GetYouTubeVideoDuration(videoId);
                }
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