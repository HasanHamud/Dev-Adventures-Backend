using Dev_Db.Data;
using Dev_Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;

[Route("api/certificates")]
[ApiController]
[Authorize]
public class CertificateController : ControllerBase
{
    private readonly Dev_DbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CertificateController(Dev_DbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet("check/{userId}/{courseId}")]
    public async Task<IActionResult> CheckCourseCompletion(string userId, int courseId)
    {
        var allLessons = await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .Select(l => l.Id)
            .ToListAsync();

        var completedLessons = await _context.UserLessonProgresses
            .Where(p => p.UserId == userId && p.CourseId == courseId && p.IsCompleted)
            .Select(p => p.LessonId)
            .ToListAsync();

        bool isCompleted = allLessons.All(l => completedLessons.Contains(l));
        return Ok(new { courseCompleted = isCompleted });
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCertificate([FromBody] CertificateRequest model)
    {
        var existingCertificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == model.UserId && c.CourseId == model.CourseId);

        if (existingCertificate != null)
        {
            return BadRequest(new { message = "Certificate already exists!", url = existingCertificate.CertificateUrl });
        }

        var allLessons = await _context.Lessons
            .Where(l => l.CourseId == model.CourseId)
            .Select(l => l.Id)
            .ToListAsync();

        var completedLessons = await _context.UserLessonProgresses
            .Where(p => p.UserId == model.UserId && p.CourseId == model.CourseId && p.IsCompleted)
            .Select(p => p.LessonId)
            .ToListAsync();

        if (!allLessons.All(l => completedLessons.Contains(l)))
        {
            return BadRequest(new { message = "User has not completed all lessons!" });
        }

        var user = await _context.Users.FindAsync(model.UserId);
        var course = await _context.Courses.FindAsync(model.CourseId);

        if (user == null || course == null)
        {
            return NotFound(new { message = "User or course not found!" });
        }

        string fileName = $"{model.UserId}-{model.CourseId}.pdf";
        var certificatePath = await GeneratePdfCertificate(user, course, fileName);

        string certificateUrl = $"/certificates/{fileName}";

        var certificate = new Certificate
        {
            UserId = model.UserId,
            CourseId = model.CourseId,
            IssuedDate = DateTime.UtcNow,
            CertificateUrl = certificateUrl
        };

        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Certificate generated successfully!",
            certificateUrl
        });
    }

    public class CertificateRequest
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }
    }


    [HttpGet("download/{courseId}")]
    public async Task<IActionResult> DownloadCertificate(int courseId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);

        if (certificate == null)
        {
            return NotFound(new { message = "Certificate not found!" });
        }

        string fileName = $"{userId}-{courseId}.pdf";
        var filePath = Path.Combine(_environment.WebRootPath, "certificates", fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { message = "Certificate file not found!" });
        }

        var memory = new MemoryStream();
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;

        return File(memory, "application/pdf", $"Certificate.pdf");
    }

    private async Task<string> GeneratePdfCertificate(User user, Course course, string fileName)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.Background().Image(_environment.WebRootPath + "/images/certificate-background.jpg");

                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        row.RelativeItem().Image(_environment.WebRootPath + "/images/logo.png").FitHeight();
                        row.RelativeItem().AlignCenter().Text("Certificate of Completion").FontSize(28).Bold();
                    });
                });

                page.Content().Element(content =>
                {
                    content.Column(column =>
                    {
                        column.Item().AlignCenter().Text($"This is to certify that").FontSize(16);
                        column.Item().AlignCenter().Text(user.UserName).FontSize(24).Bold();
                        column.Item().AlignCenter().Text("has successfully completed the course").FontSize(16);
                        column.Item().AlignCenter().Text(course.Title).FontSize(20).Bold();
                        column.Item().AlignCenter().Text($"Completed on {DateTime.UtcNow:MMMM dd, yyyy}").FontSize(16);
                    });
                });
            });
        });

        string certificatesPath = Path.Combine(_environment.WebRootPath, "certificates");
        Directory.CreateDirectory(certificatesPath);

        string filePath = Path.Combine(certificatesPath, fileName);

        document.GeneratePdf(filePath);
        return filePath;
    }
}