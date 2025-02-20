namespace Dev_Models.DTOs.LessonDTO
{
    public class LessonProgressUpdateDto
    {
        public string UserId { get; set; }
        public int LessonId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
