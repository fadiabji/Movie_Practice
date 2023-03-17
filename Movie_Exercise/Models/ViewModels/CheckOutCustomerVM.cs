namespace Movie_Exercise.Models.ViewModels
{
    public class CheckOutCustomerVM
    {
        public string Email { get; set; }
        public string BillingAddress { get; set; }
        public int BillingZip { get; set; }
        public string BillingCity { get; set; }
        public string DeliveryAddress { get; set; }
        public int DeliveryZip { get; set; }
        public string DeliveryCity { get; set; }
        public int PhoneNumber { get; set; }

    }
}
