using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    public class PendingTaskVM
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime TaskDeadline { get; set; }
        public string EmployeeName { get; set; }
        public string OfficeName { get; set; }
    }
}