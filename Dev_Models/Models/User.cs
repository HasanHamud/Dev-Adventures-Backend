using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dev_Models.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public required string Fullname { get; set; }

        public string? ProfileImage { get; set; }

        public Cart userCart { get; set; }

        public List<Course> courses { get; set; }

        public List<UserCourse> UserCourses { get; set; }
    }
}
