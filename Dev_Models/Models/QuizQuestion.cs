using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public List<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
    }
}
