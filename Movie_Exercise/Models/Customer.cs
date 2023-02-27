using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;

namespace Movie_Exercise.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required] 
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string BillingAddress { get; set; }
        [Required]
        public int BillingZip { get; set; }
        [Required]
        public string BillingCity { get; set; }
        [Required]
        public string DeliveryAddress { get; set; }
        [Required]
        public int DeliveryZip { get; set; }
        [Required]
        public string DeliveryCity { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public int PhoneNumber { get; set; }   

        public virtual ICollection<Order> Orders { get; set; }
    }
}
