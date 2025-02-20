using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev_Models.Models
{
    public class Course
    {
        public readonly object UserLessonProgresses;

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        [Range(0, 10)]
        public decimal Rating { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        [Range(-1, 9999.99)]
        public decimal Price { get; set; }

        public string ImgURL { get; set; }

        [Required]
        public CourseStatus Status { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public CourseLevel Level { get; set; }

        [Required]
        public string Language { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
        public List<CourseRequirement> Requirements { get; set; } = new List<CourseRequirement>();
        public List<CourseLearningObjective> LearningObjectives { get; set; } = new List<CourseLearningObjective>();
        public List<UserCourse> UserCourses { get; set; }

        // Updated relationships
        public List<Cart> carts { get; set; }
        public List<PlansCourses> PlansCourses { get; set; } = new List<PlansCourses>();

        public enum CourseStatus
        {
            Published,
            InProgress
        }

        public enum CourseLevel
        {
            Beginner,
            Intermediate,
            Advanced
        }

        public string? previewURL { get; set; }
    }
}