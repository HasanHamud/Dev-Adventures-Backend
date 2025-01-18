using System.ComponentModel.DataAnnotations.Schema;


namespace Dev_Models.Models
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

        public string ImgURL { get; set; }

        public List<Cart> carts { get; set; }


        public List<User> users { get; set; }

         
    }
}
