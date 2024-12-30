using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dev_Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public required string Fullname { get; set; }

        public string? ProfileImage { get; set; }
    }
}
