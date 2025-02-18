using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev_Models.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        [NotMapped]
        public int? Length
        {
            get
            {
                return Videos?.Sum(v => v.Length) ?? 0;
            }
        }

        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public List<Video> Videos { get; set; } = new List<Video>();
        public List<LearningOutcome> LearningOutcomes { get; set; } = new List<LearningOutcome>();
        public Quiz Quiz { get; set; }
        public bool HasQuiz { get; set; }
    }
}