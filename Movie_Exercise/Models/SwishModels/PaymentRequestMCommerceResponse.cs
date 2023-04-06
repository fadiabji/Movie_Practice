using System;
using System.Collections.Generic;
using System.Text;

namespace Movie_Exercise.Models.SwishModels
{
    /// <summary>
    /// Response object from a Swish for Merchant Payment Request
    /// </summary>
    public class PaymentRequestMCommerceResponse
    {
        public string Error { get; set; }
        public string Token { get; set; }
        public string Location { get; set; }
    }
}
