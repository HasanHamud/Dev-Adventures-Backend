using Dev_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.DTOs.PlanDTO
{
    public class CreatePlanDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Discount { get; set; }
        public Dev_Models.Models.Plan.PlanLevel Level { get; set; }
        public List<int> CourseIds { get; set; }
    }
}
