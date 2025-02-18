using System.ComponentModel.DataAnnotations;

public class LearningOutcomeDto
{
    [Required]
    public string Description { get; set; }
    [Required]
    public int LessonId { get; set; }
}
