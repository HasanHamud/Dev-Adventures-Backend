using Dev_Models.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Certificate
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int CourseId { get; set; }

    public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

    public string CertificateUrl { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; }
}
