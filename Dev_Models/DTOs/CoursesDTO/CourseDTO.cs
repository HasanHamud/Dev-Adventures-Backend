using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.DTOs.Courses
{
    public class CourseDTO
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Rating { get; set; }
        public decimal Price { get; set; }

        public String ImgURL { get; set; }
    }
}
