using System.ComponentModel.DataAnnotations;

namespace Dev_Models.Models
{
    public class CourseLearningObjective
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}