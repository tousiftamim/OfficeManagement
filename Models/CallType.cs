using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Office_Management.Models
{
    [Table("CallerType")]
    public class CallType
    {
        public int Id { get; set; }
        public string CallTypeName { get; set; }
    }
}