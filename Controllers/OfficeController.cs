using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Office_Management.Filters;
using Office_Management.Models;
using Office_Management.Models.ViewModel;
using Office_Management.Repository;
using Office_Management.Service;
using WebGrease.Css.Ast.Selectors;
using WebMatrix.WebData;

namespace Office_Management.Controllers
{
    [Authorize(Roles = "OfficeOwner")]
    [InitializeSimpleMembership]
    public class OfficeController : Controller
    {
        DatabaseContex dbContex = new DatabaseContex();
        public OfficeService OfficeService = new OfficeService();
        private OfficeRepository _officeRepository = new OfficeRepository();
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult PendingEmployees()
        {
            var officeInfos = dbContex.OfficeInfos.Where(_ => _.OwnerId == WebSecurity.CurrentUserId).ToList();
            var pendingEmployees = new List<PendingEmployee>();
            foreach (var officeInfo in officeInfos)
            {
                var officeId = officeInfo.Id;
                var emp =
                    dbContex.EmpRegInfos.Where(_ => _.IsAccept == false && _.OfficeId == officeId).ToList();
                var employees = emp.Select(_ => dbContex.UserProfiles.Find(_.EmpId)).ToList();
                foreach (var employee in employees)
                {
                    pendingEmployees.Add(new PendingEmployee()
                    {
                        UserId = employee.UserId,
                        UserName = employee.UserName,
                        OfficeName = officeInfo.OfficeName
                    });
                }
            }

            return View(pendingEmployees);
        }

        public ActionResult Approve(int userId)
        {
            var employee = dbContex.EmpRegInfos.FirstOrDefault(_ => _.EmpId == userId);
            employee.IsAccept = true;
            dbContex.EmpRegInfos.AddOrUpdate(employee);
            dbContex.SaveChanges();

            var userName = dbContex.UserProfiles.Find(userId).UserName;
            Roles.AddUserToRole(userName, "Employee");
            return RedirectToAction("PendingEmployees");
        }

        public ActionResult Delete(int id)
        {
            var employee = dbContex.EmpRegInfos.FirstOrDefault(_ => _.EmpId == id);
            employee.IsAccept = false;
            dbContex.EmpRegInfos.Remove(employee);
            dbContex.SaveChanges();
            return RedirectToAction("PendingEmployees");
        }


        [Authorize]
        public ActionResult CreateOffice()
        {
            //var isParmitted = dbContex.OfficeOwners.Where(officeowner => officeowner.OfficeOwnerId == WebSecurity.CurrentUserId);

            if (Roles.IsUserInRole("OfficeOwner"))
            {
                return View();
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult CreateOffice(string officename, string address, int empnumber)
        {

            var createoffice = new OfficeInfo()
            {
                OfficeName = officename,
                Address = address,
                OwnerId = WebSecurity.CurrentUserId,
                EmployeeNumber = empnumber

            };
            dbContex.OfficeInfos.Add(createoffice);
            dbContex.SaveChanges();

            return RedirectToAction("EmployeeList");

        }

        public ActionResult OfficeOwnerHome()
        {
            return View(OfficeService.GetAllOfficeInfos(WebSecurity.CurrentUserId));
        }


        public ActionResult IndivisualOfficeEmployeeList(int officeId)
        {
            var indivisualOfficeEmployeeList = from e in dbContex.EmpRegInfos
                                               join o in dbContex.OfficeInfos on e.OfficeId equals o.Id
                                               join u in dbContex.UserProfiles on e.EmpId equals u.UserId
                                               where e.IsAccept && e.OfficeId == officeId

                                               select new EmployeeListVM()
                                               {
                                                   EmployeeName = u.UserName,
                                                   EmployeeId = u.UserId


                                               };

            return View(indivisualOfficeEmployeeList);
        }

        public ActionResult IndivisualDoneWork(int empId)
        {
            var indivisualDoneWork = from t in dbContex.TaskInfos
                                     where t.TaskStatus && empId == t.EmployeeId && t.IsAccept

                                     select new IndivisualDoneWorkVM()
                                     {
                                         TaskName = t.TaskName,
                                         TaskDescription = t.TaskDescription,
                                         TaskDeadline = t.TaskDeadline
                                     };
            return View(indivisualDoneWork);
        }

        public ActionResult IndivisualPendinWork(int empId)
        {
            var indivisualPendinWork = from t in dbContex.TaskInfos
                                       where empId == t.EmployeeId && t.TaskStatus == false
                                       select new IndivisualPendingWorkVM()
                                       {
                                           TaskName = t.TaskName,
                                           TaskDescription = t.TaskDescription,
                                           TaskDeadline = t.TaskDeadline
                                       };
            return View(indivisualPendinWork);
        }

        public ActionResult IndivisualWorkHistory(int empId)
        {
            var data = from t in dbContex.TaskInfos
                       where empId == t.EmployeeId
                       select new IndivisualWorkHistoryVM()
                       {
                           TaskName = t.TaskName,
                           TaskDescription = t.TaskDescription,
                           TaskDeadline = t.TaskDeadline
                       };
            return View(data);

        }
        public ActionResult EmployeeList()
        {

            var showclintlist = OfficeService.GetAllEmployees(WebSecurity.CurrentUserId);
            
            return View(showclintlist);
        }
        public ActionResult EmployeeListPartial()
        {

            var showclintlist = OfficeService.GetEmployeeOffice(WebSecurity.CurrentUserId);

            return PartialView("~/Views/Shared/_EmployeeListPartial.cshtml", showclintlist);
        }
        public ActionResult ErrorSetTask()
        {
            return View();
        }

        public ActionResult SetTask(int empId)
        {
            var settask = dbContex.TaskInfos.Where(emp => emp.Id == empId);
            ViewBag.profile = dbContex.UserProfiles.Find(empId);
            return View(settask);
        }


        [HttpPost]
        public ActionResult SetTask(string taskname, string taskdescription, string taskdeadline, int empId)
        {
            if (Convert.ToDateTime(taskdeadline) > DateTime.Now)
            {
                var task = (from e in dbContex.EmpRegInfos
                            where (empId == e.EmpId)
                            select e).FirstOrDefault();


                var updatetask = new TaskInfo()
                {

                    TaskName = taskname,
                    TaskDescription = taskdescription,
                    TaskDeadline = Convert.ToDateTime(taskdeadline),
                    TaskStatus = false,
                    EmployeeId = empId,
                    SupervisorId = WebSecurity.CurrentUserId,
                    OfficeId = task.OfficeId,
                    DoneDateByEmp = new DateTime(2000, 01, 01),
                    AcceptDoneDateByOwner = new DateTime(2000, 01, 01),
                    OwnerSetTaskDate = DateTime.Now

                };
                dbContex.TaskInfos.Add(updatetask);
                dbContex.SaveChanges();
                return RedirectToAction("EmployeeList");
            }
            else return RedirectToAction("ErrorSetTask");

        }

       
        public ActionResult ShowDoneWork()
        {
            var showdonework = OfficeService.GetDoneWork(WebSecurity.CurrentUserId);

            return View(showdonework);
        }

        public ActionResult IsAcceptDoneWork(int taskId)
        {
            var task = dbContex.TaskInfos.Find(taskId);
            task.IsAccept = true;
            task.AcceptDoneDateByOwner = DateTime.Now;
            dbContex.TaskInfos.AddOrUpdate(task);
            dbContex.SaveChanges();
            return RedirectToAction("ShowDoneWork");
        }

        public ActionResult TodaysTask()
        {
            var data = from o in dbContex.OfficeInfos
                       join t in dbContex.TaskInfos on o.Id equals t.OfficeId
                       join u in dbContex.UserProfiles on t.EmployeeId equals u.UserId
                       where EntityFunctions.TruncateTime(t.OwnerSetTaskDate) == EntityFunctions.TruncateTime(DateTime.Now) && o.OwnerId == WebSecurity.CurrentUserId
                       select new TodaysTaskVM()
                       {
                           TaskName = t.TaskName,
                           TaskDescription = t.TaskDescription,
                           TaskDeadline = t.TaskDeadline,
                           EmployeeName = u.UserName,
                           OfficeName = o.OfficeName
                       };
            return View(data);
        }

        public ActionResult PendingTask()
        {
            var data = from o in dbContex.OfficeInfos
                join t in dbContex.TaskInfos on o.Id equals t.OfficeId
                join u in dbContex.UserProfiles on t.EmployeeId equals u.UserId
                where t.IsAccept == false && o.OwnerId == WebSecurity.CurrentUserId 
            
                       select new PendingTaskVM()
                       {
                           TaskName = t.TaskName,
                           TaskDescription = t.TaskDescription,
                           TaskDeadline = t.TaskDeadline,
                           EmployeeName = u.UserName,
                           OfficeName = o.OfficeName
                       };
            return View(data);

        }

        public ActionResult Register()
        {
            var officeOwnerId = WebSecurity.CurrentUserId;
            return View(_officeRepository.GetAllByOfficeOwnerId(officeOwnerId));
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(EmployeeRegistration model)
        {
            var officeOwnerId = WebSecurity.CurrentUserId;
            
            if (ModelState.IsValid)
            {
                
                try
                {
                    var userAlReadyExists =
                        dbContex.UserProfiles.Any(_ => _.UserName.ToLower() == model.UserName.ToLower());
                    if (userAlReadyExists)
                    {
                        ViewBag.ErrorMessage = "UserName Already Exists";
                        return View("RegisterError");
//                        return View(_officeRepository.GetAllByOfficeOwnerId(officeOwnerId));

                    }
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    var userId = WebSecurity.GetUserId(model.UserName);
                    dbContex.EmpRegInfos.Add(
                        new EmpRegInfo()
                        {
                            EmpId = userId,
                            IsAccept = true,
                            OfficeId = model.OfficeId
                        });
                    dbContex.SaveChanges();

                    Roles.AddUserToRole(model.UserName, "Employee");

                    return RedirectToAction("OfficeOwnerHome");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
                
            }
            return View();


        }

        private Exception ErrorCodeToString(MembershipCreateStatus statusCode)
        {
            throw new NotImplementedException();
        }

        public ActionResult TaskHistory()
        {
            var data = from o in dbContex.OfficeInfos
                       join t in dbContex.TaskInfos on o.Id equals t.OfficeId
                       join u in dbContex.UserProfiles on t.EmployeeId equals u.UserId
                       where o.OwnerId == WebSecurity.CurrentUserId
                       select new TaskHistory()
                       {
                           TaskName = t.TaskName,
                           TaskDescription = t.TaskDescription,
                           TaskDeadline = t.TaskDeadline,
                           EmployeeName = u.UserName,
                           OfficeName = o.OfficeName
                       };
            return View(data);
        }

        public ActionResult CallRegister()
        {
            
            return View();
        }

        public ActionResult CreateCallType()
        {

            return  View();
        }

        [HttpPost]
        public ActionResult CreateCallType(string callTypeName)
        {
            var createCalltype = new CallType()
            {
                CallTypeName = callTypeName
            };
            dbContex.CallTypes.Add(createCalltype);
            dbContex.SaveChanges();
            return RedirectToAction("CallRegister");

        }

        
        [HttpGet]
        public ActionResult ReportGenerate()
        {
            ViewBag.callerTypes = dbContex.CallTypes.ToList();
            return View();
        }
                       where c.OwnerId == WebSecurity.CurrentUserId

        [HttpPost]
        public ActionResult ReportGenerate(string from, string to, int callerTypeId)
        {
            var data = DataForReportGeneration(from, to, callerTypeId);
            ViewBag.dataCount = data.Count;
            ViewBag.from = from;
            ViewBag.to = to;

            ViewBag.callerTypeId = callerTypeId;

            var name = dbContex.CallTypes.Find(callerTypeId);

            ViewBag.callerTypeName = name.CallTypeName;
           
            return View("CallRegReportGenerateVM", data);
        }

        private List<CallRegReportGenerateVMforAll> DataForReportGeneration(string from, string to, int callerTypeId)
        {
            
            var stringFrom = Convert.ToDateTime(from);
            var stringTo = Convert.ToDateTime(to);

            var data = (from c in dbContex.CallRegistrations
                        where EntityFunctions.TruncateTime(c.Time) >= EntityFunctions.TruncateTime(stringFrom) &&
                              EntityFunctions.TruncateTime(c.Time) <= EntityFunctions.TruncateTime(stringTo) &&
                              c.CallTypeId == callerTypeId
                        select new CallRegReportGenerateVMforAll()
                        {
                            CallerName = c.CallerName,
                            MobileNumber = c.MobileNumber,
                            Email = c.CallerEmail,
                            Description = c.Description,
                            ComplainType = c.ComplainType,
                            QueryType = c.QueryType
                        }).ToList();

            return data;

        }

        public string ExportReportToExcel(string from, string to, int callerTypeId)
        {

            var datatable = new System.Data.DataTable("Table");

            datatable.Columns.Add("Caller Name", typeof (string));
            datatable.Columns.Add("Mobile Number", typeof (string));
            datatable.Columns.Add("Description", typeof(string));
            
            if (callerTypeId == 10)
            {
                datatable.Columns.Add("Emails", typeof(string));
            }
            else if (callerTypeId == 11)
            {
                datatable.Columns.Add("Complain Type", typeof(string));
            }
            else if (callerTypeId == 12)
            {
                datatable.Columns.Add("Quary Type", typeof(string));
            }
           
            

            var data = DataForReportGeneration(from, to, callerTypeId);
            
            foreach (var callRegReportGenerateVm in data)
            {

                if (callerTypeId == 10)
                {
                    datatable.Rows.Add(callRegReportGenerateVm.CallerName, callRegReportGenerateVm.MobileNumber,
                        callRegReportGenerateVm.Description,
                        callRegReportGenerateVm.Email);
                }
                else if (callerTypeId == 11)
                {
                    datatable.Rows.Add(callRegReportGenerateVm.CallerName, callRegReportGenerateVm.MobileNumber,
                        callRegReportGenerateVm.Description,
                        callRegReportGenerateVm.ComplainType);
                }
                else if (callerTypeId == 12)
                {
                    datatable.Rows.Add(callRegReportGenerateVm.CallerName, callRegReportGenerateVm.MobileNumber,
                        callRegReportGenerateVm.Description,
                        callRegReportGenerateVm.QueryType);
                }
                else if (callerTypeId == 13)
                {
                    datatable.Rows.Add(callRegReportGenerateVm.CallerName, callRegReportGenerateVm.MobileNumber,
                        callRegReportGenerateVm.Description
                        );
                }
                    
            }
            datatable.Rows.Add(data.Count);
            
            var grid = new GridView()
            {
                DataSource = datatable
            };

            grid.DataBind();

            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);
            if (grid.HeaderRow != null)
                grid.HeaderRow.BackColor = Color.White;

            grid.RenderControl(htw);

            //style to format numbers to string
            string style = @"<style> .textmode { } </style>";
            Response.Write(style);

            grid.HeaderStyle.BackColor = Color.White;
            Response.ClearContent();
            Response.Buffer = true;
            var fileName = "ExcelReport.xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "";
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return fileName;
       
        }

        public ActionResult SetOutboundCall()

        {
            var emplist = (from e in dbContex.EmpRegInfos
                join u in dbContex.UserProfiles on e.EmpId equals u.UserId
                where e.OfficeId == 41
                select new EmpListByCallRegistrationVM()
                {
                    Id = u.UserId,
                    Name = u.UserName
                }).ToList();
            ViewBag.empList = emplist;

            return View();
        }

        [HttpPost]
        public ActionResult SetOutboundCall( OutboundCallReg outboundCallReg )
        {
            
            outboundCallReg.OfficeId = 41;
            outboundCallReg.IsDone = false;
            dbContex.OutboundCallRegs.Add(outboundCallReg);
            dbContex.SaveChanges();

            return RedirectToAction("OfficeOwnerHome");
        }
     
    }
}
