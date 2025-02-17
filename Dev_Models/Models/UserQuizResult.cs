using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.Models
{
    public class UserQuizResult
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
