using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    public class CallRegReportGenerateVM
    {
        public string CallerName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime DeliveryDate { get; set; }

    }
}