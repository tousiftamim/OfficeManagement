using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    [Table("TaskInfo")]
    public class TaskInfo
    {
        public int Id { get; set; }    
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime TaskDeadline { get; set; }
        public bool TaskStatus { get; set; }
        public int EmployeeId { get; set; }
        public int SupervisorId { get; set; }
        public int OfficeId { get; set; }
        public bool IsAccept { get; set; }
        public DateTime DoneDateByEmp { get; set; }
        public DateTime AcceptDoneDateByOwner { get; set; }
        public DateTime OwnerSetTaskDate { get; set; }


    }
}