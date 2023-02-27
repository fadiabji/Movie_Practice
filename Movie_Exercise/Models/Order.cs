using System.ComponentModel.DataAnnotations;

namespace Movie_Exercise.Models
{
    public class Order
    {
        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
