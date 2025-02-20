using Dev_Db.Data;
using Dev_Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/progress")]
[ApiController]
public class UserLessonProgressController : ControllerBase
{
    private readonly Dev_DbContext _context;

    public UserLessonProgressController(Dev_DbContext context)
    {
        _context = context;
    }

    [HttpPost("video/complete")]
    public async Task<IActionResult> MarkVideoCompleted([FromBody] UserVideoProgress model)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not found");
        }

        if (model == null || model.VideoId <= 0)
        {
            return BadRequest("Invalid request: VideoId is required.");
        }

        try
        {
            var videoProgress = await _context.UserVideoProgresses
                .FirstOrDefaultAsync(vp => vp.UserId == userId && vp.VideoId == model.VideoId);

            if (videoProgress == null)
            {
                videoProgress = new UserVideoProgress
                {
                    UserId = userId,
                    VideoId = model.VideoId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };
                _context.UserVideoProgresses.Add(videoProgress);
            }
            else if (!videoProgress.IsCompleted)
            {
                videoProgress.IsCompleted = true;
                videoProgress.CompletedAt = DateTime.UtcNow;
                _context.UserVideoProgresses.Update(videoProgress);
            }

            await _context.SaveChangesAsync();

            await CheckLessonCompletion(userId, model.VideoId);

            return Ok(new { message = "Video marked as completed!", videoId = model.VideoId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("lesson/{lessonId}/complete")]
    public async Task<IActionResult> MarkLessonCompleted(int lessonId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not found");
        }

        try
        {
            var lessonVideos = await _context.Videos
                .Where(v => v.LessonId == lessonId)
                .Select(v => v.Id)
                .ToListAsync();

            var completedVideos = await _context.UserVideoProgresses
                .Where(vp => vp.UserId == userId && lessonVideos.Contains(vp.VideoId) && vp.IsCompleted)
                .Select(vp => vp.VideoId)
                .ToListAsync();

            if (completedVideos.Count == lessonVideos.Count)
            {
                var lessonProgress = await _context.UserLessonProgresses
                    .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId);

                if (lessonProgress == null)
                {
                    lessonProgress = new UserLessonProgress
                    {
                        UserId = userId,
                        LessonId = lessonId,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow
                    };
                    _context.UserLessonProgresses.Add(lessonProgress);
                }
                else if (!lessonProgress.IsCompleted)
                {
                    lessonProgress.IsCompleted = true;
                    lessonProgress.CompletedAt = DateTime.UtcNow;
                    _context.UserLessonProgresses.Update(lessonProgress);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Lesson marked as completed!" });
            }
            else
            {
                return BadRequest("Not all videos in the lesson are completed.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    private async Task CheckLessonCompletion(string userId, int videoId)
    {
        var video = await _context.Videos
            .Include(v => v.Lesson)
            .ThenInclude(l => l.Course)
            .FirstOrDefaultAsync(v => v.Id == videoId);

        if (video?.Lesson == null)
        {
            return;
        }

        var lessonId = video.LessonId;
        var lessonVideos = await _context.Videos
            .Where(v => v.LessonId == lessonId)
            .Select(v => v.Id)
            .ToListAsync();

        var completedVideos = await _context.UserVideoProgresses
            .Where(vp => vp.UserId == userId && lessonVideos.Contains(vp.VideoId) && vp.IsCompleted)
            .Select(vp => vp.VideoId)
            .ToListAsync();

        if (completedVideos.Count == lessonVideos.Count)
        {
            await MarkLessonCompleted(userId, lessonId, video.Lesson.CourseId);
        }
    }

    private async Task MarkLessonCompleted(string userId, int lessonId, int courseId)
    {
        var lessonProgress = await _context.UserLessonProgresses
            .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId);

        if (lessonProgress == null)
        {
            lessonProgress = new UserLessonProgress
            {
                UserId = userId,
                LessonId = lessonId,
                CourseId = courseId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            };
            _context.UserLessonProgresses.Add(lessonProgress);
        }
        else if (!lessonProgress.IsCompleted)
        {
            lessonProgress.IsCompleted = true;
            lessonProgress.CompletedAt = DateTime.UtcNow;
            _context.UserLessonProgresses.Update(lessonProgress);
        }

        await _context.SaveChangesAsync();
        await CheckCourseCompletion(userId, courseId);
    }

    private async Task CheckCourseCompletion(string userId, int courseId)
    {
        var courseLessons = await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .Select(l => l.Id)
            .ToListAsync();

        var completedLessons = await _context.UserLessonProgresses
            .Where(lp => lp.UserId == userId && courseLessons.Contains(lp.LessonId) && lp.IsCompleted)
            .Select(lp => lp.LessonId)
            .ToListAsync();

        if (completedLessons.Count == courseLessons.Count)
        {
            var userCourseProgress = await _context.UserCourseProgresses
                .FirstOrDefaultAsync(cp => cp.UserId == userId && cp.CourseId == courseId);

            if (userCourseProgress == null)
            {
                userCourseProgress = new UserCourseProgress
                {
                    UserId = userId,
                    CourseId = courseId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };
                _context.UserCourseProgresses.Add(userCourseProgress);
            }
            else if (!userCourseProgress.IsCompleted)
            {
                userCourseProgress.IsCompleted = true;
                userCourseProgress.CompletedAt = DateTime.UtcNow;
                _context.UserCourseProgresses.Update(userCourseProgress);
            }

            await _context.SaveChangesAsync();
        }
    }

    [HttpGet("video/complete")]
    public async Task<IActionResult> GetCompletedVideos()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not found.");
        }

        try
        {
            var completedVideos = await _context.UserVideoProgresses
                .Where(vp => vp.UserId == userId && vp.IsCompleted)
                .Select(vp => vp.VideoId)
                .ToListAsync();

            return Ok(new { completedVideoIds = completedVideos });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("course/{courseId}/status")]
    public async Task<IActionResult> GetCourseProgress(int courseId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not found");
        }

        try
        {
            var courseLessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .Select(l => l.Id)
                .ToListAsync();

            var completedLessons = await _context.UserLessonProgresses
                .Where(lp => lp.UserId == userId &&
                            courseLessons.Contains(lp.LessonId) &&
                            lp.IsCompleted)
                .Select(lp => lp.LessonId)
                .ToListAsync();

            var courseProgress = await _context.UserCourseProgresses
                .FirstOrDefaultAsync(cp => cp.UserId == userId &&
                                         cp.CourseId == courseId);

            return Ok(new
            {
                totalLessons = courseLessons.Count,
                completedLessons = completedLessons.Count,
                isCompleted = courseProgress?.IsCompleted ?? false,
                completedAt = courseProgress?.CompletedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
