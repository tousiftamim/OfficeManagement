using System.ComponentModel.DataAnnotations.Schema;

namespace Office_Management.Models
{
    [Table("EmpRegInfo")]
    public class EmpRegInfo
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int OfficeId { get; set; }
        public bool IsAccept { get; set; }
    }
}