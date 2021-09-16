using Newtonsoft.Json;
using PowerIBroker.Models;
using PowerIBrokerBusinessLayer;
using PowerIBrokerBusinessLayer.UserLogin;
using PowerIBrokerDataLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PowerIBroker.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        CompanyManagement objClientMngmt = new CompanyManagement();
        PowerIBrokerBusinessLayer.Comman objComman = new PowerIBrokerBusinessLayer.Comman();
        private void LoadHomePageContent()
        {

            CmsManagement cmsObj = new CmsManagement();
            var homePageContent = cmsObj.GetHomePageContent();
            ViewBag.HomePageContent = homePageContent;
            ViewBag.MetaTitle = homePageContent.MetaTitle;
            ViewBag.MetaDescription = homePageContent.MetaDescriptions;
            ViewBag.MetaKeywords = homePageContent.MetaKeywords;
            ViewBag.MetaRobots = homePageContent.MetaKeyPhrase;

            var homePageClientBanners = cmsObj.GetHomePageBannerClients();
            ViewBag.HomePageClientBanners = homePageClientBanners;

            var benefitsPageContent = cmsObj.GetBenefitsPageContent();
            ViewBag.BenefitsPageContent = benefitsPageContent;

            var requestDemoContent = cmsObj.GetRequestDemoContent();
            ViewBag.RequestDemoContent = requestDemoContent;

            var featureContents = cmsObj.GetFeaturesContent();
            ViewBag.FeatureContents = featureContents;

            var homePageTopContents = cmsObj.GetHomePageTopContent();
            ViewBag.HomePageTopContents = homePageTopContents;

            var pricingContent = cmsObj.GetPricingPageContent();
            ViewBag.PricingContent = pricingContent;
        }
        public ActionResult Index(string ReturnUrl = "")
        {
            ViewBag.ReturnUrl = Convert.ToString(ReturnUrl);
            Areas.Broker.Models.ClientMasterValidation Obj = new Areas.Broker.Models.ClientMasterValidation();
            bool checklink = ReturnUrl.Contains("editbenefitlifeevent");
            bool checklinkediteoi = ReturnUrl.Contains("editeoi");
            if (checklink == true || checklinkediteoi == true)
            {
                TempData["MessagePoup"] = "Yes";

            }
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                TempData["MessagePoup"] = "Yes";
            }
            LoadHomePageContent();
            return View(Obj);
        }
        public ActionResult SSOTest()
        {
            if(Session["saml_sso_username"]!=null)
            {
                ViewBag.Email = Session["saml_sso_username"] as string;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(CompanyMaster objCMaster)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    //    if (!string.IsNullOrEmpty(admID))
                    //    {
                    string randomPwd = Helper.CreateRandomPassword(8);
                    string randomPassword = Helper.Encrypt(randomPwd);
                    objCMaster.IsActive = false;
                    objCMaster.CPassword = randomPassword;
                    //objCMaster.OwnerId = adminID;
                    objCMaster.OwnerType = "C";
                    objCMaster.NoOfAttempts = 0;
                    objCMaster.Status = "P";
                    if (!string.IsNullOrEmpty(objCMaster.CContact))
                    {
                        objCMaster.CContact = objComman.RemoveMasking(objCMaster.CContact);
                    }
                    else
                    {
                        objCMaster.CContact = null;
                    }
                    int result = objClientMngmt.AddClient(objCMaster);
                    if (result == 1)
                    {
                        TempData["MessageType"] = "Success1";
                        TempData["CustomMessage"] = "Company saved successfully.";
                        var adminEmailID = Helper.GetCompanyAdminEmail();
                        string body_Demo_request = string.Empty;
                        string baseurl = Helper.GetBaseUrl();
                        string DemoReq_email_subject = "";
                        DemoReq_email_subject = "Email Confirmation";
                        body_Demo_request = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/Demo_Request.html"));
                        body_Demo_request = body_Demo_request.Replace("@@name@@", objCMaster.CName);
                        body_Demo_request = body_Demo_request.Replace("@@Email@@", objCMaster.CEmail);
                        body_Demo_request = body_Demo_request.Replace("@@Phone@@", objCMaster.CContact);
                        body_Demo_request = body_Demo_request.Replace("@@Company@@", objCMaster.CCompanyName);
                        body_Demo_request = body_Demo_request.Replace("@@NoOfEmp@@", objCMaster.NoOfEmp.ToString());
                        body_Demo_request = body_Demo_request.Replace("@@Message@@", objCMaster.CompanyMessage);
                        body_Demo_request = body_Demo_request.Replace("@@url@@", baseurl);
                        if (!string.IsNullOrEmpty(adminEmailID.AdminEmail))
                        {
                            Helper.SendAdminEmail(adminEmailID.AdminEmail, body_Demo_request, DemoReq_email_subject);
                        }


                        string body_Schedule_Demo = string.Empty;
                        string DemoReq_Schedule_Demo = string.Empty;
                        DemoReq_Schedule_Demo = "Thanks for asking about EnrollMyGroup";
                        body_Schedule_Demo = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/Schedule_Demo.html"));
                        body_Schedule_Demo = body_Schedule_Demo.Replace("@@name@@", objCMaster.CName);
                        body_Schedule_Demo = body_Schedule_Demo.Replace("@@Email@@", objCMaster.CEmail);
                        body_Schedule_Demo = body_Schedule_Demo.Replace("@@url@@", baseurl);
                        if (!string.IsNullOrEmpty(objCMaster.CEmail))
                        {
                            // Date21Sept as per client Discussion code comment
                            Helper.SendEmail(objCMaster.CEmail, body_Schedule_Demo, DemoReq_Schedule_Demo);
                        }


                        int statusPhone = 0;
                        if (!string.IsNullOrEmpty(objCMaster.CContact))
                        {
                            // Date21Sept as per client Discussion code comment
                            statusPhone = SendTwilloMessage.SendmessageTwillo(objCMaster.CContact, "Thanks for asking about EnrollMyGroup. \n \n We’re happy to answer questions and demo our product.\n \n Let us know when might be a good time to call you.");
                        }

                        //////////////////////////////////////////////////////////////   

                        //////////////////////////////////////////////////////////////

                        //////////////////////////////////////////////////////////////


                    }
                    else if (result == 2)
                    {
                        TempData["MessageType"] = "Error1";
                        TempData["CustomMessage"] = "Email id already exists while processing your request.";

                    }
                    else
                    {
                        TempData["MessageType"] = "Error1";
                        TempData["CustomMessage"] = "Some error while processing your request.";

                    }
                    ModelState.Clear();
                    // }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("AddClient", ex.Message);

            }
            LoadHomePageContent();
            return View();
        }
        [HttpPost]
        public ActionResult CheckAdminExistence(string email)
        {
            string status = string.Empty;
            try
            {
                UserManagement um = new UserManagement();
                var user = um.CheckAdminExistence(email);
                if (user != null)
                {

                    Guid UniqueID = Guid.NewGuid();
                    string verifier = UniqueID.ToString() + "-" + DateTime.UtcNow.Ticks.ToString();
                    PasswordResetRequest obj = new PasswordResetRequest()
                    {
                        Email = user.Email,
                        Verifier = verifier,
                        IsActive = true
                    };

                    var request = um.GeneratePasswordResetRequest(obj);
                    if (request.Id != 0)
                    {
                        int i = Helper.PasswordResetEmail(user.Username, user.Email, "Home/PasswordReset", request.Verifier);
                        status = i > 0 ? "success" : "error";
                    }
                    else
                    {
                        status = "error";
                    }


                }
                else
                {
                    status = "not_found";
                }
            }
            catch (Exception ex)
            {
                status = "error";
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return Content(status, "application/json");
        }
        public ActionResult PasswordReset(string email, string verifier)
        {
            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(verifier))
                {
                    UserManagement um = new UserManagement();
                    email = email.Replace(" ", "+");
                    email = Helper.Decrypt(email);
                    var request = um.CheckPasswordResetRequest(email, verifier);
                    if (request != null)
                    {
                        ViewData["MessageType"] = "success";
                        ViewData["Message"] = "Thank you for verifying, kindly create your password";
                        ViewBag.UserEmail = Helper.Decrypt(email);
                        ViewBag.Verifier = verifier;
                        ViewBag.IsVerifierValid = true;

                    }
                    else
                    {
                        ViewData["MessageType"] = "error";
                        ViewData["Message"] = "Some error occurred while processing the request. Please try again.";
                    }

                }

            }
            catch (Exception ex)
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Some error occurred while processing the request. Please try again.";
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public ActionResult PasswordReset(string email, string password, string verifier)
        {
            try
            {
                if (!string.IsNullOrEmpty(password))
                {
                    UserManagement um = new UserManagement();
                    int i = um.ResetAdminPassword(new User { Email = email, Password = password }, verifier);
                    if (i > 0)
                    {
                        ViewData["MessageType"] = "success";
                        ViewData["Message"] = "Your password has been successfully changed, kindly login to your dashboard.";
                    }
                    else
                    {
                        ViewData["MessageType"] = "error";
                        ViewData["Message"] = "Some error occurred while processing the request. Please try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["MessageType"] = "error";
                ViewData["Message"] = "Some error occurred while processing the request. Please try again.";
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public ActionResult CheckUserExistence(string email)
        {
            string status = string.Empty;
            try
            {
                UserManagement um = new UserManagement();

                Administration objAdmin = new Administration();
                var userTypeBroker = objAdmin.CheckUserExistenceForAdmin(email);
                if (userTypeBroker != null)
                {
                    if (userTypeBroker.EntityType == "B")
                    {
                        Guid UniqueID = Guid.NewGuid();
                        string verifier = UniqueID.ToString() + "-" + DateTime.UtcNow.Ticks.ToString();
                        PasswordResetRequest obj = new PasswordResetRequest()
                        {
                            Email = userTypeBroker.Email,
                            Verifier = verifier,
                            IsActive = true
                        };

                        var info = um.GetBrokerInfoBroker(userTypeBroker.Email);

                        var request = um.GeneratePasswordResetRequest(obj);

                        if (request.Id != 0)
                        {
                            um.UpdateBrokerPasswordStatus(userTypeBroker.Email);
                            int i = Helper.PasswordResetEmail(string.IsNullOrEmpty(info.FirstName) ? "" : info.FirstName + " " + info.LastName, info.Email, "/Home/PasswordUserReset", request.Verifier);

                            //string baseurl = Helper.GetBaseUrl();
                            //string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/ForgetPassword.html"));  // email template
                            //string ActivationUrl = baseurl + "/Home/PasswordUserReset" + "?email=" + info.Email + "&verifier=" + request.Verifier + "&flag=" + 0;
                            //Mailbody = Mailbody.Replace("###HostUrl####", baseurl);

                            //Mailbody = Mailbody.Replace("###ActivationUrl####", ActivationUrl);                           
                            //if (!string.IsNullOrEmpty(info.Email))
                            //{
                            //    Helper.SendEmail(info.Email, Mailbody, "Password Reset");

                            //}
                            //status = "sucess";
                            status = i > 0 ? "success" : "error";
                        }
                        else
                        {
                            status = "error";
                        }
                    }
                }
                else
                {
                    var user = um.CheckUserExistence(email);
                    if (user != null)
                    {

                        Guid UniqueID = Guid.NewGuid();
                        string verifier = UniqueID.ToString() + "-" + DateTime.UtcNow.Ticks.ToString();
                        PasswordResetRequest obj = new PasswordResetRequest()
                        {
                            Email = user.Email,
                            Verifier = verifier,
                            IsActive = true
                        };
                        string cname = "";
                        cname = um.GetUserInfo(user.Email).FirstName;
                        var request = um.GeneratePasswordResetRequest(obj);
                        if (request.Id != 0)
                        {
                            int i = Helper.PasswordResetEmail(string.IsNullOrEmpty(cname) ? "" : cname, user.Email, "/Home/PasswordUserReset", request.Verifier);
                            if (um.GetUserInfo(user.Email) == null)
                            {
                                var details = um.GetUserInfoP(email);
                                if (details != null)
                                {
                                    System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
                                    string PageURL = (string)settingsReader.GetValue("PageURL", typeof(String));
                                    string EcEmail = Helper.Encrypt(email);
                                    string ActivationUrl = "Your password has been reset.  Click this link to set up a new password and login "+ PageURL + "/Home/PasswordUserReset" + "?email=" + EcEmail + "&verifier=" + verifier + "&flag=" + 0;
                                    // Date21Sept as per client Discussion code comment 
                                    i = SendTwilloMessage.SendmessageTwillo(email, ActivationUrl);
                                }
                            }

                            status = i > 0 ? "success" : "error";
                        }
                        else
                        {
                            status = "error";
                        }


                    }
                    else
                    {
                        status = "not_found";
                    }
                }


            }
            catch (Exception ex)
            {
                status = "error";
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return Json(status);
        }
        public ActionResult PasswordUserReset(string email, int flag = 1)
        {

            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    UserManagement um = new UserManagement();
                    UserAuthentication obj = new UserAuthentication();
                    email = email.Replace(" ", "+");
                    var emailchk = Helper.Decrypt(email);
                    var data = um.EmployeeRedirection(emailchk);
                    if (data != null)
                    {
                        if (!string.IsNullOrEmpty(data.Password))
                        {
                            TempData["EmployeeRedirection"] = "Yes";
                          //  TempData["Email"] = emailchk;
                            //HttpCookie myCookie = Request.Cookies["UserEmail"];
                          //  Request.Cookies["UserEmail"] = emailchk.ToString();
                            Response.Cookies["UserEmail"].Value = emailchk;
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    ViewData["MessageType"] = "success";
                    ViewData["Message"] = "Thank you for verifying, kindly create your password";
                    ViewBag.UserEmail = Helper.Decrypt(email);
                    ViewBag.IsVerifierValid = true;
                    ViewBag.Flag = flag;

                }

            }
            catch (Exception ex)
            {
                TempData["MessageType"] = "error";
                TempData["Message"] = "Some error occurred while processing the request. Please try again";
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public ActionResult PasswordUserReset(string email, string password, string verifier, int flag = 1)
        {
            try
            {
                if (!string.IsNullOrEmpty(password))
                {
                    int i = 0;
                    UserManagement um = new UserManagement();

                    Administration objAdmin = new Administration();
                    var userTypeBroker = objAdmin.CheckUserExistenceForAdmin(email);
                    if (userTypeBroker != null)
                    {
                        if (userTypeBroker.EntityType == UserLoginType.Broker)
                        {
                            var info = um.GetBrokerInfoBroker(email);
                            if (info.IsPassword == false)
                            {
                                i = um.ResetUserPasswordBroker(new User { Email = email, Password = password }, verifier);
                                if (i > 0)
                                {

                                    Session["SubBrokerId"] = info.SubBrokerId;
                                    Session["BrokerId"] = info.BrokerId;
                                    Session["EntityType"] = userTypeBroker.EntityType;
                                    Session["Name"] = info.FirstName + " " + info.LastName;
                                    return RedirectToAction("ManageCompany", "Broker");
                                }
                            }
                            else
                            {
                                ViewBag.UserEmail = email;
                                ViewBag.Verifier = "";
                                ViewBag.Flag = "1";
                                ViewBag.MessageType = "error";
                                ViewBag.Message = "You have already set the password.";

                            }

                        }
                    }
                    else
                    {

                        var user = um.CheckUserExistence(email);
                        if (user.EntityType == "C")
                        {
                            i = um.ResetCompanyPassword(new User { Email = email, Password = password }, verifier);
                            if (i > 0)
                            {
                                var info = um.GetUserInfo(email); ;//um.GetCompanyInfo(email);
                                ViewData["MessageType"] = "success";
                                Session["CompanyId"] = info.CompanyId;
                                Session["CompanyEmail"] = info.Email;
                                Session["CompanyName"] = info.Email;
                                Session["ComEmployeeId"] = info.ID;
                                if (flag != 0)
                                {
                                    Session["CPopUp"] = "PopUp";
                                }

                                //Session["CmpanyLogin"] = null;
                                return RedirectToAction("Index", "Dashboard", new { area = "Company" });
                                //ViewData["Message"] = "Your password has been successfully changed, kindly login to your dashboard.";
                            }
                        }
                        if (user.EntityType == "E")
                        {
                            i = um.ResetUserPassword(new User { Email = email, Password = password }, verifier);
                            if (i > 0)
                            {
                                var info = um.GetUserInfo(email);
                                Session["EmployeeId"] = info.ID;
                                Session["EmployeeCompanyId"] = info.CompanyId;
                                Session["EmployeeEmail"] = info.Email;

                                if (info.IsLogin == 0)
                                {
                                    int result = um.updateFirstLogin(email);
                                    return RedirectToAction("Index", "Basicinfo", new { area = "Employee" });
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
                                }
                                //Session["CmpanyLogin"] = null;
                                //  return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
                                // return RedirectToAction("Index", "Basicinfo", new { area = "Employee" });
                                //ViewData["Message"] = "Your password has been successfully changed, kindly login to your dashboard.";
                            }
                        }
                        else
                        {
                            ViewData["MessageType"] = "error";
                            ViewData["Message"] = "Some error occurred while processing the request. Please try again.";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ViewData["MessageType"] = "error";
                ViewData["Message"] = "Some error occurred while processing the request. Please try again.";
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult DataTableServerSide2()
        {
            return View();

        }
        public ActionResult ShowMessage()
        {
            return View();
        }
        public string DataTable3(string request, string ddl, string pageno)
        {
            int startpage = Convert.ToInt32(pageno);
            int EndPage = Convert.ToInt32(pageno) + Convert.ToInt32(ddl);
            string search = string.Empty;
            var d = JsonConvert.DeserializeObject(request);
            var d1 = (Newtonsoft.Json.Linq.JObject)(d);
            var jss = new JavaScriptSerializer();
            var table = jss.Deserialize<dynamic>(request);
            var ds = table as System.Collections.Generic.Dictionary<string, object>;
            int i = 0;
            int draw1 = 1;
            string SortColumn = string.Empty;
            string SortType = string.Empty;
            foreach (var dr in ds)
            {
                if (i == 0)
                {
                    draw1 = Convert.ToInt32(dr.Value);
                }
                if (i == 2)
                {
                    var Ordarable = (object[])dr.Value;
                    var order = (Dictionary<string, object>)Ordarable[0];
                    int ordarablecount = 0;
                    foreach (var d12 in order)
                    {
                        if (ordarablecount == 0)
                        {
                            SortColumn = d12.Value.ToString();
                        }
                        else
                        {
                            SortType = d12.Value.ToString();
                        }
                        ordarablecount++;

                    }

                }

                if (i == 5)
                {
                    var s = (Dictionary<string, object>)dr.Value;
                    int j = 0;
                    foreach (var dsd in s)
                    {
                        if (j == 0)
                        {
                            search = dsd.Value.ToString();
                        }
                        j++;
                    }

                }

                i++;
            }
            var collection = new List<YourModelClass1>() {

                new YourModelClass1{Id=1, first_name="amit",last_name="12345",position="APPROVED",salary="500",start_date="2011/04/25",office="Edinburgh"},
                  new YourModelClass1{Id=1,first_name="sumit",last_name="shukla",position="APPROVED",salary="400",start_date="2011/04/25",office="Edinburgh"},
                    new YourModelClass1{Id=1,first_name="raja",last_name="hello",position="APPROVED",salary="300",start_date="2011/04/25",office="Edinburgh"},
                      new YourModelClass1{Id=1,first_name="neraj",last_name="rajat",position="System Architect",salary="200",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1,first_name="rakesh",last_name="neeraj",position="System Architect",salary="100",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1, first_name="amit1",last_name="dwivedie",position="System Architect",salary="500",start_date="2011/04/25",office="Edinburgh"},
                  new YourModelClass1{Id=1,first_name="sumit1",last_name="rajat",position="System Architect",salary="400",start_date="2011/04/25",office="Edinburgh"},
                    new YourModelClass1{Id=1,first_name="raja1",last_name="12345",position="System Architect",salary="300",start_date="2011/04/25",office="Edinburgh"},
                      new YourModelClass1{Id=1,first_name="neraj1",last_name="12345",position="System Architect",salary="200",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1,first_name="rakesh1",last_name="12345",position="System Architect",salary="100",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{ Id=1,first_name="amit2",last_name="12345",position="System Architect",salary="500",start_date="2011/04/25",office="Edinburgh"},
                  new YourModelClass1{Id=1,first_name="sumit2",last_name="12345",position="System Architect",salary="400",start_date="2011/04/25",office="Edinburgh"},
                    new YourModelClass1{Id=1,first_name="raja2",last_name="12345",position="System Architect",salary="300",start_date="2011/04/25",office="Edinburgh"},
                      new YourModelClass1{Id=1,first_name="neraj2",last_name="12345",position="System Architect",salary="200",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1,first_name="rakesh2",last_name="12345",position="System Architect",salary="100",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1, first_name="amit3",last_name="12345",position="System Architect",salary="500",start_date="2011/04/25",office="Edinburgh"},
                  new YourModelClass1{Id=1,first_name="sumit3",last_name="12345",position="System Architect",salary="400",start_date="2011/04/25",office="Edinburgh"},
                    new YourModelClass1{Id=1,first_name="raja3",last_name="12345",position="System Architect",salary="300",start_date="2011/04/25",office="Edinburgh"},
                      new YourModelClass1{Id=1,first_name="neraj3",last_name="12345",position="System Architect",salary="200",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1,first_name="rakesh3",last_name="12345",position="System Architect",salary="100",start_date="2011/04/25",office="Edinburgh"},
                          new YourModelClass1{Id=1, first_name="amit",last_name="12345",position="System Architect",salary="500",start_date="2011/04/25",office="Edinburgh"},
                  new YourModelClass1{Id=1,first_name="sumit4",last_name="12345",position="System Architect",salary="400",start_date="2011/04/25",office="Edinburgh"},
                    new YourModelClass1{Id=1,first_name="raja4",last_name="12345",position="System Architect",salary="300",start_date="2011/04/25",office="Edinburgh"},
                      new YourModelClass1{Id=1,first_name="neraj4",last_name="nanwal",position="System Architect",salary="200",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1,first_name="rakesh4",last_name="12345",position="System Architect",salary="100",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1, first_name="amit15",last_name="12345",position="System Architect",salary="500",start_date="2011/04/25",office="Edinburgh"},
                  new YourModelClass1{Id=1,first_name="sumit15",last_name="12345",position="System Architect",salary="400",start_date="2011/04/25",office="Edinburgh"},
                    new YourModelClass1{Id=1,first_name="raja15",last_name="12345",position="System Architect",salary="300",start_date="2011/04/25",office="Edinburgh"},
                      new YourModelClass1{Id=1,first_name="neraj15",last_name="12345",position="System Architect",salary="200",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1,first_name="rakesh15",last_name="12345",position="System Architect",salary="100",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1, first_name="amit26",last_name="12345",position="System Architect",salary="500",start_date="2011/04/25",office="Edinburgh"},
                  new YourModelClass1{Id=1,first_name="sumit26",last_name="12345",position="System Architect",salary="400",start_date="2011/04/25",office="Edinburgh"},
                    new YourModelClass1{Id=1,first_name="raja26",last_name="12345",position="System Architect",salary="300",start_date="2011/04/25",office="Edinburgh"},
                      new YourModelClass1{Id=1,first_name="neraj26",last_name="12345",position="System Architect",salary="200",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1,first_name="rakesh26",last_name="12345",position="System Architect",salary="100",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1, first_name="amit37",last_name="12345",position="System Architect",salary="500",start_date="2011/04/25",office="Edinburgh"},
                  new YourModelClass1{Id=1,first_name="sumit37",last_name="12345",position="System Architect",salary="400",start_date="2011/04/25",office="Edinburgh"},
                    new YourModelClass1{Id=1,first_name="raja37",last_name="12345",position="System Architect",salary="300",start_date="2011/04/25",office="Edinburgh"},
                      new YourModelClass1{Id=1,first_name="neraj37",last_name="12345",position="System Architect",salary="200",start_date="2011/04/25",office="Edinburgh"},
                        new YourModelClass1{Id=1,first_name="rakesh37",last_name="12345",position="System Architect",salary="100",start_date="2011/04/25",office="Edinburgh"},

            };
            int TotalCount = 0;
            if (!string.IsNullOrEmpty(search))
            {
                collection = collection.Where(a => a.first_name.ToLower().Contains(search) || a.last_name.ToLower().Contains(search) || a.position.ToLower().Contains(search)).ToList();
            }
            TotalCount = collection.Count;

            if (!string.IsNullOrEmpty(SortColumn))
            {
                // collection= SortColumn == "0" ?(SortType == "asc"? collection.OrderBy(a=>a.first_name):collection.OrderByDescending(a=>a.first_name)):
                switch (SortColumn)
                {
                    case "0":
                        collection = SortType == "asc" ? collection.OrderBy(a => a.first_name).ToList() : collection.OrderByDescending(a => a.first_name).ToList();
                        break;
                    case "1":
                        collection = SortType == "asc" ? collection.OrderBy(a => a.last_name).ToList() : collection.OrderByDescending(a => a.last_name).ToList();
                        break;
                    case "2":
                        collection = SortType == "asc" ? collection.OrderBy(a => a.position).ToList() : collection.OrderByDescending(a => a.position).ToList();
                        break;
                    case "3":
                        collection = SortType == "asc" ? collection.OrderBy(a => a.office).ToList() : collection.OrderByDescending(a => a.office).ToList();
                        break;
                    case "4":
                        collection = SortType == "asc" ? collection.OrderBy(a => a.start_date).ToList() : collection.OrderByDescending(a => a.start_date).ToList();
                        break;
                    case "5":
                        collection = SortType == "asc" ? collection.OrderBy(a => a.salary).ToList() : collection.OrderByDescending(a => a.salary).ToList();
                        break;
                }
            }
            if (pageno != null)
            {
                collection = collection.Skip(Convert.ToInt32(pageno)).Take(Convert.ToInt32(Convert.ToInt32(ddl))).ToList();
            }

            dynamic collectionWrapper = new
            {
                draw = draw1,
                recordsTotal = TotalCount,
                recordsFiltered = TotalCount,
                data = collection

            };

            var output = JsonConvert.SerializeObject(collectionWrapper);
            return output;
        }
        [HttpGet]
        public ActionResult PasswordSSN()
        {

            return View();
        }
        [HttpPost]
        public ActionResult PasswordSSN(string SSN, string resetPassword, string Email, string Phone)
        {
            tblCompany_Employee_BasicInfo objb = new tblCompany_Employee_BasicInfo();
            tblCompany_Employee_PersonalInfo objp = new tblCompany_Employee_PersonalInfo();
            tblCompany_Employee_EmploymentInfo objE = new tblCompany_Employee_EmploymentInfo();
            objE = new UserManagement().GetEmployeeID(SSN);
            if (objE != null)
            {
                long userid = Convert.ToInt64(objE.EmployeeID);
                CompanyManagement obj = new CompanyManagement();
                if (userid != 0)
                {

                    objb = new UserManagement().GetBasicInfo(userid);
                    objp = new UserManagement().GetUserPersonalInfo(userid);
                    if (objb.IsPassword == true)
                    {
                        ViewBag.MessageType = "error";
                        ViewBag.Message = "You have already set the password.";

                    }
                    else
                    {
                        if ((string.IsNullOrEmpty(objb.Email)) && (string.IsNullOrEmpty(objp.Phone)))
                        {
                            var duplicatecheckEmail = new tblCompany_Employee_BasicInfo();
                            duplicatecheckEmail = null;
                            var duplicatecheckPhone = new tblCompany_Employee_PersonalInfo();
                            duplicatecheckPhone = null;
                            if (!string.IsNullOrEmpty(Email))
                            {
                                duplicatecheckEmail = new UserManagement().GetUserInfo(Email);
                            }
                            if (!string.IsNullOrEmpty(Phone))
                            {
                                duplicatecheckPhone = new UserManagement().GetUserInfoP(Phone);
                            }

                            if (duplicatecheckEmail == null && duplicatecheckPhone == null)
                            {
                                objb.IsPassword = true;
                                objb.ID = Convert.ToInt64(userid);
                                objb.Password = Helper.Encrypt(resetPassword);
                                objb.Email = Email;
                                objp.Phone = Phone;
                                objp.EmployeeID = Convert.ToInt64(userid);

                                int a = obj.UpdateSSNWithPassord(objb, objp);
                                if (a == 1)
                                {
                                    ViewBag.MessageType = "success";
                                    ViewBag.Message = "Password successfully updated.";

                                    int statusPhone = 0;
                                    if (!string.IsNullOrEmpty(Phone))
                                    {
                                        // Date21Sept as per client Discussion code comment
                                        statusPhone = SendTwilloMessage.SendmessageTwillo(Phone, "Hello " + objb.FirstName + ", \n Welcome to Enroll My Group! \n kindly use your credentials to login.\n \n Thanks, \n Enroll My Group Team");
                                    }
                                    if (!string.IsNullOrEmpty(Email))
                                    {

                                        string result = string.Empty;
                                        string baseurl = Helper.GetBaseUrl();
                                        string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/PasswordSSN.html"));  // email template
                                        Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                                        Mailbody = Mailbody.Replace("###UserName###", objb.FirstName);
                                        if (!string.IsNullOrEmpty(Email))
                                        {
                                            // Date21Sept as per client Discussion code comment
                                            result = Helper.SendEmail(Email, Mailbody, "Welcome to EnrollMyGroup");
                                            result = "Success";
                                        }

                                    }
                                }
                            }
                            else
                            {
                                ViewBag.MessageType = "error";
                                ViewBag.Message = "Email or Mobile Phone already exists. Please try another.";
                            }

                        }
                        else
                        {

                            ViewBag.MessageType = "error";
                            ViewBag.Message = "Email or Mobile Phone already exists.";
                        }
                    }
                }

            }
            else
            {
                ViewBag.MessageType = "error";
                ViewBag.Message = "Invalid SSN";
            }



            return View();
        }
        class YourModelClass1
        {
            public int Id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string position { get; set; }
            public string salary { get; set; }
            public string start_date { get; set; }
            public string office { get; set; }

        }

    }
}
