using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.DTOs.QuizDTO
{
    public class QuizDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int LessonId { get; set; }
        public List<QuizQuestionDTO> Questions { get; set; }
    }
}
