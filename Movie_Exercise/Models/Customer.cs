using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;

namespace Movie_Exercise.Models
{
    public class Customer
    {
        public Customer(string emailAddress, string firstName, string lastName, int phoneNumber, string billingAddress, string billingCity, int billingZip, string deliveryAddress, string deliveryCity, int deliveryZip)
        {
            EmailAddress = emailAddress;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            BillingAddress = billingAddress;
            BillingCity = billingCity;
            BillingZip = billingZip;
            DeliveryAddress = deliveryAddress;
            DeliveryCity = deliveryCity;
            DeliveryZip = deliveryZip;
        }
        public Customer(){}


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
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "You must provide a phone number")]
        [DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number ex 8005551212")]
        public int PhoneNumber { get; set; }   

        public virtual ICollection<Order> Orders { get; set; }
    }
}
