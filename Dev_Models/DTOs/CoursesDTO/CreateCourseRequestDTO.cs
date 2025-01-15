using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.DTOs.Courses
{
    public class CreateCourseRequestDTO
    {

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Rating { get; set; }
        public decimal Price { get; set; }

        public String ImgURL { get; set; }
    }
}
