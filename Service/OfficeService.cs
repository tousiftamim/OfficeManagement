using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Office_Management.Models;
using Office_Management.Models.ViewModel;
using Office_Management.Repository;
using WebMatrix.WebData;

namespace Office_Management.Service
{
    public class OfficeService
    {
        public EmployeeRepository EmployeeRepository = new EmployeeRepository();
        public OfficeRepository OfficeRepository = new OfficeRepository();
        DatabaseContex dbContex = new DatabaseContex();
        public List<OfficeViewModel> GetAllOfficeInfos(int officeOwnerId)
        {
            var officeInfos = new List<OfficeViewModel>();
            var showOfficeList = OfficeRepository.GetAllByOfficeOwnerId(officeOwnerId);
            foreach (var officeList in showOfficeList)
            {
                var employeeCount = EmployeeRepository.GetEmployeeCount(officeList.Id);
                officeInfos.Add(new OfficeViewModel()
                {
                    OfficeName = officeList.OfficeName,
                    EmployeeCount = employeeCount,
                    OfficeId = officeList.Id
                });
            }
            return officeInfos;
        }

        public List<UserProfile> GetAllEmployees(int currentUserId)
        {
            var allOffices = OfficeRepository.GetAllByOfficeOwnerId(currentUserId);
            var listOfficeEmployees = new List<UserProfile>();
            foreach (var office in allOffices)
            {
                var employees = EmployeeRepository.GetEmployeeByOfficeId(office.Id);
                listOfficeEmployees.AddRange(employees);
            }

            return listOfficeEmployees;
        }


        public List<TaskInfoVM> GetDoneWork(int currentUserId)
        {
            var officeInfo = new OfficeInfo();
            var data = (from o in dbContex.OfficeInfos
                       join t in dbContex.TaskInfos on o.Id equals t.OfficeId
                       join u in dbContex.UserProfiles on t.EmployeeId equals u.UserId
                       where t.TaskStatus && o.OwnerId == currentUserId
                       select new TaskInfoVM()
                       {
                           Id = t.Id,
                           EmployeeId = t.EmployeeId,
                           TaskDescription = t.TaskDescription,
                           OfficeName = o.OfficeName,
                           TaskDeadline = t.TaskDeadline,
                           TaskName = t.TaskName,
                           EmployeeName = u.UserName,
                           IsAccept = t.IsAccept

                       }).ToList();
            return data;
        }

        public List<SideBar> GetEmployeeOffice(int currentUserId)
        {
           var userProfile = new UserProfile();
           var data = (from o in dbContex.OfficeInfos
                       join e in dbContex.EmpRegInfos on o.Id equals e.OfficeId
                       join u in dbContex.UserProfiles on e.EmpId equals u.UserId
                       where o.OwnerId == currentUserId
                       select new SideBar()
                       {
                           OfficeName = o.OfficeName,
                           EmployeeName = u.UserName,
                           UserId = u.UserId
                       }).ToList();
            return data;
        }
    }
}