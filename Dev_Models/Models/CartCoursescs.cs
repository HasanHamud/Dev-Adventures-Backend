using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.Models
{
    public class CartCoursescs
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int CartId { get; set; }

        public Cart Cart { get; set; }

    }
}
