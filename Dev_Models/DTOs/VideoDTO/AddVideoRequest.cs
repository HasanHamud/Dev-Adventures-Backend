using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models.DTOs.VideoDTO
{
    public class AddVideoRequest
    {
        public string Title { get; set; }

        public string VideoURL { get; set; }
    }
}
