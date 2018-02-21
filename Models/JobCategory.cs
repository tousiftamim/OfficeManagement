using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    [Table("JobCategory")]
    public class JobCategory
    {
        public int Id { get; set; }
        public string Category { get; set; }
    }
}