using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    [Table("OutboundCallReg")]
    public class OutboundCallReg
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Reason { get; set; }
        public int AssignerId { get; set; }
        public int OfficeId { get; set; }
        public string AssignerComment { get; set; }
        public bool IsDone { get; set; }
        public int CallingStatusId { get; set; }

    }
}