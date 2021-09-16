using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PowerIBroker.Models;
using PowerIBrokerBusinessLayer;
using PowerIBrokerDataLayer;
using System.Data;
using System.IO;
using System.Text;
using PowerIBroker.Areas.Broker.Models;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.Configuration;
using System.Globalization;

namespace PowerIBroker.Controllers
{

    [BrokerSessionattribute]
    public class BrokerController : Controller
    {
        //
        // GET: /Broker/
        CompanyManagement objClientMngmt = new CompanyManagement();
        Administration objAdmin = new Administration();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BrokerLogout()
        {
            Session.Abandon();
            Session["CompanyID"] = null;
            Session["EmployeeID"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult BrokerDashboard()
        {
            if (Session["SubBrokerId"] == null)
                return RedirectToAction("Index", "Home");
            return View();
        }
        public ActionResult ManageCompany()
        {
            if (Session["SubBrokerId"] == null)
                return RedirectToAction("Index", "Home");
            return View();
        }
        public ActionResult _ManageCompany()
        {
            if (Session["SubBrokerId"] == null)
                return RedirectToAction("Index", "Home");
            CompanyMaster obj = new CompanyMaster();

            var BrokerId = Convert.ToInt64(Session["BrokerId"]);
            var SubBrokerId = Convert.ToInt64(Session["SubBrokerId"]);
            var Data = objAdmin.GetAllBrokerClients(BrokerId, SubBrokerId);
            ViewBag.CompanyData = Data;
            return PartialView();
        }

        public ActionResult ManageEmployee()
        {
            if (Session["SubBrokerId"] == null)
                return RedirectToAction("Index", "Home");

            var BrokerId = Convert.ToInt64(Session["BrokerId"]);
            var SubBrokerId = Convert.ToInt64(Session["SubBrokerId"]);
            var cdata = objAdmin.GetAllBrokerClients(BrokerId, SubBrokerId);
            SelectList data = new SelectList(cdata, "ID", "CCompanyName");
            ViewBag.Company = data;
            return View();
        }

        public ActionResult _ManageEmployee(long CompanyId = 0)
        {
            if (Session["SubBrokerId"] == null)
                return RedirectToAction("Index", "Home");
            var EmployeeData = objAdmin.GetEmployees(CompanyId);
            ViewBag.Employee = EmployeeData;
            return PartialView();
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

        public ActionResult SendMailonCompanyEmployees(string empID)
        {
            string result = string.Empty;
            var splitdata = empID.Split(',');
            if (splitdata.Length > 0)
            {
                PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
                for (int i = 0; i < splitdata.Length; i++)
                {

                    var objComEmpBasicInfo = objClientMngmt.GetEmployeesBasicDetails(Convert.ToInt64(splitdata[i]));
                    var companyDetails = objClientMngmt.GetCompanyName(objComEmpBasicInfo[0].CompanyId);
                    var employmentInfo = objComEmpMgnt.GetEmployementInfo(objComEmpBasicInfo[0].ID);
                    var employmentPersonalInfo = objComEmpMgnt.GetPersonalInfo(objComEmpBasicInfo[0].ID);
                    var data = new EMG_OpenEnrollment();
                    data = null;
                    if (employmentInfo != null)
                    {
                        data = objComEmpMgnt.GetOpenEmrollmentDate(Convert.ToInt64(employmentInfo.MonthlyBasisID), Convert.ToInt64(employmentInfo.DivisionID));
                    }

                    string baseurl = Helper.GetBaseUrl();
                    string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/RegistrationConfirmation_Admin.html"));  // email template
                    Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                    Mailbody = Mailbody.Replace("###LoginUserName###", objComEmpBasicInfo[0].Email);
                    Mailbody = Mailbody.Replace("###FirstName###", objComEmpBasicInfo[0].FirstName);
                    if (!string.IsNullOrEmpty(objComEmpBasicInfo[0].Password))
                    {
                        Mailbody = Mailbody.Replace("###Password###", Helper.Decrypt(objComEmpBasicInfo[0].Password));
                    }

                    Mailbody = Mailbody.Replace("###UserNAME###", Helper.Encrypt(objComEmpBasicInfo[0].Email));
                    Mailbody = Mailbody.Replace("###BrokerNAME###", companyDetails.tblBroker.Broker);
                    Mailbody = Mailbody.Replace("###BrokerEmail###", companyDetails.tblBroker.Email);
                    Mailbody = Mailbody.Replace("###CompanyNAME###", companyDetails.CCompanyName);
                    if (data != null)
                    {
                        Mailbody = Mailbody.Replace("###EndDate###", string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(data.EndDate)));
                    }

                    if (!string.IsNullOrEmpty(objComEmpBasicInfo[0].Email))
                    {
                        // Date21Sept as per client Discussion code comment 
                        result = Helper.SendEmail(objComEmpBasicInfo[0].Email, Mailbody, "Welcome to " + companyDetails.CCompanyName + " Benefits");
                        result = "Success";
                    }
                    // Email sending 




                    int statusPhone = 0;

                    if (!string.IsNullOrEmpty(employmentPersonalInfo.Phone))
                    {
                        string URLReset = baseurl + "/Home/PasswordUserReset?email=" + Helper.Encrypt(objComEmpBasicInfo[0].Email);
                        string SmsText = objComEmpBasicInfo[0].FirstName + ", Welcome to " + companyDetails.CCompanyName + " new benefits management system.  Click this link to set up a password and get started " + URLReset;
                        statusPhone = SendTwilloMessage.SendmessageTwillo(employmentPersonalInfo.Phone, SmsText);                       
                    }



                }

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult ChangeAdminPassword(string OldPassword, string NewPassword)
        {
            int result = 0;
            try
            {
                string OldPswd = Helper.Encrypt(OldPassword);
                string NewPswd = Helper.Encrypt(NewPassword);
                UserManagement um = new UserManagement();
                long   SubBrokerId = Convert.ToInt64(Session["SubBrokerId"]);

                Administration objadmin = new Administration();
                result = objadmin.ChangePasswordBroker(SubBrokerId, OldPswd, NewPswd);
              
            }
            catch (Exception ex)
            {
               
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
