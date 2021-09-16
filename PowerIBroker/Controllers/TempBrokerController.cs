using PowerIBroker.Areas.Broker.Models;
using PowerIBroker.Areas.Employee.Models;
using PowerIBroker.Models;
using PowerIBrokerBusinessLayer;
using PowerIBrokerDataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    [BrokerSessionattribute]
    public class TempBrokerController : Controller
    {
        //
        // GET: /TempAdmin/
        PowerIBrokerEntities context = new PowerIBrokerEntities();
        CompanyManagement objClientMngmt = new CompanyManagement();
        CommonMasters cmm = new CommonMasters();
        public ActionResult Index()
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
        [HttpPost]
        public ActionResult AddCompanyNew(CompanyNew details)
        {
            long result = objClientMngmt.AddCompanyNew(details);
           
            if (result > 0)
            {
                var mulAdmin = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement().GetMutipleCompanyAdmin(details.ID);
                foreach (var item in mulAdmin)// in case of active mail will be send on multiple company admin.
                {
                    if (string.IsNullOrEmpty(item.Password))
                    {
                        try
                        {
                            string baseurl = Helper.GetBaseUrl();
                            string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/Add_New_Company.html"));  // email template
                            Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                            Mailbody = Mailbody.Replace("###LoginUserName###", item.Email);
                            Mailbody = Mailbody.Replace("###UserName###", item.FirstName);
                            Mailbody = Mailbody.Replace("###Password###", "");//Helper.Decrypt(item.Password)
                            Mailbody = Mailbody.Replace("###UserNAME###", Helper.Encrypt(item.Email));
                            Mailbody = Mailbody.Replace("###CompanyName###", details.CCompanyName);
                            if (!string.IsNullOrEmpty(item.Email))
                            {
                                // Date21Sept as per client Discussion code comment 
                                Helper.SendEmail(item.Email, Mailbody, "Welcome to EnrollMyGroup");
                            }
                        }
                        catch (Exception)
                        {

                            
                        }
                        

                    }
                }
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult UpdateClient(CompanyClientNew details)
        {
            return Json(objClientMngmt.UpdateClient(details));
        }
        public ActionResult SubscriptionPlan()
        {
            return View();
        }
        public ActionResult PurchaseSubscription()
        {
            return View();
        }
        public ActionResult PurchaseHistory()
        {
            return View();
        }
        public ActionResult CreateCompany(long ID=0)
        {
            ViewBag.ID = ID;
            return View();
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
        public ActionResult EmployeeRedirect(string CompanyId,string URL)
        {
            Session["CompanyID"] = CompanyId;
            return Redirect(URL);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session["CompanyID"] = null;
            Session["EmployeeID"] = null;
            return RedirectToAction("Login", "Home");
        }
        public ActionResult AddBroker(long ID = 0)
        {
            CompanyBroker obj = new CompanyBroker();
            List<Broker> listBroker = new List<Broker>();
            var BrokerMasterData = cmm.GetBrokerMaster(0);
            ViewBag.BrokerMaster = BrokerMasterData;
            var BrokerUserData = cmm.GetBrokerUserMaster(0, Convert.ToInt32(Session["BrokerId"]));
            ViewBag.Clients = BrokerUserData;
            return View();
        }
        
        public ActionResult ManageBroker()
        {
            CompanyBroker obj = new CompanyBroker();
            var BrokerUserData = cmm.GetBrokerUserMaster(0, Convert.ToInt32(Session["BrokerId"]));
            ViewBag.Clients = BrokerUserData;
            return View();
        }
        public ActionResult AddBroker2()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadPlanValidate(long CompanyId)
        {
            Comman objComman = new Comman();
            CompanyManagement objClientMngmt = new CompanyManagement();
            string finalflag = string.Empty;
            string strCity = string.Empty;
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            StringBuilder msgError = new StringBuilder();
            long CompanyID = CompanyId;
            StringBuilder msgSuccess = new StringBuilder();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data1 = new SelectList(cdata, "ID", "CCompanyName");
            ViewBag.Company = data1;

            bool flag = true;
            var EMedical = "";
            var EDental = "";
            var EVision = "";
            string EmpSSN = string.Empty;
            string DependentSSN = string.Empty;
            try
            {
                if (Request.Files.Count > 0)
                {

                    HttpPostedFileBase file = Request.Files[0];
                    if (file != null)
                    {
                        string fileName = string.Empty;
                        fileName = DateTime.UtcNow.Ticks + "-" + file.FileName;
                        var path = Server.MapPath("~/Uploads/Admin/EmpolyeeDetailsDocs/");

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);

                        OleDbConnection oledbConn = new OleDbConnection();
                        if (Path.GetExtension(file.FileName) == ".xls")
                        {
                            oledbConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";

                            oledbConn.Open();

                        }
                        else if (Path.GetExtension(file.FileName) == ".xlsx")
                        {
                            oledbConn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0 Xml; HDR = YES\"";
                            oledbConn.Open();
                        }

                        if (oledbConn.State == ConnectionState.Closed)
                            oledbConn.Open();

                        OleDbCommand cmd = new OleDbCommand();
                        OleDbDataAdapter oleda = new OleDbDataAdapter();
                        DataSet ds = new DataSet();
                        System.Data.DataTable dt = new System.Data.DataTable();
                        cmd.Connection = oledbConn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM [Sheet1$]";
                        oleda = new OleDbDataAdapter(cmd);
                        oleda.Fill(ds, "Sheet1");
                        dt = ds.Tables["Sheet1"];
                        int counterrow = 1;
                        EMG_OpenEnrollment infoCheckOEExists = new EMG_OpenEnrollment();

                        List<tblEmployee_Benefits> mainList = new List<tblEmployee_Benefits>();

                        foreach (DataRow dr in dt.Rows)
                        {
                            tblEmployee_Benefits list = new tblEmployee_Benefits();
                            long? EmpId = 0;
                            long DepdentId = 0;
                            long PlanId = 0;
                            EmpSSN = objComman.RemoveMasking(Convert.ToString(dr["Employee SSN"]).Trim());
                            DependentSSN = objComman.RemoveMasking(Convert.ToString(dr["Dependent SSN"]).Trim());
                            if (!string.IsNullOrEmpty(EmpSSN))
                            {
                                int ValEmp = Convert.ToInt32(EmpSSN);
                                EmpSSN = string.Format(String.Format("{0:000000000}", ValEmp));
                            }
                            if (!string.IsNullOrEmpty(DependentSSN))
                            {
                                int ValDep = Convert.ToInt32(DependentSSN);
                                DependentSSN = string.Format(String.Format("{0:000000000}", ValDep));
                            }

                            if (string.IsNullOrEmpty(EmpSSN))
                            {
                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> value is null at Row No. - " + counterrow + "</li>");
                                flag = false;
                            }

                            if (!string.IsNullOrEmpty(EmpSSN))
                            {
                                if (EmpSSN.Length != 9)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> length should be equal to 9 at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                            }
                            if (!string.IsNullOrEmpty(EmpSSN))
                            {
                                long a = 0;
                                // long.TryParse(Convert.ToString(dr["Employee SSN"]).Trim(), out a);
                                if (long.TryParse(EmpSSN, out a) == false)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> should be numeric at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(EmpSSN))
                            {
                                var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone("", "", 0, 2, EmpSSN);

                                if (duplicateComRecord.Count == 0)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> does not exists at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                                else
                                {
                                    // vk
                                    var info = new CompanyManagement().GetEmployementInfobySSN(EmpSSN);
                                    if (info != null)
                                    {

                                        if (string.IsNullOrEmpty(Convert.ToString(info.Salary)))
                                        {
                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Salary</b> is null or empty at Row No. - " + counterrow + "</li>");
                                            flag = false;
                                        }
                                        if (string.IsNullOrEmpty(Convert.ToString(info.DivisionID)))
                                        {
                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Division</b> is null or empty at Row No. - " + counterrow + "</li>");
                                            flag = false;
                                        }
                                        if (string.IsNullOrEmpty(Convert.ToString(info.MonthlyBasisID)))
                                        {
                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Monthly</b> is null or empty at Row No. - " + counterrow + "</li>");
                                            flag = false;
                                        }

                                        if (info.EmployeeID != 0)
                                        {
                                            EmpId = info.EmployeeID;
                                            var personalinfo = new CompanyManagement().GetEmploymePersonalInfo(Convert.ToInt64(info.EmployeeID));
                                            if (personalinfo != null)
                                            {
                                                if (string.IsNullOrEmpty(personalinfo.Gender))
                                                {
                                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Gender</b> is null or empty at Row No. - " + counterrow + "</li>");
                                                    flag = false;
                                                }
                                                if (string.IsNullOrEmpty(Convert.ToString(personalinfo.DOB)))
                                                {
                                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>DOB</b> is null or empty at Row No. - " + counterrow + "</li>");
                                                    flag = false;

                                                }
                                            }

                                        }


                                    }

                                }
                            }

                            if (!string.IsNullOrEmpty(DependentSSN))
                            {
                                if (DependentSSN.Length != 9)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> length should be equal to 9 at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                            }
                            if (!string.IsNullOrEmpty(DependentSSN))
                            {
                                long a = 0;
                                if (long.TryParse(DependentSSN, out a) == false)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> should be numeric at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                            }

                            if (!string.IsNullOrEmpty(DependentSSN))
                            {
                                var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone("", "", 0, 4, DependentSSN);

                                if (duplicateComRecord.Count == 0)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> does not exists at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                                else
                                {
                                    var info = new CompanyManagement().GetEmployementInfobySSN(EmpSSN);
                                    var dinfo = new CompanyManagement().GetDependentInfobySSN(DependentSSN);
                                    if (info != null)
                                    {
                                        if (dinfo != null)
                                        {
                                            if (info.EmployeeID != dinfo.EmployeeId)
                                            {
                                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent </b> not exists  with associate emp .at Row No. - " + counterrow + "</li>");
                                                flag = false;
                                            }
                                            else
                                            {
                                                DepdentId = dinfo.ID;
                                                var catDependent = new CommonMasters().GetPlanCategory(0);
                                                for (int i = 0; i < catDependent.Count; i++)
                                                {
                                                    if (Convert.ToInt64(catDependent[i].ID) == (int)CategoryEnum.PlanCategory.Medical || Convert.ToInt64(catDependent[i].ID) == (int)CategoryEnum.PlanCategory.Dental || Convert.ToInt64(catDependent[i].ID) == (int)CategoryEnum.PlanCategory.Vision)
                                                    {
                                                        if (!string.IsNullOrEmpty(Convert.ToString(dr[catDependent[i].PlanCategory]).Trim()))
                                                        {
                                                            var plancheckDep = new CompanyManagement().CheckPlanExists(Convert.ToString(dr[catDependent[i].PlanCategory]).Trim(), CompanyID, Convert.ToInt64(catDependent[i].ID));
                                                            if (plancheckDep == null)
                                                            {
                                                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>" + Convert.ToString(dr[catDependent[i].PlanCategory]).Trim() + "</b> Plan does not exists at Row No. - " + counterrow + "</li>");
                                                                flag = false;
                                                            }
                                                            else
                                                            {
                                                                var dateAndTime = DateTime.UtcNow;
                                                                var datePart = dateAndTime.Date;

                                                                if (plancheckDep.StartDate < datePart)
                                                                {
                                                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>" + Convert.ToString(dr[catDependent[i].PlanCategory]).Trim() + "</b> Plan Start Date less than from Current Date  at Row No. - " + counterrow + "</li>");
                                                                    flag = false;
                                                                }
                                                                //cfff


                                                                if (Convert.ToInt64(catDependent[i].ID) == (int)CategoryEnum.PlanCategory.Medical)
                                                                {
                                                                    if (Convert.ToString(EMedical) != Convert.ToString(plancheckDep.PlanName))
                                                                    {
                                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>" + Convert.ToString(dr[catDependent[i].PlanCategory]).Trim() + "</b> Dependent plan name does not match with employee at Row No. - " + counterrow + "</li>");
                                                                        flag = false;
                                                                    }
                                                                }
                                                                if (Convert.ToInt64(catDependent[i].ID) == (int)CategoryEnum.PlanCategory.Dental)
                                                                {
                                                                    if (Convert.ToString(EDental) != Convert.ToString(plancheckDep.PlanName))
                                                                    {
                                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>" + Convert.ToString(dr[catDependent[i].PlanCategory]).Trim() + "</b> Dependent plan name does not match with employee at Row No. - " + counterrow + "</li>");
                                                                        flag = false;
                                                                    }
                                                                }
                                                                if (Convert.ToInt64(catDependent[i].ID) == (int)CategoryEnum.PlanCategory.Vision)
                                                                {
                                                                    if (Convert.ToString(EVision) != Convert.ToString(plancheckDep.PlanName))
                                                                    {
                                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>" + Convert.ToString(dr[catDependent[i].PlanCategory]).Trim() + "</b> Dependent plan name does not match with employee at Row No. - " + counterrow + "</li>");
                                                                        flag = false;
                                                                    }
                                                                }


                                                            }
                                                        }
                                                    }


                                                }


                                            }
                                        }

                                    }

                                }

                            }

                            var cat = new CommonMasters().GetPlanCategory(0);
                            if ((!string.IsNullOrEmpty(EmpSSN)) && (string.IsNullOrEmpty(DependentSSN)))
                            {
                                var onePlanReq = false;

                                EMedical = "";
                                EDental = "";
                                EVision = "";

                                for (int i = 0; i < cat.Count; i++)
                                {

                                    if (!string.IsNullOrEmpty(Convert.ToString(dr[cat[i].PlanCategory]).Trim()))
                                    {

                                        var plancheck = new CompanyManagement().CheckPlanExists(Convert.ToString(dr[cat[i].PlanCategory]).Trim(), CompanyID, Convert.ToInt64(cat[i].ID));
                                        if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.Medical || Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.Dental || Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.Vision)
                                        {
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.Medical)
                                            {
                                                if (plancheck != null)
                                                {
                                                    EMedical = plancheck.PlanName;
                                                }

                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.Dental)
                                            {
                                                if (plancheck != null)
                                                {
                                                    EDental = plancheck.PlanName;
                                                }
                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.Vision)
                                            {
                                                if (plancheck != null)
                                                {
                                                    EVision = plancheck.PlanName;
                                                }
                                            }

                                        }
                                        if (plancheck == null)
                                        {
                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>" + Convert.ToString(dr[cat[i].PlanCategory]).Trim() + "</b> Plan does not exists for " + Convert.ToString(cat[i].PlanCategory).Trim() + " category at Row No. - " + counterrow + "</li>");
                                            flag = false;
                                        }
                                        else
                                        {
                                            var dateAndTime = DateTime.UtcNow;
                                            var datePart = dateAndTime.Date;

                                            if (plancheck.StartDate < datePart)
                                            {
                                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>" + Convert.ToString(dr[cat[i].PlanCategory]).Trim() + "</b> Plan Start Date less than from Current Date  at Row No. - " + counterrow + "</li>");
                                                flag = false;
                                            }


                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                                            {

                                                if (string.IsNullOrEmpty(Convert.ToString(dr["Voluntary Life Value"] != DBNull.Value ? dr["Voluntary Life Value"] : "")))
                                                {
                                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Value</b> value is null at Row No. - " + counterrow + "</li>");
                                                    flag = false;
                                                }

                                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Voluntary Life Value"] != DBNull.Value ? dr["Voluntary Life Value"] : "")))
                                                {
                                                    long a = 0;
                                                    if (long.TryParse(Convert.ToString(dr["Voluntary Life Value"]).Trim(), out a) == false)
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Value</b> should be numeric at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }
                                                }


                                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Voluntary Life Value"] != DBNull.Value ? dr["Voluntary Life Value"] : "")))
                                                {
                                                    if (Convert.ToString(dr["Voluntary Life Value"]).Trim().Length > 14)
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Value</b> length can not be exceed to 14 at Row No. - " + counterrow + " </li>");
                                                        flag = false;
                                                    }
                                                }
                                                var detailsSpouseCheck = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetMinMaxByInsurancePlanID(plancheck.ID);
                                                if (detailsSpouseCheck.IsSpouse == true)
                                                {
                                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Voluntary Life Spouse Value"] != DBNull.Value ? dr["Voluntary Life Spouse Value"] : "")))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Spouse Value</b> value is null at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }

                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["Voluntary Life Spouse Value"] != DBNull.Value ? dr["Voluntary Life Spouse Value"] : "")))
                                                    {
                                                        long a = 0;
                                                        if (long.TryParse(Convert.ToString(dr["Voluntary Life Spouse Value"]).Trim(), out a) == false)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Spouse Value</b> should be numeric at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }
                                                    }

                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["Voluntary Life Spouse Value"] != DBNull.Value ? dr["Voluntary Life Spouse Value"] : "")))
                                                    {
                                                        if (Convert.ToString(dr["Voluntary Life Spouse Value"]).Trim().Length > 14)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Spouse Value</b> length can not be exceed to 14 at Row No. - " + counterrow + " </li>");
                                                            flag = false;
                                                        }
                                                    }
                                                }

                                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Is Child"] != DBNull.Value ? dr["Is Child"] : "")))
                                                {
                                                    if (detailsSpouseCheck.IsChild == true)
                                                    {
                                                        var isChildValue = Convert.ToString(dr["Is Child"]).ToLower().Trim();
                                                        if (isChildValue != "yes" && isChildValue != "no")
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Is Child</b> value should be Yes or No at Row No. - " + counterrow + " </li>");
                                                            flag = false;
                                                        }
                                                    }
                                                }

                                            }

                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                                            {
                                                var details = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetMinMaxByInsurancePlanID(plancheck.ID);
                                                var employmentDetails = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetEmployeeSalaryByEmpID((long)EmpId);
                                                long bonus = (plancheck.IsBonus == true ? employmentDetails.Bonus : 0);
                                                long commision = (plancheck.IsCommission == true ? employmentDetails.Commisiion : 0);
                                                long empsalary = Convert.ToInt64(employmentDetails.Salary) + bonus + commision;
                                                var maxValue = details.Maxvalue;
                                                var minValue = details.MinValue;
                                                var monthlyEEValue = details.MonthlyEEValue;
                                                /////////////////////////////////////////Employee///////////////////////////////////////
                                                if (plancheck.IsFlat == 6)
                                                {

                                                    if (details.ApprovedCost != "6")
                                                    {
                                                        maxValue = (maxValue > (Convert.ToInt32(details.ApprovedCost) * empsalary) ? (Convert.ToInt32(details.ApprovedCost) * empsalary) : maxValue);

                                                    }
                                                    if (flag == true)
                                                    {

                                                        if (minValue > Convert.ToInt64(dr["Voluntary Life Value"]) || maxValue < Convert.ToInt64(dr["Voluntary Life Value"]))
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Value</b> does not exists between the min and max selected range at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }


                                                        long EMPPurchaseIncrement = 0;
                                                        EMPPurchaseIncrement = (Convert.ToInt64(dr["Voluntary Life Value"])) % Convert.ToInt64(monthlyEEValue);
                                                        if (EMPPurchaseIncrement != 0)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Value</b> should be in multiple of " + monthlyEEValue + " at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }


                                                        long val = (Convert.ToInt64(dr["Voluntary Life Value"]));
                                                        var detailsSpouseCheck = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetMinMaxByInsurancePlanID(plancheck.ID);
                                                        if (detailsSpouseCheck.IsSpouse == true)
                                                        {

                                                            long SpValue = (Convert.ToInt64(dr["Voluntary Life Spouse Value"]));
                                                            long Spmaxval = (val * Convert.ToInt32(details.ApprovalBenefit)) / 100;

                                                            //   Spmaxval = (Spmaxval > Convert.ToInt32(details.SMaxCapAmount) ? Convert.ToInt32(details.SMaxCapAmount) : Spmaxval);
                                                            //if (details.SMaxCapAmount != 0 && details.SMaxCapAmount > Spmaxval)
                                                            if (details.SMaxCapAmount != 0)
                                                            {
                                                                Spmaxval = (Spmaxval > Convert.ToInt32(details.SMaxCapAmount) ? Convert.ToInt32(details.SMaxCapAmount) : Spmaxval);
                                                            }
                                                            if (SpValue > Spmaxval)
                                                            {
                                                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Spouse Value</b> cannot be greater than " + Spmaxval + " at Row No. - " + counterrow + "</li>");
                                                                flag = false;
                                                            }


                                                        }

                                                    }
                                                }
                                                else
                                                {


                                                    if (Convert.ToInt64(dr["Voluntary Life Value"]) > Convert.ToInt64(plancheck.IsFlat))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Value</b> cannot be greater than " + plancheck.IsFlat + " at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }


                                                    if (flag == true)
                                                    {
                                                        long val = (Convert.ToInt64(dr["Voluntary Life Value"]));
                                                        var detailsSpouseCheck = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetMinMaxByInsurancePlanID(plancheck.ID);
                                                        if (detailsSpouseCheck.IsSpouse == true)
                                                        {
                                                            long SpValue = (Convert.ToInt64(dr["Voluntary Life Spouse Value"]));
                                                            long Spmaxval = (Convert.ToInt32(new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().ConvertNumbertoWords(val * empsalary)) * Convert.ToInt32(details.ApprovalBenefit)) / 100;


                                                            if (details.SMaxCapAmount != 0)
                                                            {
                                                                Spmaxval = (Spmaxval > Convert.ToInt32(details.SMaxCapAmount) ? Convert.ToInt32(details.SMaxCapAmount) : Spmaxval);
                                                                //Spmaxval = (long)details.SMaxCapAmount;
                                                            }

                                                            if (SpValue > Spmaxval)
                                                            {
                                                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Life Spouse Value</b> cannot be greater than " + Spmaxval + " at Row No. - " + counterrow + "</li>");
                                                                flag = false;
                                                            }


                                                        }


                                                    }
                                                }
                                                ////////////////////////////////////Spouse////////////////////////////////////

                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.OtherVoluntary)
                                            {
                                                if (flag == true)
                                                {

                                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Other Voluntary Value"] != DBNull.Value ? dr["Other Voluntary Value"] : "")))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Other Voluntary Value</b> value is null at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }
                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["Other Voluntary Value"] != DBNull.Value ? dr["Other Voluntary Value"] : "")))
                                                    {
                                                        long a = 0;
                                                        if (long.TryParse(Convert.ToString(dr["Other Voluntary Value"]).Trim(), out a) == false)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Other Voluntary Value</b> should be numeric at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["Other Voluntary Value"] != DBNull.Value ? dr["Other Voluntary Value"] : "")))
                                                    {
                                                        if (Convert.ToString(dr["Other Voluntary Value"]).Trim().Length > 14)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Other Voluntary Value</b> length can not be exceed to 14 at Row No. - " + counterrow + " </li>");
                                                            flag = false;
                                                        }
                                                    }

                                                    if (flag == true)
                                                    {
                                                        var SelectedRangeValue = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetMinMaxByInsurancePlanID(plancheck.ID);
                                                        if (SelectedRangeValue.MinValue > Convert.ToInt64(dr["Other Voluntary Value"]) || SelectedRangeValue.Maxvalue < Convert.ToInt64(dr["Other Voluntary Value"]))
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Other Voluntary Value</b> does not exists between the min and max selected range at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }
                                                    }

                                                }
                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.VoluntaryLongTermDisability)
                                            {
                                                if (flag == true)
                                                {

                                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Voluntary Long Term Disability Value"] != DBNull.Value ? dr["Voluntary Long Term Disability Value"] : "")))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Long Term Disability Value</b> value is null at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }

                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["Voluntary Long Term Disability Value"] != DBNull.Value ? dr["Voluntary Long Term Disability Value"] : "")))
                                                    {
                                                        long a = 0;
                                                        if (long.TryParse(Convert.ToString(dr["Voluntary Long Term Disability Value"]).Trim(), out a) == false)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Long Term Disability Value</b> should be numeric at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }
                                                    }

                                                    if (flag == true)
                                                    {

                                                        var SelectedRangeValue = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetMinMaxByInsurancePlanID(plancheck.ID);
                                                        if (SelectedRangeValue.MinValue > Convert.ToInt64(dr["Voluntary Long Term Disability Value"]) || SelectedRangeValue.Maxvalue < Convert.ToInt64(dr["Voluntary Long Term Disability Value"]))
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Long Term Disability Value</b> does not exists between the min and max selected range at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }


                                                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Voluntary Long Term Disability Value"] != DBNull.Value ? dr["Voluntary Long Term Disability Value"] : "")))
                                                        {
                                                            if (Convert.ToString(dr["Voluntary Long Term Disability Value"]).Trim().Length > 14)
                                                            {
                                                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Voluntary Long Term Disability Value</b> length can not be exceed to 14 at Row No. - " + counterrow + " </li>");
                                                                flag = false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.FSA)
                                            {
                                                if (flag == true)
                                                {

                                                    if (string.IsNullOrEmpty(Convert.ToString(dr["FSA Value"] != DBNull.Value ? dr["FSA Value"] : "")))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>FSA Value </b> value is null at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }

                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["FSA Value"] != DBNull.Value ? dr["FSA Value"] : "")))
                                                    {
                                                        long a = 0;
                                                        if (long.TryParse(Convert.ToString(dr["FSA Value"]).Trim(), out a) == false)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>FSA Value</b> should be numeric at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }
                                                    }


                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["FSA Value"] != DBNull.Value ? dr["FSA Value"] : "")))
                                                    {
                                                        if (Convert.ToString(dr["FSA Value"]).Trim().Length > 14)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>FSA Value</b> length can not be exceed to 14 at Row No. - " + counterrow + " </li>");
                                                            flag = false;
                                                        }
                                                    }

                                                    var detailatsa = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetFSAMinMaxByInsurancePlanID(plancheck.ID);
                                                    if (detailatsa.SponsoredVal == true)
                                                    {

                                                        if (string.IsNullOrEmpty(Convert.ToString(dr["FSA Child Value"] != DBNull.Value ? dr["FSA Child Value"] : "")))
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>FSA Child Value </b> value is null at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }

                                                        if (!string.IsNullOrEmpty(Convert.ToString(dr["FSA Child Value"] != DBNull.Value ? dr["FSA Child Value"] : "")))
                                                        {
                                                            long a = 0;

                                                            if (long.TryParse(Convert.ToString(dr["FSA Child Value"]).Trim(), out a) == false)
                                                            {
                                                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>FSA Child Value</b> should be numeric at Row No. - " + counterrow + "</li>");
                                                                flag = false;
                                                            }
                                                        }


                                                        if (!string.IsNullOrEmpty(Convert.ToString(dr["FSA Child Value"] != DBNull.Value ? dr["FSA Child Value"] : "")))
                                                        {
                                                            if (Convert.ToString(dr["FSA Child Value"]).Trim().Length > 14)
                                                            {
                                                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>FSA Child Value</b> length can not be exceed to 14 at Row No. - " + counterrow + " </li>");
                                                                flag = false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.HSA)
                                            {
                                                if (flag == true)
                                                {

                                                    if (string.IsNullOrEmpty(Convert.ToString(dr["HSA Value"] != DBNull.Value ? dr["HSA Value"] : "")))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>HSA Value </b> value is null at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }

                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["HSA Value"] != DBNull.Value ? dr["HSA Value"] : "")))
                                                    {
                                                        long a = 0;
                                                        if (long.TryParse(Convert.ToString(dr["HSA Value"]).Trim(), out a) == false)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>HSA Value</b> should be numeric at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }
                                                    }

                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["HSA Value"] != DBNull.Value ? dr["HSA Value"] : "")))
                                                    {
                                                        if (Convert.ToString(dr["HSA Value"]).Trim().Length > 14)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>HSA Value</b> length can not be exceed to 14 at Row No. - " + counterrow + " </li>");
                                                            flag = false;
                                                        }
                                                    }
                                                }
                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.TSA)
                                            {
                                                if (flag == true)
                                                {

                                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Transport Value"] != DBNull.Value ? dr["Transport Value"] : "")))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Transport Value </b> value is null at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }

                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["Transport Value"] != DBNull.Value ? dr["Transport Value"] : "")))
                                                    {
                                                        long a = 0;
                                                        if (long.TryParse(Convert.ToString(dr["Transport Value"]).Trim(), out a) == false)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Transport Value</b> should be numeric at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }
                                                    }


                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["Transport Value"] != DBNull.Value ? dr["Transport Value"] : "")))
                                                    {
                                                        if (Convert.ToString(dr["Transport Value"]).Trim().Length > 14)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Transport Value</b> length can not be exceed to 14 at Row No. - " + counterrow + " </li>");
                                                            flag = false;
                                                        }
                                                    }


                                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Parking Value"] != DBNull.Value ? dr["Parking Value"] : "")))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Parking Value </b> value is null at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }
                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["Parking Value"] != DBNull.Value ? dr["Parking Value"] : "")))
                                                    {
                                                        long a = 0;

                                                        if (long.TryParse(Convert.ToString(dr["Parking Value"]).Trim(), out a) == false)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Parking Value</b> should be numeric at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }
                                                    }

                                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["Parking Value"] != DBNull.Value ? dr["Parking Value"] : "")))
                                                    {
                                                        if (Convert.ToString(dr["Parking Value"]).Trim().Length > 14)
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Parking Value</b> length can not be exceed to 14 at Row No. - " + counterrow + " </li>");
                                                            flag = false;
                                                        }
                                                    }
                                                }
                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.FSA)
                                            {
                                                if (flag == true)
                                                {
                                                    var detailatsa = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetFSAMinMaxByInsurancePlanID(plancheck.ID);
                                                    if (detailatsa.MinRange > Convert.ToInt64(dr["FSA Value"]) || detailatsa.MaxRange < Convert.ToInt64(dr["FSA Value"]))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>FSA Value</b> does not exists between the min and max selected range at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }
                                                    if (detailatsa.SponsoredVal == true)
                                                    {
                                                        if (detailatsa.AmountStart > Convert.ToInt64(dr["FSA Child Value"]) || detailatsa.AmountEnd < Convert.ToInt64(dr["FSA Child Value"]))
                                                        {
                                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>FSA Child Value</b> does not exists between the min and max selected range at Row No. - " + counterrow + "</li>");
                                                            flag = false;
                                                        }
                                                    }
                                                }
                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.HSA)
                                            {
                                                if (flag == true)
                                                {
                                                    var detailatsa = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetFSAMinMaxByInsurancePlanID(plancheck.ID);
                                                    if (detailatsa.MinRange > Convert.ToInt64(dr["HSA Value"]))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>HSA Value</b> does not exists between the min and max selected range at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }
                                                }
                                            }
                                            if (Convert.ToInt64(cat[i].ID) == (int)CategoryEnum.PlanCategory.TSA)
                                            {
                                                if (flag == true)
                                                {
                                                    var detailatsa = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment().GetFSAMinMaxByInsurancePlanID(plancheck.ID);
                                                    if (detailatsa.MinRange > Convert.ToInt64(dr["Transport Value"]) || (detailatsa.MaxRange - Convert.ToInt64(detailatsa.TransitContribution)) < Convert.ToInt64(dr["Transport Value"]))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Transport Value</b> does not exists between the min and max selected range at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }

                                                    if (detailatsa.AmountStart > Convert.ToInt64(dr["Parking Value"]) || (detailatsa.AmountEnd - Convert.ToInt64(detailatsa.ParkingContribution)) < Convert.ToInt64(dr["Parking Value"]))
                                                    {
                                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Parking Value</b> does not exists between the min and max selected range at Row No. - " + counterrow + "</li>");
                                                        flag = false;
                                                    }
                                                }
                                            }

                                        }
                                        onePlanReq = true;
                                    }

                                }
                                if (onePlanReq == false)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>One </b> plan is mandatory for employee at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                            }


                            counterrow++;
                        }

                        if (dt.Rows.Count == 0)
                        {
                            flag = false;
                            string msgdisplay = "There is no data in excel sheet";
                            return Json(new { flag, Message = msgdisplay, JsonRequestBehavior.AllowGet });
                        }


                        if (flag)
                        {
                            var rowupdated = counterrow - 1;
                            string msgdisplay = "Total " + rowupdated + " rows validated successfully.";
                            ///////////////////////////////////////////////////////////////////


                            ///////////////////////////////////////////////////////////////////
                            return Json(new { flag, Message = msgdisplay, JsonRequestBehavior.AllowGet });

                        }
                        if (!flag)
                        {
                            string msgdisplay = msgError.ToString();
                            return Json(new { flag, Message = msgdisplay, JsonRequestBehavior.AllowGet });

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                return Json(new { flag, Message = ex.Message, JsonRequestBehavior.AllowGet });
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public static bool isValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
        [HttpPost]
        public ActionResult UploadTerminateValidate(long CompanyId)
        {
            Comman objComman = new Comman();
            CompanyManagement objClientMngmt = new CompanyManagement();
            string finalflag = string.Empty;
            string strCity = string.Empty;
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            StringBuilder msgError = new StringBuilder();
            long CompanyID = CompanyId;
            StringBuilder msgSuccess = new StringBuilder();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data1 = new SelectList(cdata, "ID", "CCompanyName");
            ViewBag.Company = data1;
            bool flag = true;
            string EmpSSN = string.Empty;
            string PhoneHome = string.Empty;
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (file != null)
                    {
                        string fileName = string.Empty;
                        fileName = DateTime.UtcNow.Ticks + "-" + file.FileName;
                        var path = Server.MapPath("~/Uploads/Admin/EmpolyeeDetailsDocs/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                        OleDbConnection oledbConn = new OleDbConnection();
                        if (Path.GetExtension(file.FileName) == ".xls")
                        {
                            oledbConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            oledbConn.Open();

                        }
                        else if (Path.GetExtension(file.FileName) == ".xlsx")
                        {
                            oledbConn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';";

                        }

                        if (oledbConn.State == ConnectionState.Closed)
                            oledbConn.Open();
                        OleDbCommand cmd = new OleDbCommand();
                        OleDbDataAdapter oleda = new OleDbDataAdapter();
                        DataSet ds = new DataSet();
                        System.Data.DataTable dt = new System.Data.DataTable();
                        cmd.Connection = oledbConn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM [Sheet1$]";
                        oleda = new OleDbDataAdapter(cmd);
                        oleda.Fill(ds, "Sheet1");
                        dt = ds.Tables["Sheet1"];
                        tblCompany_Employee_BasicInfo objComEmpBasicInfo = new tblCompany_Employee_BasicInfo();
                        int counterrow = 1;
                        foreach (DataRow dr in dt.Rows)
                        {
                            objComEmpBasicInfo.CompanyId = CompanyID;
                            EmpSSN = objComman.RemoveMasking(Convert.ToString(dr["SSN"]).Trim());

                            if (!string.IsNullOrEmpty(EmpSSN))
                            {
                                int ValEmp = Convert.ToInt32(EmpSSN);
                                EmpSSN = string.Format(String.Format("{0:000000000}", ValEmp));
                            }
                            PhoneHome = objComman.RemoveMasking(Convert.ToString(dr["Home_Phone"]).Trim());
                            if (string.IsNullOrEmpty(EmpSSN))
                            {
                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> value is null at Row No. - " + counterrow + "</li>");
                                flag = false;
                            }

                            if (!string.IsNullOrEmpty(EmpSSN))
                            {
                                long a = 0;
                                if (long.TryParse(EmpSSN, out a) == false)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> Must be numeric at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                                if (EmpSSN.Length != 9)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> length Must be equal to 9 at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                                var SSNExists = objClientMngmt.GetEmployementInfobySSN(Convert.ToString(EmpSSN).Trim());
                                if (SSNExists == null)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> does not exists  at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                            }

                            if (string.IsNullOrEmpty(Convert.ToString(dr["Termination_Date"] != DBNull.Value ? dr["Termination_Date"] : "")))
                            {
                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Termination Date</b> value is null at Row No. - " + counterrow + "</li>");
                                flag = false;
                            }
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["Termination_Date"] != DBNull.Value ? dr["Termination_Date"] : "")))
                            {

                                DateTime a;
                                if (DateTime.TryParse(Convert.ToString(dr["Termination_Date"]).Trim(), out a) == false)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Termination Date </b> format must be MM/DD/YYYY at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                            }
                            if (string.IsNullOrEmpty(Convert.ToString(dr["Benefit_Termination_Date"] != DBNull.Value ? dr["Benefit_Termination_Date"] : "")))
                            {
                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Benefit Termination Date</b> value is null at Row No. - " + counterrow + "</li>");
                                flag = false;
                            }
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["Benefit_Termination_Date"] != DBNull.Value ? dr["Benefit_Termination_Date"] : "")))
                            {
                                DateTime a;
                                if (DateTime.TryParse(Convert.ToString(dr["Benefit_Termination_Date"]).Trim(), out a) == false)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Benefit Termination Date </b> format must be MM/DD/YYYY at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                            }
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["Reason"] != DBNull.Value ? dr["Reason"] : "")))
                            {
                                if (Convert.ToString(dr["Reason"]).Length > 1000)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Reason</b> length can not be exceed to 1000 at Row No. - " + counterrow + " </li>");
                                    flag = false;
                                }
                            }

                            // 
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["Personal_Email"] != DBNull.Value ? dr["Personal_Email"] : "")))
                            {
                                // regex should be check
                                bool emailcheck = isValidEmail(Convert.ToString(dr["Personal_Email"]));
                                if (!emailcheck)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Personal Email</b> is not valid at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                                if (Convert.ToString(dr["Personal_Email"]).Length > 100)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Personal Email</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                            }

                            if (!string.IsNullOrEmpty(PhoneHome))
                            {
                                long a = 0;
                                if (long.TryParse(PhoneHome, out a) == false)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Home Phone</b> Must be numeric at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                                if (PhoneHome.Length != 10)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Home Phone</b> length Must be 10 at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                            }
                            counterrow++;
                        }

                        if (dt.Rows.Count == 0)
                        {
                            flag = false;
                            string msgdisplay = "There is no data in excel sheet";
                            return Json(new { flag, Message = msgdisplay, JsonRequestBehavior.AllowGet });
                        }


                        if (flag)
                        {
                            var rowupdated = counterrow - 1;
                            string msgdisplay = "Total " + rowupdated + " rows validated successfully.";

                            return Json(new { flag, Message = msgdisplay, JsonRequestBehavior.AllowGet });

                        }
                        if (!flag)
                        {
                            string msgdisplay = msgError.ToString();
                            return Json(new { flag, Message = msgdisplay, JsonRequestBehavior.AllowGet });

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                return Json(new { flag, Message = ex.Message, JsonRequestBehavior.AllowGet });
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
       
        public ActionResult UploadCensusValidate(long CompanyId = 0)
        {
            Comman objComman = new Comman();
            CompanyManagement objClientMngmt = new CompanyManagement();
            string finalflag = string.Empty;
            string strCity = string.Empty;
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            StringBuilder msgError = new StringBuilder();
            long CompanyID = CompanyId == 0 ? Convert.ToInt64(Session["CompanyID"].ToString()) : CompanyId;
            StringBuilder msgSuccess = new StringBuilder();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data1 = new SelectList(cdata, "ID", "CCompanyName");
            ViewBag.Company = data1;

            bool flag = true;
            bool spouseduplicateflag = true;
            string EmpSSN = string.Empty;
            string DependentSSN = string.Empty;
            string MobilePhone = string.Empty;
            string WorkPhone = string.Empty;
            string EmergancyPhone = string.Empty;
            try
            {
                if (Request.Files.Count > 0)
                {

                    HttpPostedFileBase file = Request.Files[0];
                    if (file != null)
                    {
                        string fileName = string.Empty;
                        fileName = DateTime.UtcNow.Ticks + "-" + file.FileName;
                        var path = Server.MapPath("~/Uploads/Admin/EmpolyeeDetailsDocs/");

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);

                        OleDbConnection oledbConn = new OleDbConnection();
                        if (Path.GetExtension(file.FileName) == ".xls")
                        {
                            oledbConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            //oledbConn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + tempPath + postedFile.FileName + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";                         
                            oledbConn.Open();

                        }
                        else if (Path.GetExtension(file.FileName) == ".xlsx")
                        {
                            oledbConn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';";

                        }
                        
                        if (oledbConn.State == ConnectionState.Closed)
                            oledbConn.Open();

                        OleDbCommand cmd = new OleDbCommand();
                        OleDbDataAdapter oleda = new OleDbDataAdapter();
                        DataSet ds = new DataSet();
                        System.Data.DataTable dt = new System.Data.DataTable();

                        cmd.Connection = oledbConn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM [Sheet1$]";
                        oleda = new OleDbDataAdapter(cmd);
                        oleda.Fill(ds, "Sheet1");
                        dt = ds.Tables["Sheet1"];
                        tblCompany_Employee_BasicInfo objComEmpBasicInfo = new tblCompany_Employee_BasicInfo();
                        tblCompany_Employee_EmploymentInfo objComEmpEmpInfo = new tblCompany_Employee_EmploymentInfo();
                        tblCompany_Employee_PersonalInfo objComEmpPerInfo = new tblCompany_Employee_PersonalInfo();
                        tblCompany_Employee_DependentInfo dependentInfo = new tblCompany_Employee_DependentInfo();
                        int counterrow = 1;
                        foreach (DataRow dr in dt.Rows)
                        {
                            objComEmpBasicInfo.CompanyId = CompanyID;
                            // if ssn and Dependent_SSN are equal that is employee
                            EmpSSN = objComman.RemoveMasking(Convert.ToString(dr["SSN"]).Trim());
                            if (!string.IsNullOrEmpty(EmpSSN))
                            {
                                int ValEmp = Convert.ToInt32(EmpSSN);
                                EmpSSN = string.Format(String.Format("{0:000000000}", ValEmp));
                            }

                            DependentSSN = objComman.RemoveMasking(Convert.ToString(dr["Dependent_SSN"]).Trim());
                            if (!string.IsNullOrEmpty(DependentSSN))
                            {
                                int ValDep = Convert.ToInt32(DependentSSN);
                                DependentSSN = string.Format(String.Format("{0:000000000}", ValDep));
                            }

                            MobilePhone = objComman.RemoveMasking(Convert.ToString(dr["Mobile_Phone"]).Trim());
                            WorkPhone = objComman.RemoveMasking(Convert.ToString(dr["Work_Phone"]).Trim());
                            EmergancyPhone = objComman.RemoveMasking(Convert.ToString(dr["Emergency_Phone"]).Trim());
                            if (EmpSSN == DependentSSN)
                            {
                                if (string.IsNullOrEmpty(EmpSSN))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(EmpSSN))
                                {
                                    if (EmpSSN.Length != 9)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> length Must be equal to 9 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(EmpSSN))
                                {
                                    long a = 0;
                                    if (long.TryParse(EmpSSN, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> Must be numeric at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(EmpSSN))
                                {
                                    var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone("", "", 0, 2, EmpSSN);

                                    if (duplicateComRecord.Count > 0)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> duplicate at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (string.IsNullOrEmpty(DependentSSN))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(DependentSSN))
                                {
                                    if (DependentSSN.Length != 9)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> length Must be equal to 9 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(DependentSSN))
                                {
                                    long a = 0;
                                    if (long.TryParse(DependentSSN, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> Must be numeric at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(DependentSSN))
                                {
                                    var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone("", "", 0, 4, DependentSSN);
                                    if (duplicateComRecord.Count > 0)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> duplicate at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (string.IsNullOrEmpty(Convert.ToString(dr["First_name"] != DBNull.Value ? dr["First_name"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>First Name</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }


                                if (!string.IsNullOrEmpty(Convert.ToString(dr["First_name"] != DBNull.Value ? dr["First_name"] : "")))
                                {
                                    if (Convert.ToString(dr["First_name"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>First Name</b> length can not be exceed to 100 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Middle_Name"] != DBNull.Value ? dr["Middle_Name"] : "")))
                                {
                                    if (Convert.ToString(dr["Middle_Name"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Middle Name</b> length can not be exceed to 100 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["Last_Name"] != DBNull.Value ? dr["Last_Name"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Last Name</b> value is null at Row No. - " + counterrow + " </li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Last_Name"] != DBNull.Value ? dr["Last_Name"] : "")))
                                {
                                    if (Convert.ToString(dr["Last_Name"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Last Name</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Suffix"] != DBNull.Value ? dr["Suffix"] : "")))
                                {
                                    if (Convert.ToString(dr["Suffix"]).Length > 10)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Suffix</b> length can not be exceed to 10 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Address"] != DBNull.Value ? dr["Address"] : "")))
                                {
                                    if (Convert.ToString(dr["Address"]).Length > 500)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Address</b> length can not be exceed to 500 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["City"] != DBNull.Value ? dr["City"] : "")))
                                {
                                    if (Convert.ToString(dr["City"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>City</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["State"] != DBNull.Value ? dr["State"] : "")))
                                {
                                    if (Convert.ToString(dr["State"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>State</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["ZipCode"] != DBNull.Value ? dr["ZipCode"] : "")))
                                {
                                    if (Convert.ToString(dr["ZipCode"]).Length > 10)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>ZipCode</b> length can not be exceed to 10 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Country"] != DBNull.Value ? dr["Country"] : "")))
                                {
                                    if (Convert.ToString(dr["Country"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Country</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Email"] != DBNull.Value ? dr["Email"] : "")))
                                {
                                    // regex should be check
                                    bool emailcheck = isValidEmail(Convert.ToString(dr["Email"]));
                                    if (!emailcheck)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Email</b> is not valid at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }

                                    if (Convert.ToString(dr["Email"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Email</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                    var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone("", Convert.ToString(dr["Email"]).Trim(), 0, 2, "");
                                    if (duplicateComRecord.Count > 0)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Email </b> duplicate at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(MobilePhone))
                                {
                                    long a = 0;

                                    if (long.TryParse(MobilePhone, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Mobile Phone</b> Must be numeric at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                    if (MobilePhone.Length != 10)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Mobile Phone</b> length Must be 10 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }

                                    var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone(MobilePhone, "", 0, 2, "");

                                    if (duplicateComRecord.Count > 0)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Mobile_Phone </b> duplicate at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }


                                if (!string.IsNullOrEmpty(WorkPhone))
                                {
                                    long a = 0;
                                    if (long.TryParse(WorkPhone, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Work Phone</b> Must be numeric at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                    if (WorkPhone.Length != 10)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Work Phone</b> length Must be 10 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }


                                if (string.IsNullOrEmpty(Convert.ToString(dr["DOB"] != DBNull.Value ? dr["DOB"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>DOB</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }


                                if (!string.IsNullOrEmpty(Convert.ToString(dr["DOB"] != DBNull.Value ? dr["DOB"] : "")))
                                {

                                    DateTime a;
                                    if (DateTime.TryParse(Convert.ToString(dr["DOB"]).Trim(), out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>DOB </b> format must be MM/DD/YYYY at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }

                                }

                                if (string.IsNullOrEmpty(Convert.ToString(dr["Gender"] != DBNull.Value ? dr["Gender"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Gender</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Gender"] != DBNull.Value ? dr["Gender"] : "")))
                                {
                                    var Gender = Convert.ToString(dr["Gender"]).Trim().ToLower();

                                    if (Gender == "m" || Gender == "male")
                                    {

                                    }
                                    else if (Gender == "f" || Gender == "female")
                                    {

                                    }
                                    else
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Gender</b> Must be M or Male or F or Female at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }


                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Title"] != DBNull.Value ? dr["Title"] : "")))
                                {
                                    // regex should be check
                                    if (Convert.ToString(dr["Title"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Title</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }


                                if (string.IsNullOrEmpty(Convert.ToString(dr["Work_State"] != DBNull.Value ? dr["Work_State"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Work State</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }


                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Work_State"] != DBNull.Value ? dr["Work_State"] : "")))
                                {

                                    if (Convert.ToString(dr["Work_State"]).Trim().Length > 2)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Work State</b> length can not be exceed to 2 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                    var empStateCode = objClientMngmt.GetEmployeeStateCode(Convert.ToString(dr["Work_State"]).Trim());
                                    if (empStateCode != null)
                                    {
                                        if ((Convert.ToString(dr["Work_State"]).Trim().ToUpper() != empStateCode.StateCode.ToString().ToUpper()))
                                        {
                                            msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Work State  </b> Must be State Postal Abbreviation at Row No. - " + counterrow + " </li>");
                                            flag = false;
                                        }
                                    }
                                    else
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Work State  </b> Must be State Postal Abbreviation at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["Division"] != DBNull.Value ? dr["Division"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Division</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Division"] != DBNull.Value ? dr["Division"] : "")))
                                {
                                    if (Convert.ToString(dr["Division"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Division </b> length Must be less than or equal to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }

                                    var divisionDet = objComEmpMgnt.GetDivisions(Convert.ToString(dr["Division"]), CompanyID);
                                    if (divisionDet == null)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Division not exist</b> at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                    else
                                    {
                                        objComEmpEmpInfo.DivisionID = divisionDet.ID;
                                    }
                                }
                                string department = Convert.ToString(dr["Department"]);
                                if (!string.IsNullOrEmpty(department))
                                {
                                    if (department.Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Department </b> length Must be less than or equal to 100 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                    objComEmpBasicInfo.DepartmentID = objComEmpMgnt.GetDepartmentIDByNameandCompanyID(department, Convert.ToInt32(objComEmpBasicInfo.CompanyId));
                                    if (objComEmpBasicInfo.DepartmentID == 0)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Department not exist</b> at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }

                                if (string.IsNullOrEmpty(Convert.ToString(dr["Hire_Date"] != DBNull.Value ? dr["Hire_Date"] : "")))
                                {
                                    objComEmpEmpInfo.StartDate = null;
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Hire_Date"] != DBNull.Value ? dr["Hire_Date"] : "")))
                                {

                                    DateTime a;
                                    if (DateTime.TryParse(Convert.ToString(dr["Hire_Date"]).Trim(), out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Hire Date </b> format must be MM/DD/YYYY at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                //if (string.IsNullOrEmpty(Convert.ToString(dr["Employee_Type"] != DBNull.Value ? dr["Employee_Type"] : "")))
                                //{
                                //    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Employee Type</b> value is null at Row No. - " + counterrow + "</li>");
                                //    flag = false;
                                //}


                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Employee_Type"] != DBNull.Value ? dr["Employee_Type"] : "")))
                                {

                                    if (Convert.ToString(dr["Employee_Type"]).Trim().Length > 12)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Employee Type</b> length can not be exceed to 12 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }

                                    var EmployeeTypebool = (Convert.ToString(dr["Employee_Type"]).ToUpper().Trim() == "FT" ? true : Convert.ToString(dr["Employee_Type"]).ToUpper().Trim() == "PT" ? true : Convert.ToString(dr["Employee_Type"]).ToUpper().Trim() == "FT-E" ? true : false);
                                    if (EmployeeTypebool == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Employee Type</b> Must be code FT or PT or FT-E at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }


                                if (string.IsNullOrEmpty(Convert.ToString(dr["Payroll_Frequency"] != DBNull.Value ? dr["Payroll_Frequency"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Payroll Frequency</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }


                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Payroll_Frequency"] != DBNull.Value ? dr["Payroll_Frequency"] : "")))
                                {

                                    if (Convert.ToString(dr["Payroll_Frequency"]).Trim().Length > 9)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Payroll Frequency</b> length can not be exceed to 9 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }

                                    var MonthlyStatusbool = (Convert.ToString(dr["Payroll_Frequency"]).ToUpper().Trim() == "WEEKLY" ? true : Convert.ToString(dr["Payroll_Frequency"]).ToUpper().Trim() == "BIWEEKLY" ? true : Convert.ToString(dr["Payroll_Frequency"]).ToUpper().Trim() == "BIMONTHLY" ? true : Convert.ToString(dr["Payroll_Frequency"]).ToUpper().Trim() == "MONTHLY" ? true : false);
                                    if (MonthlyStatusbool == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Payroll Frequency</b> Must be Weekly OR BiWeekly OR BiMonthly OR Monthly at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }


                                if (!string.IsNullOrEmpty(Convert.ToString(dr["EmployeeId"] != DBNull.Value ? dr["EmployeeId"] : "")))
                                {
                                    if ((Convert.ToString(dr["EmployeeId"])).Length > 10)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>EmployeeId </b> length Must be less than or equal to 10 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }

                                    var empCode = objClientMngmt.GetEmployeeCode(Convert.ToString(dr["EmployeeId"]));
                                    if (empCode != null)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>EmployeeId </b> duplicate at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }

                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Relation"] != DBNull.Value ? dr["Relation"] : "")))
                                {
                                    if ((Convert.ToString(dr["Relation"])).Length > 6)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Relation </b> length Must be less than or equal to 6 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }

                                string Salary = Convert.ToString(dr["Annual_Salary"]);
                                if (string.IsNullOrEmpty(Salary))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Annual Salary</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                                if (Salary.Length > 10)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Annual Salary </b> length Must be less than or equal to 10 at Row No. - " + counterrow + " </li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(Salary))
                                {
                                    decimal a = 0;
                                    if (decimal.TryParse(Salary, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Salary </b> Must be numeric at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }

                                string Bonus = Convert.ToString(dr["Bonus"]);


                                if (string.IsNullOrEmpty(Convert.ToString(dr["Bonus"] != DBNull.Value ? dr["Bonus"] : "")))
                                {
                                    objComEmpEmpInfo.Bonus = 0;
                                }
                                if (Bonus.Length > 10)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Bonus </b> length Must be less than or equal to 10 at Row No. - " + counterrow + " </li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(Bonus))
                                {
                                    decimal a = 0;

                                    if (decimal.TryParse(Bonus, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Bonus </b> Must be numeric at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }

                                string Commission = Convert.ToString(dr["Commission"]);
                                if (string.IsNullOrEmpty(Convert.ToString(dr["Commission"] != DBNull.Value ? dr["Commission"] : "")))
                                {
                                    objComEmpEmpInfo.Commisiion = 0;
                                }
                                if (Commission.Length > 10)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Commission </b> length Must be less than or equal to 10 at Row No. - " + counterrow + " </li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(Commission))
                                {
                                    decimal a = 0;

                                    if (decimal.TryParse(Commission, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Commission </b> Must be numeric at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Relationship_with_Contact"] != DBNull.Value ? dr["Relationship_with_Contact"] : "")))
                                {
                                    if ((Convert.ToString(dr["Relationship_with_Contact"]).Length > 100))
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Relation With Contact</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Emergency_Contact_Name"] != DBNull.Value ? dr["Emergency_Contact_Name"] : "")))
                                {
                                    if (Convert.ToString(dr["Emergency_Contact_Name"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Emergency Contact Name</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(EmergancyPhone))
                                {
                                    long a = 0;

                                    if (long.TryParse(EmergancyPhone, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Emergency Phone </b> Must be numeric at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                    if (EmergancyPhone.Length != 10)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Emergency Phone </b> length Must be 10  at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                            }
                            else
                            {
                                // this is for dependent loop
                                // check  validation for depedent                                
                                if (string.IsNullOrEmpty(EmpSSN))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                                if (!string.IsNullOrEmpty(EmpSSN))
                                {
                                    if (EmpSSN.Length != 9)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> length Must be equal to 9 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(EmpSSN))
                                {
                                    long a = 0;

                                    if (long.TryParse(EmpSSN, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> Must be numeric at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (!string.IsNullOrEmpty(EmpSSN))
                                {
                                    var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone("", "", 0, 2, EmpSSN);

                                    if (duplicateComRecord.Count > 0)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>SSN</b> duplicate at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }


                                if (string.IsNullOrEmpty(DependentSSN))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }
                                if (!string.IsNullOrEmpty(DependentSSN))
                                {
                                    if (DependentSSN.Length != 9)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> length Must be equal to 9 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(DependentSSN))
                                {
                                    long a = 0;

                                    if (long.TryParse(DependentSSN, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> Must be numeric at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(DependentSSN))
                                {
                                    var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone("", "", 0, 4, DependentSSN);


                                    if (duplicateComRecord.Count > 0)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Dependent SSN</b> duplicate at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["First_name"] != DBNull.Value ? dr["First_name"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>First Name</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }


                                if (!string.IsNullOrEmpty(Convert.ToString(dr["First_name"] != DBNull.Value ? dr["First_name"] : "")))
                                {
                                    if (Convert.ToString(dr["First_name"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>First Name</b> length can not be exceed to 100 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }




                                if (string.IsNullOrEmpty(Convert.ToString(dr["Last_Name"] != DBNull.Value ? dr["Last_Name"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Last Name</b> value is null at Row No. - " + counterrow + " </li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Last_Name"] != DBNull.Value ? dr["Last_Name"] : "")))
                                {
                                    if (Convert.ToString(dr["Last_Name"]).Length > 100)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Last Name</b> length can not be exceed to 100 at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (string.IsNullOrEmpty(Convert.ToString(dr["DOB"] != DBNull.Value ? dr["DOB"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>DOB</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }




                                if (!string.IsNullOrEmpty(Convert.ToString(dr["DOB"] != DBNull.Value ? dr["DOB"] : "")))
                                {
                                    bool checkDatefromat = ValidateDate(Convert.ToString(dr["DOB"]));
                                    if (!checkDatefromat)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>DOB</b> format must be MM/DD/YYYY at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }
                                }

                                if (string.IsNullOrEmpty(Convert.ToString(dr["Gender"] != DBNull.Value ? dr["Gender"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Gender</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Gender"] != DBNull.Value ? dr["Gender"] : "")))
                                {
                                    var Gender = Convert.ToString(dr["Gender"]).Trim().ToLower();

                                    if (Gender == "m" || Gender == "male")
                                    {

                                    }
                                    else if (Gender == "f" || Gender == "female")
                                    {

                                    }
                                    else
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Gender</b> Must be M or Male or F or Female at Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }

                                }

                                if (string.IsNullOrEmpty(Convert.ToString(dr["Relation"] != DBNull.Value ? dr["Relation"] : "")))
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Relation</b> value is null at Row No. - " + counterrow + "</li>");
                                    flag = false;
                                }


                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Relation"] != DBNull.Value ? dr["Relation"] : "")))
                                {
                                    if (Convert.ToString(dr["Relation"]).Length > 20)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Relation</b> length can not be exceed to 20 at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }

                                }

                            }

                            counterrow++;
                        }

                        if (dt.Rows.Count == 0)
                        {
                            flag = false;
                            string msgdisplay = "There is no data in excel sheet";
                            return Json(new { flag, Message = msgdisplay, JsonRequestBehavior.AllowGet });
                        }


                        if (flag)
                        {
                            var rowupdated = counterrow - 1;
                            string msgdisplay = "Total " + rowupdated + " rows validated successfully.";

                            return Json(new { flag, Message = msgdisplay, JsonRequestBehavior.AllowGet });

                        }
                        if (!flag)
                        {
                            string msgdisplay = msgError.ToString();
                            return Json(new { flag, Message = msgdisplay, JsonRequestBehavior.AllowGet });

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                return Json(new { flag, Message = ex.Message, JsonRequestBehavior.AllowGet });
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public static bool ValidateDate(string date)
        {
            string strRegex = @"^(((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(date))
                return (true);
            else
                return (false);
        }
        [HttpPost]
        public ActionResult ManageBroker(BrokerNew details)
        {
            tblBroker objBroker = new tblBroker();
            if (details.BrokerId != 0)
            {
                long bId = details.BrokerId;
                objBroker = context.tblBrokers.Where(a => a.BrokerId == bId && a.IsDeleted == false).FirstOrDefault();
                objBroker.DateModified = DateTime.UtcNow;
            }
            objBroker.Broker = details.BrokerName;
            objBroker.Street = details.Street;
            objBroker.ZipCode = details.ZipCode;
            objBroker.WorkPhone = details.BrokerPhone;
            long lngCountryIDB = new Comman().GetCountry(details.Country);
            long lngStateIDB = new Comman().GetState(details.State, lngCountryIDB);
            long lngCityIDB = new Comman().GetCity(details.City, lngStateIDB, lngCountryIDB);
            objBroker.Country = lngCountryIDB;
            objBroker.State = lngStateIDB;
            objBroker.City = lngCityIDB;
            if (details.BrokerId == 0)
            {
                objBroker.DateCreated = DateTime.UtcNow;
                objBroker.IsActive = true;
                objBroker.IsDeleted = false;
                context.tblBrokers.Add(objBroker);
            }
            context.SaveChanges();
            
            
            return Json(objBroker.BrokerId);
        }
        [HttpPost]
        public ActionResult SaveBroker(BrokerNew details)
        {
            tblSubBroker obj = new tblSubBroker();
            if (details.SubBrokerId != 0)
            {
                obj = context.tblSubBrokers.Where(a => a.SubBrokerId == details.SubBrokerId).FirstOrDefault();
            }
            obj.BrokerId = details.BrokerId;
            obj.Broker = details.BrokerName;
            obj.FirstName = details.FirstName;
            obj.LastName = details.LastName;
            obj.Email = details.Email;
            obj.Phone = details.Phone;
            obj.WorkPhone = details.WorkPhone;
            obj.AdministratorType = details.AdministratorType;
            obj.DateCreated = DateTime.UtcNow;
            obj.DateModified = DateTime.UtcNow;
            if (details.SubBrokerId == 0)
            {
                obj.IsActive = true;
                obj.IsDeleted = false;
                obj.CompanyStatus = true;
                context.tblSubBrokers.Add(obj);
            }
            context.SaveChanges();
            return Json(obj.SubBrokerId);
        }
        public ActionResult UpdateStatusSubBroker(long SubBrokerId, bool Status,string Type)
        {
            return Json(Type == "Status" ? objClientMngmt.UpdateStatusSubBroker(SubBrokerId, Status) : objClientMngmt.DeactivateSubBroker(SubBrokerId));
        }
    }
}
