﻿namespace Dev_Models.DTOs.Courses
{
    public class CourseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public decimal Price { get; set; }
        public string ImgURL { get; set; }
        public string Level { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }


        public string previewURL { get; set; }


        public List<string> Requirements { get; set; } = new List<string>();
        public List<string> LearningObjectives { get; set; } = new List<string>();
    }
}
