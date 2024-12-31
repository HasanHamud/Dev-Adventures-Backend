using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dev_Models
{
    public class Course
    {

       
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        public decimal Rating { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal Price { get; set; }

        public String ImgURL { get; set; }

    }
}
