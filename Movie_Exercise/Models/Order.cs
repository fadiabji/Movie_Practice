using System.ComponentModel.DataAnnotations;

namespace Movie_Exercise.Models
{
    public class Order
    {
        [Required]   
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Required]
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
