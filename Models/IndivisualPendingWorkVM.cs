using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    public class IndivisualPendingWorkVM
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime TaskDeadline { get; set; }

    }
}