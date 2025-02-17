using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.DTOs.QuizDTO
{
    public class QuizQuestionDTO
    {

        public int Id { get; set; }
        public string Text { get; set; }
        public List<QuizAnswerDTO> Answers { get; set; }
    }
}
