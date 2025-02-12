using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.Models
{
    public class Quiz
    {

        public int Id { get; set; }

        public string LessonId { get; set; }

        public Lesson Lesson { get; set; }  

        public List<Question> Questions { get; set; } = new List<Question>(); 

        public int Grade { get; set; }



    }
}
