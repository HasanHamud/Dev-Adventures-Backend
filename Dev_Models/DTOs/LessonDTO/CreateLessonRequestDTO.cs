using Dev_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.DTOs.LessonDTO
{
    public class CreateLessonRequestDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }



    }
}
