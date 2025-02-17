using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev_Models.Models
{
    public class PlansCourses
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("PlanId")]
        public Plan Plan { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public int OrderIndex { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}