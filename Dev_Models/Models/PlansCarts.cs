using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev_Models.Models
{
    public class PlansCarts
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        public int CartId { get; set; }

        [ForeignKey("PlanId")]
        public Plan Plan { get; set; }

        [ForeignKey("CartId")]
        public Cart Cart { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal AppliedPrice { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}