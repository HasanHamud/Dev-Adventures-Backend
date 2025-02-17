using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.Models
{
    public class Quiz
    {


        public int Id { get; set; }
        public string Title { get; set; }
        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public Lesson Lesson { get; set; }
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    }


}

