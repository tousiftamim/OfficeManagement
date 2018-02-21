using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    [Table("OfficeInfo")]
    public class OfficeInfo
    {
        public int Id { get; set; }
        public string OfficeName { get; set; }
        public string Address { get; set; }
        public int OwnerId { get; set; }
        public int EmployeeNumber { get; set; }

    }
}