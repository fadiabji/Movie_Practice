using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;

namespace Movie_Exercise.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Billing Address")]
        public string BillingAddress { get; set; }
        [Required]
        [Display(Name = "Billing Zip")]
        public int BillingZip { get; set; }
        [Required]
        [Display(Name = "Billing City")]
        public string BillingCity { get; set; }
        [Required]
        [Display(Name = "Delivery Address")]

        public string DeliveryAddress { get; set; }
        [Required]
        [Display(Name = "Delivery Zip")]
        public int DeliveryZip { get; set; }
        [Required]
        [Display(Name = "Delivery City")]
        public string DeliveryCity { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public int PhoneNumber { get; set; }   

        public virtual ICollection<Order> Orders { get; set; }
    }
}
