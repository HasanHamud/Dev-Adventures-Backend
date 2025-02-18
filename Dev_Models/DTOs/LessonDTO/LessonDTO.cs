namespace Dev_Models.DTOs.LessonDTO
{
    public class LessonDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public int CourseId { get; set; }
        public List<string> LearningOutcomes { get; set; }
    }
}