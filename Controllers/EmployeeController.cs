﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Office_Management.Filters;
using Office_Management.Models;
using Office_Management.Models.ViewModel;
using Office_Management.Repository;
using WebMatrix.WebData;

namespace Office_Management.Controllers
{
    [InitializeSimpleMembership]
    public class EmployeeController : Controller
    {
        //
        // GET: /Employee/
        //        private string callingStartTime;
        //        private string callingEndTime;

        readonly DatabaseContex _databaseContex = new DatabaseContex();
        private OfficeRepository _officeRepository = new OfficeRepository();
        [Authorize(Roles = "Employee")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult EmployeeRegistration()
        {
            var userId = WebSecurity.CurrentUserId;
            var officelist = _officeRepository.GetAllByOfficeOwnerId(userId);
            return View(officelist);


        }
        [Authorize]
        [HttpPost]
        public ActionResult EmployeeRegistration(int officeId)
        {
            var showEmpList = new EmpRegInfo()
            {
                EmpId = WebSecurity.CurrentUserId,
                OfficeId = officeId
            };
            _databaseContex.EmpRegInfos.Add(showEmpList);
            _databaseContex.SaveChanges();

            //Roles.AddUserToRole(WebSecurity.CurrentUserName,"Employee");

            return RedirectToAction("Index", "Home");

        }
        public ActionResult DoneWork(int id)
        {
            var doneWorkEmp = _databaseContex.TaskInfos.Find(id);
            doneWorkEmp.TaskStatus = true;
            doneWorkEmp.DoneDateByEmp = DateTime.Now;
            _databaseContex.TaskInfos.AddOrUpdate(doneWorkEmp);
            _databaseContex.SaveChanges();

            return RedirectToAction("Index", "Home");

        }

        public ActionResult MyTask()
        {
            var data = (from t in _databaseContex.TaskInfos
                where t.EmployeeId == WebSecurity.CurrentUserId
                select new TaskInfoVM()
                {
                    Id = t.Id,
                    TaskName = t.TaskName,
                    TaskDescription = t.TaskDescription,
                    TaskDeadline = t.TaskDeadline,
                    IsAccept = t.IsAccept,
                    TaskStatus = t.TaskStatus
                    
                }).ToList();
            return View("~/Views/Home/EmployeeHome.cshtml", data);

        }

        public ActionResult NotDoneWork(int id)
        {
            var notDone = _databaseContex.TaskInfos.Find(id);
            notDone.TaskStatus = false;
            _databaseContex.TaskInfos.AddOrUpdate(notDone);
            _databaseContex.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult CallRegistration()
        {
            ViewBag.callingStartTime = DateTime.Now.ToString("h:mm:ss");

            DatabaseContex db = new DatabaseContex();
            ViewBag.callerTypes = db.CallTypes.ToList();
            ViewBag.callingStatuses = db.CallingStatuses.ToList();
            ViewBag.jobCategories = db.JobCategories.ToList();
            var employeeReginfo = _databaseContex.EmpRegInfos.FirstOrDefault(_ => _.EmpId == WebSecurity.CurrentUserId);
            if (employeeReginfo != null)
            {
                var officeId = employeeReginfo.OfficeId;
            }
            if (employeeReginfo != null) ViewBag.officeOwnerId = employeeReginfo.OfficeId;

            return View();
        }

        [HttpPost]
        public ActionResult CallRegistration(CallRegistration callRegistration)
        {
            
        
            if (Convert.ToDateTime(deliveryDate) > DateTime.Now)
            {
                var callRegistration = new CallRegistration()
        {
                    CallerName = callerName,
                    MobileNumber = mobileNumber,
                    Address = address,
                    Time = DateTime.Now,
                    Description = description,
                    OrderCode = orderCode,
                    PaymentMethod = paymentMethod,
                    DeliveryDate = Convert.ToDateTime(deliveryDate),
                    CallerTypeId = callerTypeId,
                    AssignerId = WebSecurity.CurrentUserId,
                    OfficeOwnerId = officeOwnerId

                };
            callRegistration.UserId = WebSecurity.CurrentUserId;
            callRegistration.CallDuration = duration.ToString();
            db.CallRegistrations.Add(callRegistration);
            db.SaveChanges();
            return RedirectToAction("Successfull", "Employee", callRegistration);
            }
            else return RedirectToAction("WrongDeliverInpute");
       
        }
        public ActionResult WrongDeliverInpute()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        //        public ActionResult EmployeeProfile(HttpPostedFileBase file, string address, string fullName, string email)
        //        {
        //            if (file != null)
        //            {
        //                string pic = System.IO.Path.GetFileName(file.FileName);
        //                string path = System.IO.Path.Combine(
        //                                       Server.MapPath("~/Uploads"), pic);
        //
        //                file.SaveAs(path);
        //
        //                var saveProfileInfo = new EmployeeProfile()
        //                {
        //                    ImagePath = file.FileName,
        //                    FullName = fullName,
        //                    Address = address,
        //                    Email = email,
        //                    EmployeeId = WebSecurity.CurrentUserId
        //
        //                };
        //                _databaseContex.EmployeeProfiles.Add(saveProfileInfo);
        //                _databaseContex.SaveChanges();
        //
        //            }
        //            // after successfully uploading redirect the user
        ////            return RedirectToAction("Index", "Home");
        //        }

        public ActionResult MyProfile()
        {
            var data = from e in _databaseContex.EmployeeProfiles
                       where e.EmployeeId == WebSecurity.CurrentUserId
                       select new MyProfileVM()
                       {
                           ImagePath = e.ImagePath,
                           FullName = e.FullName,
                           Address = e.Address,
                           Email = e.Email
                       };

            return View(data);
        }

        public ActionResult Inbound()
        {
            return View();
        }

        public ActionResult Successfull(CallRegistration cr)
        {
            return RedirectToAction("Details", "Home", new { id = cr.Id });
        }
        public ActionResult OutboundCallforEmp()
        {
            var data = from o in _databaseContex.OutboundCallRegs
                       where o.AssignerId == WebSecurity.CurrentUserId
                       select new OutboundCallforEmpVM()
                       {
                           Id = o.Id,
                           Name = o.Name,
                           PhoneNumber = o.PhoneNumber,
                           Reason = o.Reason
                       };
            return View(data);
        }
        [HttpGet]
        public ActionResult CommentforOutboundcall(int id)
        {
            //            ViewBag.rowId = id;
            ViewBag.outboundId = id;
            ViewBag.callingStatuses = _databaseContex.CallingStatuses.ToList();
            var outBound = _databaseContex.OutboundCallRegs.Find(id);

            return View(outBound);
        }

        [HttpPost]
        public ActionResult CommentForOutboundcall(OutboundCallReg outboundcallreg)
        {
            var elm = _databaseContex.OutboundCallRegs.FirstOrDefault(one => one.Id == outboundcallreg.Id);
            if (elm != null)
            {
                elm.AssignerComment = outboundcallreg.AssignerComment;
                elm.CallingStatusId = outboundcallreg.CallingStatusId;
                _databaseContex.OutboundCallRegs.Attach(elm);
                _databaseContex.Entry(elm).State = EntityState.Modified;

            }

            //            _databaseContex.OutboundCallRegs.AddOrUpdate(outboundcallreg);
            _databaseContex.SaveChanges();
            return RedirectToAction("OutboundCallforEmp");
        }
        
        public ActionResult CreateOutboundByOwn()
        {
            ViewBag.callingStatus = _databaseContex.CallingStatuses.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult CreateOutboundByOwn(OutboundCallReg callRegistration)
        {
            callRegistration.AssignerId = WebSecurity.CurrentUserId;
            callRegistration.OfficeId = 41;
            _databaseContex.OutboundCallRegs.AddOrUpdate(callRegistration);
            _databaseContex.SaveChanges();
            return RedirectToAction("Inbound");
        }


    }
