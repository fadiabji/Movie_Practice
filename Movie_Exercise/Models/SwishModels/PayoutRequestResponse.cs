using System;
using System.Collections.Generic;
using System.Text;

namespace Movie_Exercise.Models.SwishModels
{
    public class PayoutRequestResponse
    {
        public string Error { get; set; }
        public string Location { get; set; }
        public string JSON { get; set; }
        public PayoutRequestData Payload { get; set; }
    }
}
