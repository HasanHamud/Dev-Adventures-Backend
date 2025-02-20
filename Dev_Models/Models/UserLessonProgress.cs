using Dev_Models.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class UserLessonProgress
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int LessonId { get; set; }
    public int CourseId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }

    [ForeignKey("LessonId")]
    public Lesson Lesson { get; set; }
    public Course Course { get; set; }

    public List<int> CompletedVideos { get; set; } = new();
}
