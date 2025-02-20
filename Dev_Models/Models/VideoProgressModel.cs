using System.ComponentModel.DataAnnotations;

namespace Dev_Models.Models
{
    public class UserVideoProgress
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int VideoId { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

    }
}
