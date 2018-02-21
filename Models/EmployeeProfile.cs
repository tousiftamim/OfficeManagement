
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Office_Management.Models
{
    [Table("EmployeeProfile")]
    public class EmployeeProfile
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int EmployeeId { get; set; }



    }
}