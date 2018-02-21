using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    public class TaskInfoVM
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime TaskDeadline { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string OfficeName { get; set; }
        public bool IsAccept { get; set; }
        public int Id { get; set; }
        public bool TaskStatus { get; set; }


    }
}