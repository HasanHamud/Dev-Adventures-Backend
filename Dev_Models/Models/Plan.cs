using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dev_Models.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public int Discount { get; set; }
        public PlanLevel Level { get; set; }
        public decimal totalPrice { get; set; }

        public List<PlansCourses> PlansCourses { get; set; } = new List<PlansCourses>();
        public List<PlansCarts> PlansCarts { get; set; } = new List<PlansCarts>();

        public enum PlanLevel
        {
            Beginner,
            Intermediate,
            Advanced
        }
    }
}
