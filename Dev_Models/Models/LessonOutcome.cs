using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev_Models.Models
{
    public class LearningOutcome
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public Lesson Lesson { get; set; }
    }
}