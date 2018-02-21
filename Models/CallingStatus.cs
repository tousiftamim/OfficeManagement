using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    [Table("CallingStatus")]
    public class CallingStatus
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}