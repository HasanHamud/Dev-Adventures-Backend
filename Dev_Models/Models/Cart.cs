using System;
using System.Collections.Generic;

namespace Dev_Models.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal totalPrice { get; set; } = 0;
        
        public List<Course> courses { get; set; } = new List<Course>();
        public List<PlansCarts> PlansCarts { get; set; } = new List<PlansCarts>();

        public Cart()
        {
        }

        public Cart(string UID)
        {
            UserId = UID;
        }
    }
}