namespace Dev_Models.DTOs.Courses
{
    public class CreateCourseRequestDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public decimal Price { get; set; }
        public string ImgURL { get; set; }
        public string Level { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
    }
}
