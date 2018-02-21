using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office_Management.Models.ViewModel
{
    public class CallHistoryViewModel
    {
        public int Id { get; set; }
        public string CallerName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
        public string CallerEmail { get; set; }
        public string OrderCode { get; set; }
        public string ProductCode { get; set; }
        public string ComplainType { get; set; }
        public string QueryType { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int CallTypeId { get; set; }
        public string CallTypeName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string CallingStatusId { get; set; }
        public int OfficeOwnerId { get; set; }
        public string JobCategoryId { get; set; }
        public string AdminComment { get; set; }
        public string CallDuration { get; set; }
    }
}