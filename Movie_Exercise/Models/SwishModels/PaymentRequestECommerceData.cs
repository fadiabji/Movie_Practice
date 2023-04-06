using System;
using System.Collections.Generic;
using System.Text;

namespace Movie_Exercise.Models.SwishModels
{
    public class PaymentRequestECommerceData
    {
        public string payeePaymentReference { get; set; }
        public string callbackUrl { get; set; }
        public string payerAlias { get; set; }
        public string payeeAlias { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string message { get; set; }
    }
}
