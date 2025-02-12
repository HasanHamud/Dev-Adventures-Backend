using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.DTOs.QuizDTO
{
    public class QuizSubmissionDTO
    {
        public int QuizId { get; set; }
        public Dictionary<int, int> Answers { get; set; } 
    }
}
