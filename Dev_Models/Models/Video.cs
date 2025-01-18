using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev_Models.Models
{
    public class Video
    {

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string VideoURL { get; set; }

        [Required]
        public int Length { get; set; }

        [Required]
        public int LessonId { get; set; }

        [ForeignKey("LessonId")]
        public Lesson Lesson { get; set; }


    }
}
