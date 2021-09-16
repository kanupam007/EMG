using PowerIBroker.Areas.Broker.Models;
using PowerIBrokerBusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    //[sessionexpireattribute]
    public class TempAdminController : Controller
    {
        //
        // GET: /TempAdmin/
        CommonMasters cmm = new CommonMasters();
        public ActionResult SubscriptionPlan()
        {
            return View();
        }
        public ActionResult PurchaseSubscription()
        {
            return View();
        }
        public ActionResult AccessCompany(long AdminCompanyid, long EmpId)
        {
            Session["CompanyID"] = AdminCompanyid;
            Session["ComEmployeeId"] = EmpId;
            Session["EmgAdminID"] = 1;
            if (Session["CompanyID"] != null)
                return RedirectToAction("Index", "DashBoard", new { area = "Company" });

            return null;
        }
        public ActionResult PurchaseHistory()
        {
            return View();
        }
        
        public ActionResult Index()
        {
            if (Session["AdminEmail"] == null)
                return Redirect("/Admin");
            return View();
        }
        public ActionResult DemoRequest()
        {
            if (Session["AdminEmail"] == null)
                return Redirect("/Admin");
            return View();
        }
        public ActionResult CreateCompany(long ID = 0)
        {
            ViewBag.ID = ID;
            return View();
        }
        public ActionResult _BrokerList()
        {
            return PartialView("~/Views/TempAdmin/_BrokerList.cshtml");
        }
        public ActionResult ManageClient()
        {
            return View();
        }
        public ActionResult ManageCompanyEmployees()
        {
            return View();
        }
        public ActionResult ManageEmployees()
        {
            return View();
        }
        public ActionResult AddBroker()
        {
            return View();
        }
        public ActionResult ManageGeneralAgent(long ID = 0)
        {
            ViewBag.ID = ID;
            return View();
        }
        public ActionResult EditBroker()
        {
            return View();
        }
        public ActionResult ManageBroker(int ID = 0)
        {
            ViewBag.ID = ID;
            return View();
        }
    }
}
