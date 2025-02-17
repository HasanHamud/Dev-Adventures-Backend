namespace Dev_Models.DTOs.UserCourseDTO
{
    public class UserCourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Progress { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string ImgURL { get; set; }
        public decimal Rating { get; set; }
        public decimal Price { get; set; }
        public string Duration { get; set; }
        public string Level { get; set; }
        public string Language { get; set; }
    }
}
