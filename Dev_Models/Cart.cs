using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev_Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public decimal totalPrice { get; set; } = 0;

        public List<Course> courses { get; set; } = new List<Course>();


        public Cart()
        {

        }

        public Cart(string UID)
        {
            UserId = UID;
        }

    }
}
