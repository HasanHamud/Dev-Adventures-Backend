using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required]
    [MaxLength(50)]
    public string Fullname { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }

}
