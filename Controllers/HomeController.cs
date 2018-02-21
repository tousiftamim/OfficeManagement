using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Office_Management.Filters;
using Office_Management.Models;
using Office_Management.Models.ViewModel;
using Office_Management.Repository;
using Office_Management.Service;
using WebMatrix.WebData;

namespace Office_Management.Controllers
{
    [InitializeSimpleMembership]
    public class HomeController : Controller
    {
        DatabaseContex _databaseContex = new DatabaseContex();
        public OfficeService OfficeService = new OfficeService();
        public ActionResult Index()
        {
            if (Roles.IsUserInRole("OfficeOwner"))
            {
                var officeInfos = OfficeService.GetAllOfficeInfos(WebSecurity.CurrentUserId);
                return View("~/Views/Office/OfficeOwnerHome.cshtml", officeInfos);
            }
            if (Roles.IsUserInRole("Employee"))
            {
                var tasks = _databaseContex.TaskInfos.Where(_ => _.EmployeeId == WebSecurity.CurrentUserId);
                return View("~/Views/Employee/Index.cshtml", tasks);
            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CallHistory()
        {
            //List<CallHistoryViewModel> callRegistration = TempData["CallRegistration"] as List<CallHistoryViewModel>;
            int userId = WebSecurity.CurrentUserId;
            var callRegistration =new List<CallHistoryViewModel>();
            if (Roles.IsUserInRole("Employee"))
            {
                callRegistration = (from cr in _databaseContex.CallRegistrations
                                    where cr.UserId == userId
                                    join callType in _databaseContex.CallTypes on cr.CallTypeId equals callType.Id
                                    select new CallHistoryViewModel
                                    {
                                        Id = cr.Id,
                                        CallerName = cr.CallerName,
                                        MobileNumber = cr.MobileNumber,
                                        CallTypeId = cr.CallTypeId,
                                        CallTypeName = callType.CallTypeName,
                                        AdminComment = cr.AdminComment
                                    }).ToList();

            }
            else
            {
                callRegistration = (from cr in _databaseContex.CallRegistrations
                                       join callType in _databaseContex.CallTypes on cr.CallTypeId equals callType.Id
                                       join userProfile in _databaseContex.UserProfiles on cr.UserId equals userProfile.UserId
                                       select new CallHistoryViewModel
                                       {
                                           Id = cr.Id,
                                           UserName = userProfile.UserName,
                                           CallerName = cr.CallerName,
                                           MobileNumber = cr.MobileNumber,
                                           CallTypeName = callType.CallTypeName,
                                           AdminComment = cr.AdminComment,
                                           CallDuration = cr.CallDuration
                                       }).ToList();
            }

            ViewBag.callType = _databaseContex.CallTypes.ToList();
            return View(callRegistration);
        }
        public ActionResult SearchByCallType(int id)
        {
            int userId = WebSecurity.CurrentUserId;
            var callRegistration = new List<CallHistoryViewModel>();

            if (Roles.IsUserInRole("Employee"))
            {
                callRegistration = (from cr in _databaseContex.CallRegistrations
                                    where cr.UserId == userId && cr.CallTypeId == id
                                    join callType in _databaseContex.CallTypes on cr.CallTypeId equals callType.Id
                                    select new CallHistoryViewModel
                                    {
                                        Id = cr.Id,
                                        CallerName = cr.CallerName,
                                        MobileNumber = cr.MobileNumber,
                                        CallTypeId = cr.CallTypeId,
                                        CallTypeName = callType.CallTypeName,
                                        AdminComment = cr.AdminComment
                                    }).ToList();
            }
            else
            {
                callRegistration = (from cr in _databaseContex.CallRegistrations
                                    where cr.CallTypeId == id
                                    join userProfile in _databaseContex.UserProfiles on cr.UserId equals userProfile.UserId
                                    join callType in _databaseContex.CallTypes on cr.CallTypeId equals callType.Id
                                    select new CallHistoryViewModel
                                    {
                                        Id = cr.Id,
                                        UserName = userProfile.UserName,
                                        CallerName = cr.CallerName,
                                        MobileNumber = cr.MobileNumber,
                                        CallTypeId = cr.CallTypeId,
                                        CallTypeName = callType.CallTypeName,
                                        AdminComment = cr.AdminComment
                                    }).ToList();
            }
            
            return View("~/Views/Home/CallHistory.cshtml", callRegistration);
        }

        public ActionResult Details(int id)
        {
            var calReg = _databaseContex.CallRegistrations.Find(id);
            var callType = _databaseContex.CallTypes.FirstOrDefault(_ => _.Id == calReg.CallTypeId);
            var userProfile = _databaseContex.UserProfiles.FirstOrDefault(_ => _.UserId == calReg.UserId);

            var detail = new CallHistoryViewModel()
            {
                Id = calReg.Id,
                CallerName = calReg.CallerName,
                MobileNumber = calReg.MobileNumber,
                Address = calReg.Address,
                Time = calReg.Time,
                Description = calReg.Description,
                CallerEmail = calReg.CallerEmail,
                OrderCode = calReg.OrderCode,
                ProductCode = calReg.ProductCode,
                ComplainType = calReg.ComplainType,
                QueryType = calReg.QueryType,
                PaymentMethod = calReg.PaymentMethod,
                CallTypeId = calReg.CallTypeId,
                CallTypeName = callType.CallTypeName,
                UserId = calReg.UserId,
                UserName = userProfile.UserName,
                //                CallingStatusId = calReg.CallingStatusId,
                //                OfficeOwnerId = calReg.OfficeOwnerId,
                //                JobCategoryId = calReg.JobCategoryId,
                AdminComment = calReg.AdminComment,
                CallDuration = calReg.CallDuration
            };

            return View(detail);
        }
        public ActionResult UpdateCallHistory(CallHistoryViewModel callHistory)
        {

            return View(callHistory);
        }

        [HttpPost]
        public ActionResult ConfirmCallHistory(CallHistoryViewModel editedInfo)
        {
            var call = _databaseContex.CallRegistrations.Find(editedInfo.Id);
            if (call != null)
            {
                call.OrderCode = editedInfo.OrderCode;
                call.CallerName = editedInfo.CallerName;
                call.Address = editedInfo.Address;
                call.Description = editedInfo.Description;
                call.CallerEmail = editedInfo.CallerEmail;
                call.AdminComment = editedInfo.AdminComment;

                //                call.OrderCode = editedInfo.OrderCode;
                //                call.ProductCode = editedInfo.ProductCode;
                //                call.ComplainType = editedInfo.ComplainType;
                //                call.QueryType = editedInfo.QueryType;
                //                call.PaymentMethod = editedInfo.PaymentMethod;
                //                call.DeliveryDate = editedInfo.DeliveryDate;
                //                call.CallTypeId = editedInfo.CallTypeId;
                //                call.CallingStatusId = editedInfo.CallingStatusId;
                //                call.OfficeOwnerId = editedInfo.OfficeOwnerId;
                //                call.JobCategoryId = editedInfo.JobCategoryId;

                _databaseContex.CallRegistrations.Attach(call);
                _databaseContex.Entry(call).State = EntityState.Modified;

            }
            _databaseContex.SaveChanges();

            string role = WebSecurity.CurrentUserName;
            if (Roles.IsUserInRole("Employee"))
            {
                return RedirectToAction("MyCallHistory", "Employee");
            }
            else
            {
                return RedirectToAction("CallHistory", "Office");
            }
        }
    }
}
