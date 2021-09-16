using PowerIBroker.Models;
using System;
using System.Web.Mvc;
using PowerIBrokerBusinessLayer.UserLogin;
using PowerIBrokerDataLayer;
using PowerIBrokerBusinessLayer;
using Newtonsoft.Json;
using System.Globalization;
using System.Web;
using PowerIBrokerBusinessLayer.Employee;

namespace PowerIBroker.Controllers
{
    public class UserLoginController : Controller
    {
        UserLoginManagement objuser = new UserLoginManagement();
        UserAuthentication objUAuth = new UserAuthentication();

        //
        // GET: /Login/

        public ActionResult Index()
        {

            UserAuthentication obj = new UserAuthentication();
            HttpCookie myCookie = Request.Cookies["UserEmail"];
            if (myCookie != null)
            {
                obj.Email = myCookie.Value;
                HttpCookie myCookie1 = Request.Cookies["UserPassword"];
                if (myCookie1 != null)
                {
                    obj.Password = myCookie1.Value;
                }
                obj.RememberMe = true;
            }

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Index(UserAuthentication obj)
        {
            string ReturnUrl = Convert.ToString(Request.Form["hdnReturnUrl"]);
            dynamic UserInfo = null;
            try
            {
                string strUserEmail, strpassword;
                bool RememberMe;


                strUserEmail = obj.Email.ToString();
                strpassword = obj.Password.ToString();
                RememberMe = obj.RememberMe;

                if (obj != null)
                {
                    Administration objAdmin = new Administration();
                    var userTypeBroker = objAdmin.CheckUserExistenceForAdmin(strUserEmail);
                    var userTypeGeneralAgent = objAdmin.CheckGAExistenceForAdmin(strUserEmail);
                    if (userTypeBroker != null)
                    {
                        if (userTypeBroker.EntityType == UserLoginType.Broker)
                        {
                            var adminInfo = objAdmin.BrokerLogin(obj.Email, Helper.Encrypt(obj.Password));
                            if (adminInfo == null)
                            {
                                TempData["Message"] = "Invalid login credentials, please try again.";
                            }
                            else
                            {
                                if (adminInfo.CompanyStatus == true)
                                {

                                    if (adminInfo.IsActive == true)
                                    {
                                        Session["CultureName"] = obj.CultureName == null ? "en-US" : obj.CultureName;
                                        Session["CultureSymbol"] = System.Globalization.CultureInfo.GetCultureInfo(Convert.ToString(Session["CultureName"])).NumberFormat.CurrencySymbol;
                                        Session["CreatedBy"] = adminInfo.SubBrokerId;
                                        Session["SubBrokerId"] = adminInfo.SubBrokerId;
                                        Session["BrokerId"] = adminInfo.BrokerId;
                                        Session["Email"] = adminInfo.Email;
                                        Session["EntityType"] = userTypeBroker.EntityType;
                                        Session["Name"] = adminInfo.FirstName + " " + adminInfo.LastName;
                                        if(adminInfo.AdministratorType == 1)
                                        Session["BrokerUser"] = adminInfo.FirstName + " " + adminInfo.LastName;
                                        else
                                        Session["BrokerAdmin"] = adminInfo.FirstName + " " + adminInfo.LastName;
                                        if ((!string.IsNullOrEmpty(ReturnUrl))&& ReturnUrl.ToLower().Contains("broker-admin"))
                                        {
                                            return Redirect(ReturnUrl);
                                        }
                                        
                                        else
                                        {
                                            return RedirectToAction("Index", adminInfo.AdministratorType==1?"BrokerUser": "Broker-Admin");
                                        }
                                        //return RedirectToAction("ManageCompany", "Broker");
                                    }
                                    else
                                    {
                                        TempData["Message"] = "User is suspended. Please contact to administrator.";

                                    }
                                }
                                else
                                {
                                    TempData["Message"] = "Broker company is suspended. Please contact to administrator.";
                                }

                            }
                        }
                        else
                        {
                            if (userTypeBroker.EntityType == UserLoginType.Agent)
                            {
                                TempData["Message"] = "Invalid login credentials, please try again.";

                            }
                        }

                    }
                    else if (userTypeGeneralAgent != null)
                    {
                        if (userTypeGeneralAgent.EntityType == UserLoginType.GeneralAgent)
                        {
                            var adminInfo = objAdmin.GeneralAgentLogin(obj.Email, Helper.Encrypt(obj.Password));
                            if (adminInfo == null)
                            {
                                TempData["Message"] = "Invalid login credentials, please try again.";
                            }
                            else
                            {
                                if (adminInfo.CompanyStatus == true)
                                {

                                    if (adminInfo.IsActive == true)
                                    {
                                        Session["CultureName"] = obj.CultureName == null ? "en-US" : obj.CultureName;
                                        Session["CultureSymbol"] = System.Globalization.CultureInfo.GetCultureInfo(Convert.ToString(Session["CultureName"])).NumberFormat.CurrencySymbol;
                                        Session["CreatedBy"] = adminInfo.Id;
                                        Session["GeneralAgentId"] = adminInfo.Id;
                                        Session["Email"] = adminInfo.Email;
                                        Session["Gen_BrokerId"] = userTypeGeneralAgent.BrokerId;
                                        Session["EntityType"] = userTypeGeneralAgent.EntityType;
                                        Session["Name"] = adminInfo.FirstName + " " + adminInfo.LastName;
                                        Session["GeneralAgent"] = adminInfo.FirstName + " " + adminInfo.LastName;
                                        if ((!string.IsNullOrEmpty(ReturnUrl)) && ReturnUrl.ToLower().Contains("generalagent"))
                                        {
                                            return Redirect(ReturnUrl);
                                        }
                                        else
                                        {
                                            return RedirectToAction("Index", "GeneralAgent");
                                        }
                                        //return RedirectToAction("ManageCompany", "Broker");
                                    }
                                    else
                                    {
                                        TempData["Message"] = "User is suspended. Please contact to administrator.";

                                    }
                                }
                                else
                                {
                                    TempData["Message"] = "Broker company is suspended. Please contact to administrator.";
                                }

                            }
                        }
                        else
                        {
                            if (userTypeBroker.EntityType == UserLoginType.Agent)
                            {
                                TempData["Message"] = "Invalid login credentials, please try again.";

                            }
                        }

                    }
                    else
                    {

                        var userType = objuser.CheckUserExistence(strUserEmail, "", 0, 1);
                        var configInfo = objuser.GetSystemConfig();
                        if (userType != null)
                        {
                            //It will check the email and password
                            UserInfo = objuser.EmployeeLogin(obj.Email, Helper.Encrypt(obj.Password), 1);//Now company and employee login will be the same table.
                            if (UserInfo == null)
                            {
                                //It will check the Termainate and archieve
                                UserInfo = objuser.EmployeeLogin(obj.Email, Helper.Encrypt(obj.Password), 0);
                                if (UserInfo == null)
                                {
                                    tblCompany_Employee_BasicInfo data = objuser.EmployeeLoginAttempts(obj.Email);
                                    if (data.IsPassword == false)
                                    {
                                        TempData["Message"] = "Please check your welcome email to set your password sent by the company on the enrollment for the first time login.";
                                        return RedirectToAction("Login", "Home");
                                    }
                                    else
                                    {
                                        if (data.NoOfAttempts == null)
                                        {
                                            data.NoOfAttempts = 0;
                                        }
                                        int? attempts = (data == null ? 0 : (data.NoOfAttempts == null ? 0 : data.NoOfAttempts)) + 1;
                                        int noofattampts = objuser.SaveNoofAttampts(data.ID, attempts, 0, configInfo.NoOfAttempts);
                                        if (noofattampts == 2)
                                        {
                                            TempData["Message"] = "Your Account has been blocked, due to entered wrong password " + attempts + " times.";
                                            TempData["MessageAttempts"] = attempts;
                                            return RedirectToAction("Login", "Home");
                                        }
                                        else
                                        {
                                            int? value = configInfo.NoOfAttempts - attempts;
                                            TempData["Message"] = "Invalid Email or Phone or Password. You are left with " + value + " more attempt.";
                                            TempData["MessageLeft"] = value;
                                            return RedirectToAction("Login", "Home");
                                        }
                                    }
                                }
                                else
                                {
                                    tblCompany_Employee_BasicInfo data = objuser.EmployeeLoginAttempts(obj.Email);
                                    if (data.IsPassword == false)
                                    {
                                        TempData["Message"] = "Please check your welcome email to set your password sent by the company on the enrollment for the first time login.";
                                        return RedirectToAction("Login", "Home");
                                    }

                                    else
                                    {
                                        TimeSpan? time = UserInfo.DeactivateTime - DateTime.UtcNow;
                                        if (time.Value.TotalMinutes > configInfo.DeactivateHours)
                                        {
                                            int noofattampts = objuser.SaveEmpNoofAttampts(UserInfo.ID, 0, 1, configInfo.NoOfAttempts);
                                            UserInfo = objuser.EmployeeLogin(obj.Email, Helper.Encrypt(obj.Password), 0);
                                        }
                                        else
                                        {
                                            TimeSpan? time1 = DateTime.UtcNow - data.DeactivateTime;


                                            if (time1.Value.TotalMinutes >= configInfo.DeactivateHours)
                                            {
                                                int noofattampts = objuser.SaveEmpNoofAttampts(UserInfo.ID, 0, 1, configInfo.NoOfAttempts);
                                                if (userType.EntityType == UserLoginType.Employee)
                                                {
                                                    Session["EmployeeId"] = UserInfo.ID;
                                                    Session["EmployeeCompanyId"] = UserInfo.CompanyId;
                                                    Session["EmployeeEmail"] = UserInfo.Email;

                                                    if (UserInfo.IsLogin == 0)
                                                    {
                                                        int result = objuser.updateFirstLogin(UserInfo.Email);
                                                        return RedirectToAction("Index", "Basicinfo", new { area = "Employee" });
                                                    }
                                                    else
                                                    {
                                              

                                                        OpenEnrollment OpenEnroll = new OpenEnrollment();
                                                        var compnyID = OpenEnroll.GetCompanyID(Convert.ToInt64(Session["EmployeeId"]));
                                                        var OpenEnrollData = OpenEnroll.GetOpenEnrollMent(Convert.ToInt64(Session["EmployeeId"]), compnyID);
                                                        if (OpenEnrollData.Count > 0)
                                                        {
                                                            Session["LandingOESession"] = "Yes";
                                                            return RedirectToAction("EmpLanding", "Dashboard", new { area = "Employee" });
                                                        }
                                                        else
                                                        {
                                                            Session["LandingOESession"] = "No";
                                                            return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
                                                        }
                                                        //  return RedirectToAction("Index", "Dashboard", new { area = "Employee" });




                                                    }


                                                }
                                                if (userType.EntityType == UserLoginType.Company)
                                                {
                                                    Session["CompanyId"] = UserInfo.CompanyId;//Employee ID
                                                    Session["ComEmployeeId"] = UserInfo.ID;//Employee ID
                                                    Session["CompanyEmail"] = UserInfo.Email;
                                                    Session["CompanyName"] = UserInfo.Email;
                                                    //if (UserInfo.AdministratorType == 1)
                                                    //{
                                                    //    Session["HRSpecialist"] = "1";
                                                    //}

                                                    return RedirectToAction("Index", "Dashboard", new { area = "Company" });

                                                }

                                            }
                                            else
                                            {

                                                TempData["Message"] = "The incorrect password has been entered " + data.NoOfAttempts + " times Your account has been locked for " + configInfo.DeactivateHours + " minutes Optionally, you can reset your password.";
                                                TempData["MessageDeactivateHours"] = configInfo.DeactivateHours;
                                                TempData["MessageAttempts"] = data.NoOfAttempts;
                                            }
                                            return RedirectToAction("Login", "Home");
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if ((userType.EntityType == UserLoginType.Employee) && (UserInfo.IsTerminate == true || UserInfo.IsArchive == true || UserInfo.IsActive == false))
                                {

                                    TempData["Message"] = "Your account has been deactivated. Please contact to admin.";
                                    return RedirectToAction("Login", "Home");
                                }
                                CompanyMaster data = objuser.CompanyStatus(Convert.ToInt64(userType.CompanyId));

                                if (data.IsActive == false && userType.EntityType == UserLoginType.Employee)
                                {
                                    TempData["Message"] = "Company is deactivated. Please contact to admin.";
                                    return RedirectToAction("Login", "Home");
                                }
                                if (data.IsActive == false && userType.EntityType == UserLoginType.Company)
                                {
                                    TempData["Message"] = "Your account has been deactivated. Please contact to admin.";
                                    return RedirectToAction("Login", "Home");
                                }

                            }


                            if ((UserInfo.IsTerminate == true || UserInfo.IsArchive == true || UserInfo.IsActive == false))
                            {
                                TempData["Message"] = "Your account has been deactivated. Please contact to admin.";
                                return RedirectToAction("Login", "Home");
                            }
                            else if (UserInfo != null)
                            {
                                if (RememberMe != null)
                                {
                                    if ((bool)RememberMe)
                                    {
                                        HttpCookie myCookie = new HttpCookie("UserEmail");
                                        myCookie.Value = obj.Email;
                                        myCookie.Expires = DateTime.UtcNow.AddDays(30);
                                        Response.Cookies.Add(myCookie);
                                        HttpCookie myCookie1 = new HttpCookie("UserPassword");
                                        myCookie1.Value = obj.Password;
                                        myCookie1.Expires = DateTime.UtcNow.AddDays(30);
                                        Response.Cookies.Add(myCookie1);
                                    }
                                    else
                                    {
                                        Response.Cookies["UserEmail"].Expires = DateTime.UtcNow.AddDays(-1);
                                        Response.Cookies["UserPassword"].Expires = DateTime.UtcNow.AddDays(-1);
                                    }
                                }


                                Session["CultureName"] = obj.CultureName == null ? "en-US" : obj.CultureName;
                                Session["CultureSymbol"] = System.Globalization.CultureInfo.GetCultureInfo(Convert.ToString(Session["CultureName"])).NumberFormat.CurrencySymbol;

                                if (userType.EntityType == UserLoginType.Employee)
                                {

                                    if ((Session["AdminID"] == null || Convert.ToInt32(Session["AdminID"]) == 1) && Session["ComAdminID"] == null)
                                    {

                                        Session["EmployeeId"] = UserInfo.ID;
                                        Session["EmployeeCompanyId"] = UserInfo.CompanyId;
                                        Session["EmployeeEmail"] = UserInfo.Email;
                                        long EmployeeId = Convert.ToInt64(Session["EmployeeId"]);
                                        int noofattampts = objuser.SaveEmpNoofAttampts(UserInfo.ID, 0, 1, configInfo.NoOfAttempts);

                                        if ((!string.IsNullOrEmpty(ReturnUrl)))
                                        {

                                            return Redirect(ReturnUrl);
                                        }
                                        else
                                        {
                                            if (UserInfo.IsLogin == 0)
                                            {
                                                int result = objuser.updateFirstLogin(UserInfo.Email);
                                                return RedirectToAction("Index", "Basicinfo", new { area = "Employee" });
                                            }
                                            else
                                            {
                                                OpenEnrollment OpenEnroll = new OpenEnrollment();
                                                var compnyID = OpenEnroll.GetCompanyID(Convert.ToInt64(Session["EmployeeId"]));
                                                var OpenEnrollData = OpenEnroll.GetOpenEnrollMent(Convert.ToInt64(Session["EmployeeId"]), compnyID);
                                                if (OpenEnrollData.Count > 0)
                                                {
                                                    Session["LandingOESession"] = "Yes";
                                                    return RedirectToAction("EmpLanding", "Dashboard", new { area = "Employee" });
                                                }
                                                else
                                                {
                                                    Session["LandingOESession"] = "No";
                                                    return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
                                                }



                                            }

                                            //return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
                                        }
                                    }
                                    else
                                    {
                                        TempData["Message"] = "You can not log in as another admin/user already logged in.";
                                        TempData["UserType"] = obj.UserType;
                                    }

                                }


                                if (userType.EntityType == UserLoginType.Company)
                                {

                                    if (Session["AdminID"] == null || Convert.ToInt32(Session["AdminID"]) == 1)
                                    {

                                        Session["CompanyId"] = UserInfo.CompanyId;//Employee ID
                                        Session["ComEmployeeId"] = UserInfo.ID;//Employee ID
                                        Session["CompanyEmail"] = UserInfo.Email;
                                        Session["CompanyName"] = UserInfo.Email;
                                        //if (UserInfo.AdministratorType == 1)
                                        //{
                                        //    Session["HRSpecialist"] = "1";
                                        //}
                                        int noofattampts = objuser.SaveNoofAttampts(UserInfo.ID, 0, 1, configInfo.NoOfAttempts);
                                        //return RedirectToAction("Index", "Dashboard", new { area = "Company" });


                                        if ((!string.IsNullOrEmpty(ReturnUrl)))
                                        {

                                            return Redirect(ReturnUrl);
                                        }
                                        else
                                        {
                                            return RedirectToAction("Index", "Dashboard", new { area = "Company" });
                                        }
                                    }
                                    else
                                    {
                                        TempData["Message"] = "You can not log in as another admin/user already logged in.";
                                        TempData["UserType"] = obj.UserType;
                                    }
                                }

                            }
                            else
                            {
                                TempData["Message"] = "Invalid login credentials, please try again.";
                                TempData["UserType"] = obj.UserType;
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Invalid login credentials, please try again.";
                            TempData["UserType"] = obj.UserType;
                        }

                    }


                }
                else
                {
                    TempData["Message"] = "Invalid login credentials, please try again.";
                    TempData["UserType"] = obj.UserType;
                }


            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;//"Some error occurred while processing the request. Please try again";
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return RedirectToAction("Login", "Home");

        }
        public static bool ValidateCaptcha1(string response)
        {
            //secret that was generated in key value pair  
            //string secret = "6LcqLygTAAAAAMslwMubBSFQS2GCrbXvUGWZ5OjC";
            string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new System.Net.WebClient();
            var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<UserAuthentication>(reply);

            return Convert.ToBoolean(captchaResponse.Success);

        }

        [HttpPost]
        public ActionResult ValidateCaptcha(string response)
        {
            // var response = Request["g-recaptcha-response"];
            //secret that was generated in key value pair
            string secret = System.Configuration.ConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new System.Net.WebClient();
            var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<UserAuthentication>(reply);

            //when response is false check for the error message
            if (!captchaResponse.Success)
            {
                return Json(captchaResponse, JsonRequestBehavior.AllowGet);

                //if (captchaResponse.ErrorMessage.Count <= 0) return View();

                //var error = captchaResponse.ErrorMessage[0].ToLower();
                //switch (error)
                //{
                //    case ("missing-input-secret"):
                //        ViewBag.Message = "The secret parameter is missing.";
                //        break;
                //    case ("invalid-input-secret"):
                //        ViewBag.Message = "The secret parameter is invalid or malformed.";
                //        break;

                //    case ("missing-input-response"):
                //        ViewBag.Message = "The response parameter is missing.";
                //        break;
                //    case ("invalid-input-response"):
                //        ViewBag.Message = "The response parameter is invalid or malformed.";
                //        break;

                //    default:
                //        ViewBag.Message = "Error occured. Please try again";
                //        break;
                //}
            }
            else
            {
                ViewBag.Message = "Valid";
                return Json(captchaResponse, JsonRequestBehavior.AllowGet);

            }

            //return View();
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        public string GeneratePasswordResetRequest(string email)
        {
            UserLoginManagement ulm = new UserLoginManagement();
            Guid UniqueID = Guid.NewGuid();
            string verifier = UniqueID.ToString() + "-" + DateTime.UtcNow.Ticks.ToString();
            UserPasswordResetInfo uprObj = new UserPasswordResetInfo
            {
                Email = email,
                IsActive = true,
                Verifier = verifier
            };
            return ulm.GeneratePasswordResetRequest(uprObj).ToString();

        }
    }
}
