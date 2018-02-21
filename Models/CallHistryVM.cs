using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    public class CallHistryVM
    {
        public int Id { get; set; }
        public string CallerName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string OrderCode { get; set; }
        public string PaymentMethod { get; set; }
        public string UserName { get; set; }
//        public bool OutboundAdd { get; set; }

        public string CallTypeName { get; set; }

        public string AssignerName { get; set; }
    }
}