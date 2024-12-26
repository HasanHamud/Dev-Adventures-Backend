using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dev_Models
{
    public class User : IdentityUser
    {


        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Fullname { get; set; }

        public string? ProfileImage { get; set; }
    }
}
