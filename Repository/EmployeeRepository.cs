using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Office_Management.Models;

namespace Office_Management.Repository
{
    public class EmployeeRepository
    {
        DatabaseContex dbContex = new DatabaseContex();

        public EmpRegInfo Get(int id)
        {
            return dbContex.EmpRegInfos.Find(id);
        }

        public int GetEmployeeCount(int officeId)
        {
            return dbContex.EmpRegInfos.Count(_ => _.OfficeId == officeId && _.IsAccept);
        }

        public List<UserProfile> GetEmployeeByOfficeId(int officeId)
        {
            var employees = (from e in dbContex.EmpRegInfos
                join u in dbContex.UserProfiles on e.EmpId equals u.UserId
                where e.IsAccept && e.OfficeId == officeId
                select u).ToList();
            return employees;
        }
       
    }
}