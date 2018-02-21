using System.Data.Entity;

namespace Office_Management.Models
{
    public class DatabaseContex : DbContext
    {
        public DatabaseContex()
            : base("DefaultConnection")
        {
        }
        public DbSet<OfficeInfo> OfficeInfos { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<EmpRegInfo> EmpRegInfos { get; set;  }
        public DbSet<TaskInfo> TaskInfos { get; set; }
        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
        public DbSet<CallRegistration> CallRegistrations { get; set; }
        public DbSet<CallType> CallTypes { get; set; } 
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<CallingStatus> CallingStatuses { get; set; }
        public DbSet<OutboundCallReg> OutboundCallRegs { get; set; }      
 
    }
}