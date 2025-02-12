using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.DTOs.QuizDTO
{
    public class QuizAnswerDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public bool? IsCorrect { get; set; }

    }
}
