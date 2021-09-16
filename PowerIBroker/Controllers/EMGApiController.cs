using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PowerIBroker.Models;
using System.Web;
using PowerIBrokerBusinessLayer.UserLogin;
using PowerIBrokerDataLayer;
using PowerIBrokerBusinessLayer;
using PowerIBrokerBusinessLayer.Employee;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Data.Common;
using System.IO;
using PowerIBrokerBusinessLayer.Company;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using PowerIBroker.Areas.Employee.Models;
namespace PowerIBroker.Controllers
{
    public class EMGApiController : ApiController
    {
        #region Assemblies
        UserLoginManagement objuser = new UserLoginManagement();
        UserAuthentication objUAuth = new UserAuthentication();
        Status objStatus = new Status();
        ErrorLog ObjElog = new ErrorLog();
        EmployeeInfo objemployee = new EmployeeInfo();
        Comman objComman = new Comman();
        GetDropDownValues Objddl = new GetDropDownValues();
        PowerIBrokerBusinessLayer.Employee.OpenEnrollment OpenEnroll = new PowerIBrokerBusinessLayer.Employee.OpenEnrollment();
        #endregion
        #region Customer registration information
        [HttpPost]
        public HttpResponseMessage UserLogin(UserAuthentication obj)
        {
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
                    var userType = objuser.CheckUserExistence(strUserEmail, "", 0, 1);
                    var configInfo = objuser.GetSystemConfig();
                    if (userType != null)
                    {
                        UserInfo = objuser.ApiEmployeeLogin(obj.Email, Helper.Encrypt(obj.Password), 1);
                        if (UserInfo == null)
                        {
                            UserInfo = objuser.ApiEmployeeLogin(obj.Email, Helper.Encrypt(obj.Password), 0);
                            if (UserInfo == null)
                            {
                                tblCompany_Employee_BasicInfo data = objuser.EmployeeLoginAttempts(obj.Email);
                                if (data.IsPassword == false)
                                {
                                    objStatus.Message = "Please check your welcome email to set your password sent by the company on the enrollment for the first time login.";
                                    objStatus.MessageStatus = false;
                                    return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                                }
                                else
                                {
                                    if (data.NoOfAttempts == null)
                                    {
                                        data.NoOfAttempts = 0;
                                    }
                                    int? attempts = data.NoOfAttempts + 1;
                                    int noofattampts = objuser.SaveEmpNoofAttampts(data.ID, attempts, 0, configInfo.NoOfAttempts);
                                    if (noofattampts == 2)
                                    {
                                        objStatus.Message = "Your Account has been blocked, due to entered wrong password " + attempts + " times.";
                                        objStatus.MessageStatus = false;
                                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                                    }
                                    else
                                    {
                                        int? value = configInfo.NoOfAttempts - attempts;
                                        objStatus.Message = "Invalid Email or Phone or Password. You are left with " + value + " more attempt.";
                                        objStatus.MessageStatus = false;
                                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                                    }

                                }
                            }

                            else
                            {

                                tblCompany_Employee_BasicInfo data = objuser.EmployeeLoginAttempts(obj.Email);
                                if (data.IsPassword == false)
                                {
                                    objStatus.Message = "Please check your welcome email to set your password send by the company on the enrollment for the first time login.";
                                    objStatus.MessageStatus = false;
                                    return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                                }

                                TimeSpan? time = UserInfo.DeactivateTime - DateTime.UtcNow;
                                if (time.Value.TotalHours > configInfo.DeactivateHours)
                                {
                                    int noofattampts = objuser.SaveEmpNoofAttampts(UserInfo.ID, 0, 1, configInfo.NoOfAttempts);
                                    UserInfo = objuser.ApiEmployeeLogin(obj.Email, Helper.Encrypt(obj.Password), 0);
                                }
                                else
                                {
                                    // objStatus.Message = "You can login after " + configInfo.DeactivateHours + " minutes of blocking an account.";
                                    objStatus.Message = "The incorrect password has been entered " + data.NoOfAttempts + " times Your account has been locked for " + configInfo.DeactivateHours + " minutes Optionally, you can reset your password.";
                                    objStatus.MessageStatus = false;
                                    return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                                }
                            }
                        }
                        else
                        {
                            if ((userType.EntityType == UserLoginType.Employee) && (UserInfo.IsTerminate == true || UserInfo.IsArchive == true || UserInfo.IsActive == false))
                            {

                                objStatus.Message = "Your account has been deactivated. Please contact to admin.";
                                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                            }

                        }

                        if (UserInfo != null)
                        {
                            objStatus.Message = "User Login";
                            objStatus.MessageStatus = true;
                            objStatus.DataList = UserInfo;
                            int noofattampts = objuser.SaveEmpNoofAttampts(UserInfo.ID, 0, 1, configInfo.NoOfAttempts);
                            string baseurl = Helper.GetBaseUrl();
                            //objStatus.BenefitUrl = baseurl + "/Employee/HSA?EmpId=" + UserInfo.ID + "";

                            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                        }
                        else
                        {
                            objStatus.Message = "Invalid login credentials, please try again";
                            objStatus.MessageStatus = false;
                            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                        }
                    }
                    else
                    {
                        objStatus.Message = "Invalid login credentials, please try again";
                        objStatus.MessageStatus = false;
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                    }

                }
                else
                {
                    objStatus.Message = "Invalid login credentials, please try again";
                    objStatus.MessageStatus = false;
                    return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                }

            }
            catch (Exception ex)
            {
                objStatus.Message = "Some error occurred while processing the request. Please try again";
                objStatus.MessageStatus = false;
                objStatus.Message = ex.Message;
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);

        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage SendNotification(string MessageType)
        {
            AndroidFCMPushNotificationStatus result = new AndroidFCMPushNotificationStatus();
            DateTime todayDate = DateTime.Now;
            todayDate = new DateTime(todayDate.Year, todayDate.Month, todayDate.Day, 0, 0, 0);
            try
            {
                result.Successful = false;
                result.Error = null;
                string serverApiKey = System.Configuration.ConfigurationManager.AppSettings["GoogleAppID"].ToString();
                string senderId = System.Configuration.ConfigurationManager.AppSettings["SENDER_ID"].ToString();

                #region OpenEnrollment

                if (MessageType == "OpenEnrollment")
                {
                    var empList = OpenEnroll.GetPushNotificationEmployees();
                    if (empList.Count > 0)
                    {
                        foreach (var item in empList)
                        {
                            if (!string.IsNullOrEmpty(item.DeviceTokenId))
                            {
                                var pushNotOpenEnroll = OpenEnroll.GetPushNotificationOpenEnrollment(item.EmployeeID);
                                if (pushNotOpenEnroll.Count > 0)
                                {


                                    #region Android
                                    if (item.DeviceType == "Android")
                                    {
                                        foreach (var pushEnroll in pushNotOpenEnroll)
                                        {
                                            #region Every 5 day

                                            if (todayDate.AddDays(4) <= Convert.ToDateTime(pushEnroll.EndDate))
                                            {
                                                int day = (pushEnroll.StartDate.Value.Day);
                                                //DateTime fakeDate = pushEnroll.StartDate.Value;
                                                //DateTime? addedDate = null;
                                                CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
                                                //GetFirstDayOfWeek(pushEnroll.StartDate.Value,pushEnroll.EndDate.Value, defaultCultureInfo);
                                                if (GetFirstDayOfWeek(pushEnroll.StartDate.Value, pushEnroll.EndDate.Value, defaultCultureInfo) == todayDate)
                                                {
                                                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                                                    tRequest.Method = "post";
                                                    tRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                                                    tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
                                                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                                                    //var value = "Please complete your enrollment " + pushEnroll.Description + ". Your  enrollment last date is " + string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(pushEnroll.EndDate.Value));
                                                    var value = "Please complete your open enrollment. The last date for open enrollment is " + string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(pushEnroll.EndDate.Value));
                                                    string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + item.DeviceTokenId + "&messge_type=" + "OpenEnrollment" + "";
                                                    Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                                                    tRequest.ContentLength = byteArray.Length;

                                                    using (Stream dataStream = tRequest.GetRequestStream())
                                                    {
                                                        dataStream.Write(byteArray, 0, byteArray.Length);

                                                        using (WebResponse tResponse = tRequest.GetResponse())
                                                        {
                                                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                                            {
                                                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                                                {
                                                                    String sResponseFromServer = tReader.ReadToEnd();
                                                                    result.Response = sResponseFromServer;
                                                                    result.Successful = true;
                                                                    Notification saveNot = new Notification();
                                                                    saveNot.EmployeeID = item.EmployeeID;
                                                                    saveNot.Description = value;
                                                                    saveNot.MessageType = "OpenEnrollment";
                                                                    saveNot.IsActive = true;
                                                                    saveNot.IsRead = false;
                                                                    saveNot.CreatedDate = todayDate;
                                                                    int saveresult = OpenEnroll.SaveNotification(saveNot);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                            #endregion
                                            #region last 1 day
                                            else
                                            {
                                                if (Convert.ToDateTime(pushEnroll.EndDate).AddDays(-1) == todayDate)
                                                {
                                                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                                                    tRequest.Method = "post";
                                                    tRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                                                    tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
                                                    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                                                    var value = "Please complete your open enrollment. The last date for open enrollment is " + string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(pushEnroll.EndDate.Value));
                                                    //var value = "Please complete your enrollment " + pushEnroll.Description + ",You have only one day left.";
                                                    string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + item.DeviceTokenId + "&messge_type=" + "OpenEnrollment" + "";
                                                    Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                                                    tRequest.ContentLength = byteArray.Length;

                                                    using (Stream dataStream = tRequest.GetRequestStream())
                                                    {
                                                        dataStream.Write(byteArray, 0, byteArray.Length);

                                                        using (WebResponse tResponse = tRequest.GetResponse())
                                                        {
                                                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                                            {
                                                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                                                {
                                                                    String sResponseFromServer = tReader.ReadToEnd();
                                                                    result.Response = sResponseFromServer;
                                                                    result.Successful = true;
                                                                    Notification saveNot = new Notification();
                                                                    saveNot.EmployeeID = item.EmployeeID;
                                                                    saveNot.Description = value;
                                                                    saveNot.MessageType = "OpenEnrollment";
                                                                    saveNot.IsActive = true;
                                                                    saveNot.IsRead = false;
                                                                    saveNot.CreatedDate = todayDate;
                                                                    int saveresult = OpenEnroll.SaveNotification(saveNot);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion

                                    #region IOS
                                    if (item.DeviceType == "IOS")
                                    {

                                        //string certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("/includes/IphoneNotificationFile/Customerpushcert.p12");
                                        //var payloads = new NotificationPayload("836c07dbf32b4b310122daeadd540ee9f1e636ff26ff4bc4b6f9478d875fbcb7", "test", 1, "default", "item.Type");
                                        //string certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("/includes/IphoneNotificationFile/Certificates.p12");
                                        //var payloads = new NotificationPayload("B872FD28129962017B2DE9642412C8FFF8B61EE7ABD879853C92143FEAA5A5DA", "test", 1, "default", "item.Type");
                                        //var p = new List<NotificationPayload> { payloads };
                                        //var push = new ApplePushNotification(true, certificatePath, "");
                                        //var rejected = push.SendToApple(p);
                                        //string result1 = "";
                                        //foreach (var items in rejected)
                                        //{
                                        //    result1 = items;
                                        //}
                                        foreach (var pushEnroll in pushNotOpenEnroll)
                                        {
                                            #region Every 5 day

                                            if (todayDate.AddDays(1) <= Convert.ToDateTime(pushEnroll.EndDate))
                                            {
                                                int day = (pushEnroll.StartDate.Value.Day);
                                                CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
                                                if (GetFirstDayOfWeek(pushEnroll.StartDate.Value, pushEnroll.EndDate.Value, defaultCultureInfo) == todayDate)
                                                {
                                                    //var value = "Please complete your enrollment " + pushEnroll.Description + ". Your  enrollment last date is " + string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(pushEnroll.EndDate.Value));
                                                    var value = "Please complete your open enrollment. The last date for open enrollment is " + string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(pushEnroll.EndDate.Value));
                                                    string certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("/includes/IphoneNotificationFile/Certificates.p12");
                                                    var payloads = new NotificationPayload(item.DeviceTokenId, value, 1, "default", item.DeviceType);
                                                    var p = new List<NotificationPayload> { payloads };
                                                    var push = new ApplePushNotification(true, certificatePath, "");
                                                    var rejected = push.SendToApple(p);

                                                    result.Successful = true;
                                                    Notification saveNot = new Notification();
                                                    saveNot.EmployeeID = item.EmployeeID;
                                                    saveNot.Description = value;
                                                    saveNot.MessageType = "OpenEnrollment";
                                                    saveNot.IsActive = true;
                                                    saveNot.IsRead = false;
                                                    saveNot.CreatedDate = todayDate;
                                                    int saveresult = OpenEnroll.SaveNotification(saveNot);

                                                }

                                            }
                                            #endregion
                                            #region last 1 day
                                            else
                                            {
                                                if (Convert.ToDateTime(pushEnroll.EndDate).AddDays(-1) == todayDate)
                                                {

                                                    //var value = "Please complete your enrollment " + pushEnroll.Description + ",You have only one day left.";
                                                    var value = "Please complete your open enrollment. The last date for open enrollment is " + string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(pushEnroll.EndDate.Value));
                                                    string certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("/includes/IphoneNotificationFile/Certificates.p12");
                                                    var payloads = new NotificationPayload(item.DeviceTokenId, value, 1, "default", item.DeviceType);
                                                    var p = new List<NotificationPayload> { payloads };
                                                    var push = new ApplePushNotification(true, certificatePath, "");
                                                    var rejected = push.SendToApple(p);

                                                    result.Successful = true;
                                                    Notification saveNot = new Notification();
                                                    saveNot.EmployeeID = item.EmployeeID;
                                                    saveNot.Description = value;
                                                    saveNot.MessageType = "OpenEnrollment";
                                                    saveNot.IsActive = true;
                                                    saveNot.IsRead = false;
                                                    saveNot.CreatedDate = todayDate;
                                                    int saveresult = OpenEnroll.SaveNotification(saveNot);


                                                }

                                            }
                                            #endregion
                                        }

                                    }
                                    #endregion


                                }
                            }
                        }
                    }

                }
                #endregion

                #region Birthday
                if (MessageType == "Birthday")
                {
                    var value = "Happy birthday,On this special day, May gladness fill your every hour,with joy to light your way.";
                    var empDetails = OpenEnroll.GetAllEmployeesDOB();
                    if (empDetails.Count > 0)
                    {
                        foreach (var dob in empDetails)
                        {
                            if (!string.IsNullOrEmpty(dob.DOB))
                            {
                                if (Convert.ToDateTime(dob.DOB).ToString("MM/dd") == DateTime.Now.Date.ToString("MM/dd"))
                                {
                                    if (dob.DeviceType == "Android")
                                    {
                                        WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                                        tRequest.Method = "post";
                                        tRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                                        tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
                                        tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                                        string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + dob.DeviceTokenId + "&messge_type=" + "Birthday" + "";
                                        Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                                        tRequest.ContentLength = byteArray.Length;

                                        using (Stream dataStream = tRequest.GetRequestStream())
                                        {
                                            dataStream.Write(byteArray, 0, byteArray.Length);

                                            using (WebResponse tResponse = tRequest.GetResponse())
                                            {
                                                using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                                {
                                                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                                    {
                                                        String sResponseFromServer = tReader.ReadToEnd();
                                                        result.Response = sResponseFromServer;
                                                        result.Successful = true;
                                                        Notification saveNot = new Notification();
                                                        saveNot.EmployeeID = dob.ID;
                                                        saveNot.Description = value;
                                                        saveNot.MessageType = "Birthday";
                                                        saveNot.IsActive = true;
                                                        saveNot.IsRead = false;
                                                        saveNot.CreatedDate = todayDate;
                                                        int saveresult = OpenEnroll.SaveNotification(saveNot);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (dob.DeviceType == "IOS")
                                    {
                                        string certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("/includes/IphoneNotificationFile/Certificates.p12");
                                        var payloads = new NotificationPayload(dob.DeviceTokenId, value, 1, "default", dob.DeviceType);
                                        var p = new List<NotificationPayload> { payloads };
                                        var push = new ApplePushNotification(true, certificatePath, "");
                                        var rejected = push.SendToApple(p);

                                        result.Successful = true;
                                        Notification saveNot = new Notification();
                                        saveNot.EmployeeID = dob.ID;
                                        saveNot.Description = value;
                                        saveNot.MessageType = "Birthday";
                                        saveNot.IsActive = true;
                                        saveNot.IsRead = false;
                                        saveNot.CreatedDate = todayDate;
                                        int saveresult = OpenEnroll.SaveNotification(saveNot);

                                    }
                                }


                            }
                        }
                    }

                }
                #endregion

            }
            catch (Exception ex)
            {
                result.Successful = false;
                //result.Response = null;
                result.Error = ex;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        public static DateTime GetFirstDayOfWeek(DateTime StartDate, DateTime EndDate, CultureInfo cultureInfo)
        {
            int Day = (StartDate.Day) * 1;
            DateTime? fakeDate = StartDate;
            if (fakeDate <= DateTime.Now)
            {
                while (fakeDate.Value.AddDays(+1) <= DateTime.Now)
                {
                    fakeDate = fakeDate.Value.AddDays(+1);
                }

            }

            return fakeDate.Value;
        }
        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        #endregion
        #region User Password
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage CheckUserExistence(string email)
        {
            try
            {
                UserManagement um = new UserManagement();
                var user = um.CheckUserExistence(email);
                if (user != null)
                {

                    Guid UniqueID = Guid.NewGuid();
                    string verifier = UniqueID.ToString() + "-" + DateTime.Now.Ticks.ToString();
                    PasswordResetRequest obj = new PasswordResetRequest()
                    {
                        Email = user.Email,
                        Verifier = verifier,
                        IsActive = true
                    };
                    string cname = string.Empty;
                    cname = um.GetUserInfo(user.Email).FirstName;
                    var request = um.GeneratePasswordResetRequest(obj);
                    if (request.Id != 0)
                    {
                        int i = Helper.PasswordResetEmail(cname, user.Email, "Home/PasswordUserReset", request.Verifier);

                        var details = um.GetUserInfoP(email);
                        if (details != null)
                        {
                            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
                            string PageURL = (string)settingsReader.GetValue("PageURL", typeof(String));
                            string EcEmail = Helper.Encrypt(email);
                            string ActivationUrl = "Your password has been reset.  Click this link to set up a new password and login." + PageURL + "Home/PasswordUserReset" + "?email=" + EcEmail + "&verifier=" + verifier + "&flag=" + 0;
                            // Date21Sept as per client Discussion code comment 
                            i = SendTwilloMessage.SendmessageTwillo(email, ActivationUrl);
                        }

                        objStatus.Message = i > 0 ? "Password request submitted successfully. You can reset your password from your Email ID on given link." : "error";
                        objStatus.MessageStatus = i > 0 ? true : false;
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                    }
                    else
                    {
                        objStatus.Message = "Some error occurred while processing your request.";
                        objStatus.MessageStatus = false;
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                    }


                }
                else
                {
                    objStatus.Message = "Emailid does not exist. Please check email address.";
                    objStatus.MessageStatus = false;
                    return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                }
            }
            catch (Exception ex)
            {
                objStatus.Message = "Some error occurred while processing your request.";
                objStatus.MessageStatus = false;
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }


        [HttpGet]
        public HttpResponseMessage PasswordUserReset(string email, string verifier)
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
                        objStatus.Message = "Thank you for create, kindly generate your password";
                        objStatus.MessageStatus = true;
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);

                    }
                    else
                    {
                        objStatus.Message = "Some error occurred while processing the request. Please try again";
                        objStatus.MessageStatus = false;
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                    }

                }

            }
            catch (Exception ex)
            {
                objStatus.Message = "Some error occurred while processing the request. Please try again";
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }

        [HttpPost]
        public HttpResponseMessage PasswordUserReset(string email, string password, string verifier)
        {
            try
            {
                if (!string.IsNullOrEmpty(password))
                {
                    int i = 0;
                    UserManagement um = new UserManagement();
                    email = email.Replace(" ", "+");
                    email = Helper.Decrypt(email);
                    var user = um.CheckUserExistence(email);
                    if (user.EntityType == "C")
                    {
                        i = um.ResetCompanyPassword(new User { Email = email, Password = password }, verifier);
                    }
                    if (user.EntityType == "E")
                    {
                        i = um.ResetUserPassword(new User { Email = email, Password = password }, verifier);
                    }
                    if (i > 0)
                    {
                        objStatus.Message = "Your password has been successfully changed, kindly login to your dashboard.";
                        objStatus.MessageStatus = true;
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                    }
                    else
                    {
                        objStatus.Message = "Some error occurred while processing the request. Please try again";
                        objStatus.MessageStatus = false;
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                objStatus.Message = "Some error occurred while processing the request. Please try again";
                objStatus.MessageStatus = false;
                Helper.ExceptionHandler.WriteToLogFile(ex.ToString());
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        #endregion
        #region HR Information
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage BasicInfo(long EmpId = 0)
        {
            EmployementBasicInfo objEmpbasicinfo = new EmployementBasicInfo();
            try
            {
                VW_Company_Employee_PersonalInfo objEmpinfo = objemployee.GetEmployeeDetails(EmpId);
                objEmpbasicinfo.ID = objEmpinfo.ID;
                objEmpbasicinfo.CompanyID = objEmpinfo.CompanyId;
                objEmpbasicinfo.FirstName = objEmpinfo.FirstName;
                objEmpbasicinfo.LastName = objEmpinfo.LastName;
                objEmpbasicinfo.PreferredName = objEmpinfo.PreferredName;
                objEmpbasicinfo.Address = objEmpinfo.Address;
                objEmpbasicinfo.City = objEmpinfo.City;
                objEmpbasicinfo.State = objEmpinfo.State;
                objEmpbasicinfo.Country = objEmpinfo.Country;
                objEmpbasicinfo.Zip = objEmpinfo.Zip;
                objEmpbasicinfo.Phone = objEmpinfo.Phone;
                objEmpbasicinfo.WorkPhone = objEmpinfo.WorkPhone;
                objEmpbasicinfo.WorkEmail = objEmpinfo.Email;
                objEmpbasicinfo.SSN = objEmpinfo.SSN;
                if (!string.IsNullOrEmpty(objEmpinfo.DOB))
                {
                    objEmpbasicinfo.EmpDOB = objEmpinfo.DOB;
                }
                if (string.IsNullOrEmpty(objEmpinfo.Gender))
                {
                    objEmpbasicinfo.GenderValue = null;
                }
                else
                {
                    objEmpbasicinfo.GenderValue = objEmpinfo.Gender.Trim();
                }

                string baseurl = Helper.GetBaseUrl();
                objEmpbasicinfo.EmergancyContactName = objEmpinfo.EmergancyContactName;
                objEmpbasicinfo.RelationWithContact = objEmpinfo.RelationWithContact;
                objEmpbasicinfo.EmergancyPhone = objEmpinfo.EmergancyPhone;
                //if(!string.IsNullOrEmpty(objEmpinfo.EmployeeImage) ? "/Areas/Company/Content/Upload/Employee/CompanyEmpProfile190x183/" + objEmpinfo.EmployeeImage : "/Areas/Employee/Content/images/user_smll_img.jpg");
                //objEmpbasicinfo.EmployeeImage = objEmpinfo.EmployeeImage;
                if (!string.IsNullOrEmpty(objEmpinfo.EmployeeImage))
                {
                    objEmpbasicinfo.EmployeeImage = baseurl + "/Areas/Company/Content/Upload/Employee/CompanyEmpProfile190x183/" + objEmpinfo.EmployeeImage;
                }
                else
                {
                    objEmpbasicinfo.EmployeeImage = baseurl + "/Areas/Employee/Content/images/user_smll_img.jpg";
                }
                objStatus.Message = "Basic Information.";
                objStatus.MessageStatus = true;
                objStatus.DataList = objEmpbasicinfo;

            }
            catch (Exception ex)
            {
                ObjElog.CustomErrorLog(ex.Message, "EMGAPI Controller", "ActionResult Index()", "ActionResult=[HttpGet]");
                objStatus.Message = "Some error occurred while processing your request.";
                objStatus.MessageStatus = false;
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage UploadEmployeeImage()
        {
            CompanyEmployeeManagement objComEmpMgnt = new CompanyEmployeeManagement();
            string strEmployeeImgOriginalPath, strEmpThumbnailImagePath_62px_62px, strProfileImagePath_270px_191x, strEmployeeImagePath_190px_183x, strEmployeeMstImagePath_110px_105x = string.Empty;
            tblCompany_Employee_PersonalInfo objPersoanlInfo = new tblCompany_Employee_PersonalInfo();
            string status = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;

            try
            {
                string postedImage = httpRequest["Image"];
                string strEmpId = httpRequest["EmpId"];
                long EmpId = Convert.ToInt64(strEmpId);

                strEmployeeImgOriginalPath = "~/Areas/Company/Content/Upload/Employee/CompanyEmployee_Original";
                strEmpThumbnailImagePath_62px_62px = "~/Areas/Company/Content/Upload/Employee/CompanyEmpThumb62x62";
                strProfileImagePath_270px_191x = "~/Areas/Company/Content/Upload/Employee/CompanyEmpImage270x191";
                strEmployeeImagePath_190px_183x = "~/Areas/Company/Content/Upload/Employee/CompanyEmpProfile190x183";
                strEmployeeMstImagePath_110px_105x = "~/Areas/Company/Content/Upload/Employee/CompanyEmpImg110x105";

                ImageResize ObjImgResize = new ImageResize();

                var files = httpRequest.Files;
                if (files.Count > 0)
                {
                    long companyID = OpenEnroll.GetCompanyID(EmpId);
                    var file = httpRequest.Files[0];

                    string strEmployeeOrgImg = "EmpImage-" + companyID + "-" + EmpId + "-" + file.FileName;
                    var ImagepathOrg = HttpContext.Current.Server.MapPath(strEmployeeImgOriginalPath);

                    string strEmployeeThumbImg = "EmpImage-" + companyID + "-" + EmpId + "-" + file.FileName;
                    var ImagepathThumb = HttpContext.Current.Server.MapPath(strEmpThumbnailImagePath_62px_62px);

                    string strEmployeeProfileImg = "EmpImage-" + companyID + "-" + EmpId + "-" + file.FileName;
                    var ImagepathProfile = HttpContext.Current.Server.MapPath(strProfileImagePath_270px_191x);

                    string strEmployeeImg = "EmpImage-" + companyID + "-" + EmpId + "-" + file.FileName;
                    var ImagepathEmp = HttpContext.Current.Server.MapPath(strEmployeeImagePath_190px_183x);

                    string strEmployeeMasetrImg = "EmpImage-" + companyID + "-" + EmpId + "-" + file.FileName;
                    var ImagemstpathEmp = HttpContext.Current.Server.MapPath(strEmployeeMstImagePath_110px_105x);

                    //Save image in directory
                    if (!Directory.Exists(ImagepathOrg))
                        Directory.CreateDirectory(ImagepathOrg);
                    ImagepathOrg = Path.Combine(ImagepathOrg, System.IO.Path.GetFileName(strEmployeeOrgImg));
                    file.SaveAs(ImagepathOrg);

                    // Update saved Image name in database
                    objPersoanlInfo.EmployeeImage = strEmployeeImg;
                    //objPersoanlInfo.EmpProfileImage = strEmployeeProfileImg;
                    //objPersoanlInfo.EmpThumbnailImage = strEmployeeThumbImg;
                    objPersoanlInfo.EmployeeID = EmpId;

                    int result = objComEmpMgnt.UpdateEmployeeImages(objPersoanlInfo);

                    if (result == 1)
                    {
                        string strOrgpath = HttpContext.Current.Server.MapPath(strEmployeeImgOriginalPath + @"/" + strEmployeeOrgImg);
                        System.Drawing.Image img_Original = System.Drawing.Image.FromFile(strOrgpath);

                        //Resize Thumbnail Image from Original Image
                        Image img_62px_62px = ObjImgResize.resizeImage_New(img_Original, new Size(62, 62));
                        string strpath1 = HttpContext.Current.Server.MapPath(strEmpThumbnailImagePath_62px_62px + "/" + strEmployeeThumbImg);
                        img_62px_62px.Save(strpath1);

                        //Resize Profile Image from Original Image
                        Image img_270px_191px = ObjImgResize.resizeImage_New(img_Original, new Size(270, 191));
                        string strpath2 = HttpContext.Current.Server.MapPath(strProfileImagePath_270px_191x + "/" + strEmployeeProfileImg);
                        img_270px_191px.Save(strpath2);

                        //Resize Employee Image from Original Image
                        Image img_190px_183x = ObjImgResize.resizeImage_New(img_Original, new Size(190, 183));
                        string strpath3 = HttpContext.Current.Server.MapPath(strEmployeeImagePath_190px_183x + "/" + strEmployeeImg);
                        img_190px_183x.Save(strpath3);

                        //Resize Employee Image from Original Image
                        Image img_110px_105x = ObjImgResize.resizeImage_New(img_Original, new Size(110, 105));
                        string strpath4 = HttpContext.Current.Server.MapPath(strEmployeeMstImagePath_110px_105x + "/" + strEmployeeImg);
                        img_110px_105x.Save(strpath4);

                        img_Original.Dispose();
                        img_62px_62px.Dispose();
                        img_270px_191px.Dispose();
                        img_190px_183x.Dispose();
                        img_110px_105x.Dispose();
                        //return strEmployeeImg;
                        objStatus.Message = "Image upload successfully";  //"Redirect Dependent";
                        objStatus.MessageStatus = true;
                    }
                }
                else
                {
                    objStatus.Message = "blank file";  //"Redirect Dependent";
                    objStatus.MessageStatus = false;
                }
            }
            catch (Exception ex)
            {
                objStatus.Message = "Some error occurred";  //"Redirect Dependent";
                objStatus.MessageStatus = false;
                ObjElog.CustomErrorLog(ex.Message, "EMGAPI Controller", "UploadEmployeeImage", "JsonCalling function");
                ModelState.AddModelError("Employee", ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }

        [HttpPost]
        public HttpResponseMessage PostBasicInfo(EmployementBasicInfoApi objEmpbasicinfo)
        {
            if (ModelState.IsValid)
            {
                string strCountry, strState, strCity = string.Empty;
                long Result = 0;
                try
                {
                    //Check validation
                    if (String.IsNullOrEmpty(objEmpbasicinfo.EmpDOB))
                    {
                        ModelState.AddModelError(objEmpbasicinfo.EmpDOB, "DOB is Required");
                    }

                    if (ModelState.IsValid)
                    {
                        strCountry = objEmpbasicinfo.Country;
                        strState = objEmpbasicinfo.State;
                        strCity = objEmpbasicinfo.City;

                        objEmpbasicinfo.lngCountryID = objComman.GetCountry(strCountry);
                        objEmpbasicinfo.lngStateID = objComman.GetState(strState, objEmpbasicinfo.lngCountryID);
                        objEmpbasicinfo.lngCityID = objComman.GetCity(strCity, objEmpbasicinfo.lngStateID, objEmpbasicinfo.lngCountryID);
                        tblCompany_Employee_PersonalInfo tblObjPersonalInfo = new tblCompany_Employee_PersonalInfo();
                        tblObjPersonalInfo.Address = objEmpbasicinfo.Address;
                        tblObjPersonalInfo.City = objEmpbasicinfo.lngCityID;
                        tblObjPersonalInfo.State = objEmpbasicinfo.lngStateID;
                        tblObjPersonalInfo.Country = objEmpbasicinfo.lngCountryID;
                        tblObjPersonalInfo.Zip = objEmpbasicinfo.Zip;
                        tblObjPersonalInfo.Phone = objEmpbasicinfo.Phone;
                        tblObjPersonalInfo.WorkPhone = objEmpbasicinfo.WorkPhone;
                        tblObjPersonalInfo.Gender = objEmpbasicinfo.GenderValue;
                        if (objEmpbasicinfo.DOB == null)
                        {
                            tblObjPersonalInfo.DOB = Convert.ToDateTime(objEmpbasicinfo.EmpDOB);
                        }

                        tblObjPersonalInfo.EmergancyContactName = objEmpbasicinfo.EmergancyContactName;
                        tblObjPersonalInfo.RelationWithContact = objEmpbasicinfo.RelationWithContact;
                        tblObjPersonalInfo.EmergancyPhone = objEmpbasicinfo.EmergancyPhone;

                        var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone(objEmpbasicinfo.Phone, "", Convert.ToInt32(objEmpbasicinfo.ID), 2, "");
                        if (duplicateComRecord.Count == 0)
                        {
                            Result = objemployee.UpdateEmployeeInfo(tblObjPersonalInfo, objEmpbasicinfo.ID, objEmpbasicinfo.OldPwd, objEmpbasicinfo.NewPwd);
                        }
                        else
                        {
                            Result = -1;
                        }

                        if (Result == -1)
                        {
                            objStatus.Message = "Emailid Or Phone already exists. Please try another";
                            objStatus.MessageStatus = false;
                        }

                        if (Result == 2)
                        {
                            objStatus.Message = "Password is invalid";
                            objStatus.MessageStatus = false;
                        }
                        else if (Result == 0)
                        {
                            objStatus.Message = "Some error occurred while processing your request.";
                            objStatus.MessageStatus = false;
                        }
                        else
                        {
                            objStatus.Message = "Information saved successfully";  //"Redirect Dependent";
                            objStatus.MessageStatus = true;
                            objStatus.DataList = OpenEnroll.GetEmployeeDependent(objEmpbasicinfo.ID);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);

                    }
                    else
                    {
                        objStatus.Message = "Some error occurred while processing your request.";
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                    }
                }
                catch (Exception ex)
                {
                    ObjElog.CustomErrorLog(ex.Message, "EMGAPI Controller", "ActionResult Index()", "ActionResult=[HttpPost]");
                    objStatus.Message = "Some error occurred while processing your request.";
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        #endregion
        #region Dependents
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetDependents(long EmpId = 0)
        {
            if (EmpId != 0)
            {
                var Dependents = OpenEnroll.GetEmployeeDependent(EmpId);
                objStatus.MessageStatus = true;
                objStatus.Message = "Dependent List";
                objStatus.DataList = Dependents;
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Some error occurred while processing your request.";

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage AddDependents(PowerIBrokerBusinessLayer.EnrollChklistValidationDepedent dependentlist)
        {
            try
            {
                tblCompany_Employee_DependentInfo dependentInfo = new tblCompany_Employee_DependentInfo();
                long companyID = OpenEnroll.GetCompanyID(dependentlist.EmpID);
                dependentInfo.EmployeeId = dependentlist.EmpID;
                dependentInfo.CompanyId = companyID;
                dependentInfo.ID = dependentlist.ID;
                dependentInfo.FirstName = dependentlist.FirstNameDependent;
                dependentInfo.LastName = dependentlist.LastNameDependent;
                dependentInfo.MiddleName = dependentlist.MiddleNameDependent;
                dependentInfo.NameTitle = dependentlist.NameTitle;
                dependentInfo.SSN = dependentlist.SSNDependent;
                dependentInfo.SpouseIsEmployed = dependentlist.SpouseIsEmployed;
                dependentInfo.SpouseHasCoverage = dependentlist.SpouseHasCoverage;
                if (dependentlist.Dependent == "1")
                {
                    dependentInfo.Dependent = "Spouse";
                }
                if (dependentlist.Dependent == "2")
                {
                    //dependentInfo.Dependent = "Children";
                    dependentInfo.Dependent = "Child";
                }
                if (dependentlist.Dependent == "3")
                {
                    dependentInfo.Dependent = "Other";
                }
                dependentInfo.Gender = dependentlist.Gender;
                dependentInfo.IsDisable = dependentlist.IsDisable;
                dependentInfo.IsSmoker = dependentlist.IsSmoker;
                dependentInfo.DOB = Convert.ToDateTime(dependentlist.DepDOB);
                dependentInfo.IsStudent = dependentlist.IsStudent;
                dependentInfo.IsActive = true;
                int result = OpenEnroll.AddDependents(dependentInfo);
                if (result == -5)
                {
                    objStatus.Message = "SSN already exists.";
                    objStatus.MessageStatus = true;
                    objStatus.DataList = OpenEnroll.GetEmployeeDependent(dependentlist.EmpID);
                }
                else
                {
                    if (result > 0)
                    {
                        objStatus.Message = "Dependent saved successfully.";
                        objStatus.MessageStatus = true;
                        objStatus.DataList = OpenEnroll.GetEmployeeDependent(dependentlist.EmpID);
                    }
                    else
                    {
                        objStatus.Message = "You can not add more than one spouse as a dependent.";
                        objStatus.MessageStatus = false;
                        return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                    }
                }


                //}
                //else
                //{
                //    objStatus.Message = "Some error occurred while processing your request.";
                //    objStatus.MessageStatus = false;
                //    return Request.CreateResponse(HttpStatusCode.OK, objStatus);
                //}

            }

            catch (Exception ex)
            {
                ObjElog.CustomErrorLog(ex.Message, "EMGAPI Controller", "ActionResult AddAdmin()", "ActionResult=[HttpPost]");
                objStatus.Message = "Some error occurred while processing your request.";
                objStatus.MessageStatus = false;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);

        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage EditDependentDetail(int ID)
        {
            PowerIBrokerBusinessLayer.EnrollChklistValidationDepedent details = null;

            try
            {
                long? empID = 0;
                if (ID != 0)
                {
                    empID = OpenEnroll.GetSpouseDeatils(ID).EmployeeId;
                }

                if (empID != null)
                {

                    details = OpenEnroll.GetEditDependentDetail(ID);
                    if (details.Dependent == "Spouse")
                    {
                        details.Dependent = "1";
                    }
                    //if (details.Dependent == "Children")
                    if (details.Dependent == "Child")
                    {
                        details.Dependent = "2";
                    }
                    if (details.Dependent == "Other")
                    {
                        details.Dependent = "3";
                    }
                    details.EmpID = Convert.ToInt64(empID);
                    details.DepDOB = details.DOBDependent.ToString();
                    objStatus.Message = "Dependent Data";
                    objStatus.MessageStatus = true;
                    objStatus.DataList = details;
                }
            }

            catch (Exception ex)
            {
                ObjElog.CustomErrorLog(ex.Message, "EMGAPI Controller", " EditDependentDetail", "");
                ModelState.AddModelError("Employee", ex.Message);
                objStatus.Message = "Some error occurred while processing your request.";
                objStatus.MessageStatus = false;
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);

        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage DeleteDependent(long ID)
        {
            int result = OpenEnroll.DeleteDependents(ID, 0, "", false);
            if (result == 1)
            {
                objStatus.Message = "Dependent Deleted successfully";
                objStatus.MessageStatus = true;
            }
            else
            {
                objStatus.Message = "Some error occurred while processing your request";
                objStatus.MessageStatus = false;

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);


        }
        #endregion
        #region Employment Information
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetEmploymentInfo(long EmpId = 0)
        {
            EmploymentInfomation objEmpInfo = new EmploymentInfomation();
            try
            {

                VW_Company_Employee_Employment_and_Compensation objEmp = objemployee.GetEmpInfoDetails(EmpId);
                if (objEmp != null)
                {
                    objEmpInfo.Id = objEmp.Id;
                    objEmpInfo.CompanyId = objEmp.CompanyId;
                    objEmpInfo.Title = objEmp.Title;
                    objEmpInfo.Location = objEmp.Location;
                    objEmpInfo.Department = objEmp.Department;
                    objEmpInfo.EmpTypeValue = objEmp.EmpTypeValue;
                    objEmpInfo.CompensationType = objEmp.CompensationType;
                    objEmpInfo.Title = objEmp.Title;
                    objEmpInfo.Bonus = objEmp.Bonus;
                    objEmpInfo.Commisiion = objEmp.Commisiion;
                    objEmpInfo.MonthlyBasis = objEmp.MonthlyBasis;
                    // objEmpInfo.Commisiion = objEmp.Commisiion;
                    if (string.IsNullOrEmpty(objEmp.StartDate))
                    {
                        objEmpInfo.StartDate = null;
                    }
                    else
                    {
                        objEmpInfo.StartDate = Convert.ToDateTime(objEmp.StartDate);
                    }

                    objEmpInfo.Salary = objEmp.Salary;
                    objStatus.Message = "Employment Information.";
                    objStatus.MessageStatus = true;
                    objStatus.DataList = objEmpInfo;
                }
                else
                {
                    objStatus.Message = "Some error occurred while processing your request.";
                    objStatus.MessageStatus = false;
                }

            }

            catch (Exception ex)
            {
                objStatus.Message = "Some error occurred while processing your request.";
                objStatus.MessageStatus = false;
                ObjElog.CustomErrorLog(ex.Message, "EMGAPI Controller", "ActionResult GetEmploymentInfo()", "ActionResult=[HttpGet]");
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        #endregion
        #region OpenEnrollmentApi
        //Get main Open Enrollment list
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage MainPlans(long EmpID = 0)
        {
            var compnyID = OpenEnroll.GetCompanyID(EmpID);
            var OpenEnrollData = OpenEnroll.GetOpenEnrollMent(EmpID, compnyID);
            var empinfo = OpenEnroll.GetEmployeeSalaryByEmpID(EmpID);
            if (empinfo != null)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(empinfo.StartDate)))
                {
                    objStatus.StartDate = Convert.ToString(empinfo.StartDate);
                }
                else
                {
                    objStatus.StartDate = "";
                }

            }

            if (OpenEnrollData.Count > 0)
            {
                foreach (var options in OpenEnrollData)
                {
                    var employmentDetails = OpenEnroll.GetEmployeeSalaryByEmpID(EmpID);
                    var dataexists = OpenEnroll.checkEligliblitySalaryband(EmpID, @options.ID);
                    var WaiverCheckBox = OpenEnroll.GetOpenEnrollMentCategoryWaiverStatus(EmpID, @options.ID, @options.Type);
                    var SpouseSurchargePopup = OpenEnroll.SpouseSurchargePopup(EmpID, @options.ID);
                    var WaiverStatus = "";
                    bool msgflag = true;
                    options.SpouseSurchargePopup = (SpouseSurchargePopup == true ? "true" : "false");
                    foreach (var item in WaiverCheckBox)
                    {
                        if (item.WaiveAllowedId == 1)
                        {
                            WaiverStatus = "Yes";
                        }
                        else
                        {
                            WaiverStatus = "No";
                            break;
                        }
                    }
                    options.WaiverStatus = WaiverStatus;



                    foreach (var item in dataexists)
                    {
                        long bonus = (item.IsBonus == true ? employmentDetails.Bonus : 0);
                        long commision = (item.IsCommission == true ? employmentDetails.Commisiion : 0);
                        long empsalary = Convert.ToInt64(employmentDetails.Salary) + bonus + commision;
                        var LstSalaryBand = OpenEnroll.GetSalaryBandBaseOnSalary(EmpID, item.ID);
                        if (LstSalaryBand != null)
                        {
                            var b = (LstSalaryBand).Where(a => a.SalaryStart <= empsalary && a.SalaryEnd >= empsalary).FirstOrDefault();
                            if (b == null)
                            {
                                b = (LstSalaryBand).Where(a => a.SalaryEnd == -1).FirstOrDefault();
                                if (b == null)
                                {
                                    msgflag = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            msgflag = false;
                        }

                    }
                    options.msgflag = (msgflag == true ? "true" : "false"); ;
                }
                objStatus.Message = "List of open enrollment for this employee.";
                objStatus.MessageStatus = true;
                objStatus.DataList = OpenEnrollData;
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }
            else
            {
                objStatus.Message = "No open enrollment for this employee.";
                objStatus.MessageStatus = false;
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }
        }
        //After Open Enrollment Get all Avaialable Waived ans selection list
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage PlanMenu(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                objStatus = GetCommonData(model);
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);

        }
        public Status GetCommonData(EmgApiModels model)
        {
            long TierID = OpenEnroll.GetTierID(model.EmpID);
            var OpenEnrollCat = OpenEnroll.GetOpenEnrollMentCategory(model.EmpID, model.OpnEnrollmentId, model.Type);
            if (OpenEnrollCat.Count > 0)
            {
                objStatus.Message = "Available Benefits";
                objStatus.MessageStatus = true;
                objStatus.DataList = OpenEnrollCat;
            }
            else
            {
                objStatus.Message = "No Available Benefits";
                objStatus.MessageStatus = false;
            }

            var OpenEnrollWavCat = OpenEnroll.GetOpenEnrollMentWaivedCategory(model.EmpID, model.OpnEnrollmentId, model.Type);
            if (OpenEnrollWavCat != null && OpenEnrollWavCat.Count > 0)
            {
                objStatus.Message1 = "Waived Benefits";
                objStatus.MessageStatus1 = true;
                objStatus.DataList1 = OpenEnrollWavCat;
            }
            else
            {
                objStatus.Message1 = "No Waived Benefits";
                objStatus.MessageStatus1 = false;
            }

            var OpenEnrollCatSel = OpenEnroll.GetOpenEnrollMentCategoryMySelection(model.EmpID, model.OpnEnrollmentId, model.Type);
            objStatus.DataList2 = OpenEnrollCatSel;


            //Paypercostbyamit
            //string Paypercost = GetPayperCost(model.EmpID, model.OpnEnrollmentId, model.Type);
            var OpenEnrollSelection = OpenEnroll.GetMonthlyCostMySelection(model.EmpID, model.OpnEnrollmentId, model.Type);
            if ((!object.Equals(OpenEnrollSelection, null)) && (OpenEnrollSelection.Count() > 0))
            {
                var TotalSum = OpenEnrollSelection.Where(v => v.IsActive == true).Sum(item => item.MonthlyCost);
                if (OpenEnrollSelection.Count > 0)
                {
                    objStatus.Paypercost = Convert.ToString(TotalSum);

                }
            }
            var PlanSelected = OpenEnrollCatSel.Where(a => a.IsActive == true).ToList();
            if (PlanSelected.Count > 0)
            {
                objStatus.PlanCount = PlanSelected.Count;
            }
            else
            {
                objStatus.PlanCount = 0;
            }

            return objStatus;
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetPlanDetailsByCatAndEnrollID(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {

                long planid = 0;
                objStatus.IsWaived = -1;
                bool IsSpouseAvail = false;
                IsSpouseAvail = OpenEnroll.GetEmployeeSpouse(model.EmpID);
                objStatus.IsSpouseAvail = IsSpouseAvail;

                var compnyID = OpenEnroll.GetCompanyID(model.EmpID);
                var OpenEnrollData = OpenEnroll.GetOpenEnrollMent(model.EmpID, compnyID);
                var GetEnrollType = OpenEnrollData.Where(a => a.ID == model.OpnEnrollmentId).FirstOrDefault();
                var EnrollType = GetEnrollType.EnrollType.ToUpper();
                var PlanData = OpenEnroll.ApiGetPlanDetailsByCatAndEnrollID(model.OpnEnrollmentId, Convert.ToInt64(model.PlanCategoryID), model.Type, model.EmpID, EnrollType);
                var Iswaived = (OpenEnroll.GetOpenEnrollMentCategoryMySelection(model.EmpID, model.OpnEnrollmentId, model.Type).Where(a => a.PlanCategoryID == Convert.ToInt64(model.PlanCategoryID)).ToList());

                if (Iswaived.Count > 0)
                {
                    if (Convert.ToBoolean(Iswaived[0].IsActive) == false)
                        objStatus.IsWaived = 1;
                    if (Convert.ToBoolean(Iswaived[0].IsActive) == true)
                        objStatus.IsWaived = 0;
                    if ((Iswaived[0].IsActive) == null)
                        objStatus.IsWaived = 0;
                }


                if (PlanData.Count > 0)
                {
                    objStatus.Message = "Plan Details.";
                    objStatus.MessageStatus = true;
                    objStatus.DataList = PlanData;
                    planid = PlanData[0].ID;
                }
                else
                {
                    objStatus.Message = "No Plan Details available.";
                    objStatus.MessageStatus = false;
                }

                var DependentList = OpenEnroll.ApiGetSelfandDependents(model.OpnEnrollmentId, Convert.ToInt64(model.PlanCategoryID), model.Type, model.EmpID).ToList();
                if (DependentList.Count > 0)
                {
                    if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Medical || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Dental || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Vision)
                    {
                        if (model.Flag == 1)
                        {
                            DependentList.ForEach(i => i.IsSelected = false);
                        }
                        else
                        {
                            if (Iswaived[0].IsActive == false || Iswaived[0].IsActive == null)
                            {
                                DependentList.ForEach(i => i.IsSelected = true);

                            }
                            foreach (var i in DependentList.Where(a => a.IsSelected == true))
                            {
                                model.DependentIDs = model.DependentIDs + "," + i.ID;
                            }
                            model.DependentIDs = model.DependentIDs.TrimStart(',');

                        }
                        objStatus.Message1 = "Dependent List";
                        objStatus.MessageStatus1 = true;
                        objStatus.DataList1 = DependentList;
                    }
                }
                else
                {
                    objStatus.Message1 = "No Dependent List";
                    objStatus.MessageStatus1 = false;
                }

                List<EMG_CoverageOptions> detailsdata = new List<EMG_CoverageOptions>();

                long SalaryBandId = 0;

                if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Medical || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Dental || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Vision)
                {
                    bool checkdepSpouse = false;
                    bool SpouseSurcharge = false;
                    foreach (var plan in PlanData)
                    {
                        long TierID = OpenEnroll.GetTierIDBand(model.EmpID, plan.ID);

                        long depID = OpenEnroll.GetMainID(TierID, 1);

                        if (DependentList != null)
                        {
                            depID = OpenEnroll.CommonDependents(model.OpnEnrollmentId, "OE", plan.PlanCategoryID, Convert.ToString(model.DependentIDs), TierID, Convert.ToInt64(model.EmpID));
                        }

                        var employmentDetails = OpenEnroll.GetEmployeeSalaryByEmpID(model.EmpID);
                        long bonus = (plan.IsBonus == true ? employmentDetails.Bonus : 0);
                        long commision = (plan.IsCommission == true ? employmentDetails.Commisiion : 0);
                        long empsalary1 = Convert.ToInt64(employmentDetails.Salary) + bonus + commision;
                        List<PowerIBrokerBusinessLayer.Employee.Emg_SalaryBand> LstSalaryBand = new List<PowerIBrokerBusinessLayer.Employee.Emg_SalaryBand>();
                        LstSalaryBand = OpenEnroll.GetSalaryBandBaseOnSalary(model.EmpID, plan.ID);
                        if (LstSalaryBand != null)
                        {
                            var item = ((List<PowerIBrokerBusinessLayer.Employee.Emg_SalaryBand>)LstSalaryBand).Where(a => a.SalaryStart <= empsalary1 && a.SalaryEnd >= empsalary1).FirstOrDefault();
                            if (item == null)
                            {
                                item = ((List<PowerIBrokerBusinessLayer.Employee.Emg_SalaryBand>)LstSalaryBand).Where(a => a.SalaryEnd == -1).FirstOrDefault();
                            }
                            SalaryBandId = item.ID;
                        }
                        var DependentCost = OpenEnroll.GetCostByOpenEnrollment(plan.ID, depID, Convert.ToInt64(SalaryBandId));
                        var SpouseSurchargePopup = OpenEnroll.SpouseSurchargePopup(model.EmpID, model.OpnEnrollmentId);
                        var SpouseHasCoverage = OpenEnroll.GetEmployeeSpouseHasCoverage(model.EmpID);
                        var DependentData = OpenEnroll.GetSelfandDependentsbyIDs(model.DependentIDs);

                        if (DependentData.Count > 0)
                        {
                            int conspouse = DependentData.Where(a => a.Dependent.Contains("Spouse")).ToList().Count();
                            checkdepSpouse = (conspouse > 0 ? true : false);
                        }
                        if (SpouseSurchargePopup == true && SpouseHasCoverage == true && checkdepSpouse == true)
                        {
                            SpouseSurcharge = true;
                            if (DependentCost.CoverageOption == EnumString.Employee_Spouse || DependentCost.CoverageOption == EnumString.Employee_1 || DependentCost.CoverageOption == EnumString.Family)
                            {
                                string NewSurchargeOptions = string.Empty;
                                NewSurchargeOptions = (DependentCost.CoverageOption == EnumString.Family ? EnumString.Family_Surcharge : DependentCost.CoverageOption == EnumString.Employee_1 ? EnumString.E1_Surcharge : DependentCost.CoverageOption == EnumString.Employee_Spouse ? EnumString.ES_Surcharge : "");
                                var Editrecord1 = OpenEnroll.GetCostNewSurcharge(Convert.ToInt64(DependentCost.InsurancePlanID), NewSurchargeOptions, Convert.ToInt64(DependentCost.SalaryBandId));
                                var detailsdataSurcharge = OpenEnroll.GetCostByOpenEnrollmentSurcharge(Editrecord1.ID);
                                if (detailsdataSurcharge != null)
                                {
                                    detailsdata.Add(detailsdataSurcharge);
                                }
                                else
                                {
                                    detailsdata.Add(DependentCost);
                                }
                            }
                            else
                            {
                                detailsdata.Add(DependentCost);
                            }
                        }
                        else
                        {

                            detailsdata.Add(DependentCost);
                        }

                    }

                    if (detailsdata.Count > 0)
                    {
                        objStatus.Message2 = "Cost List";
                        objStatus.MessageStatus2 = true;
                        objStatus.DataList2 = detailsdata;
                    }
                    else
                    {
                        objStatus.Message2 = "No Cost List";
                        objStatus.MessageStatus2 = false;
                    }
                }
                else
                {
                    var cost = OpenEnroll.GetCostByOpenEnrollmentandEmpID(Convert.ToInt64(planid), 1, model.OpnEnrollmentId, model.EmpID, "OE");
                    if (cost != null)
                    {
                        objStatus.Message2 = "Cost List";
                        objStatus.MessageStatus2 = true;
                        objStatus.DataList2 = cost;
                    }
                    else
                    {
                        objStatus.Message2 = "No Cost List";
                        objStatus.MessageStatus2 = false;
                    }
                }
                if ((Convert.ToInt64(model.PlanCategoryID) != 9) && (Convert.ToInt64(model.PlanCategoryID) != 10) && (Convert.ToInt64(model.PlanCategoryID) != 12))
                {
                    var PlanMinMax = OpenEnroll.GetMinMaxByInsurancePlanID(planid);
                    if (PlanMinMax != null)
                    {
                        objStatus.Message3 = "Plan Min Max Value";
                        objStatus.MessageStatus3 = true;
                        objStatus.DataList3 = PlanMinMax;
                    }
                    else
                    {
                        objStatus.Message3 = "No Plan Min Max Value";
                        objStatus.MessageStatus3 = false;
                    }
                }
                else
                {
                    var PlanMinMax = OpenEnroll.GetFSAMinMaxByInsurancePlanID(planid);
                    if (PlanMinMax != null)
                    {
                        objStatus.Message3 = "Plan Min Max Value";
                        objStatus.MessageStatus3 = true;
                        objStatus.DataList3 = PlanMinMax;
                    }
                    else
                    {
                        objStatus.Message3 = "No Plan Min Max Value";
                        objStatus.MessageStatus3 = false;
                    }
                }

                var AgeCalc = OpenEnroll.GetAgeByDate(Convert.ToInt64(model.EmpID), Convert.ToInt64(planid));
                if (AgeCalc != null)
                {
                    objStatus.Message4 = "Age Calculation";
                    objStatus.MessageStatus4 = true;
                    objStatus.DataList4 = AgeCalc;
                }
                else
                {
                    objStatus.Message4 = "No Age Calculation found";
                    objStatus.MessageStatus4 = false;
                }

                var empsalary = OpenEnroll.GetEmployeeSalaryByEmpID(Convert.ToInt64(model.EmpID));

                if (empsalary != null)
                {
                    objStatus.Message5 = "Salary Calculation";
                    objStatus.MessageStatus5 = true;
                    EmploymentDetails details = new EmploymentDetails();
                    details.ID = empsalary.ID;
                    details.EmployeeID = empsalary.EmployeeID;
                    details.StartDate = empsalary.StartDate;
                    details.Type = empsalary.Type;
                    details.EmployeeType = empsalary.EmployeeType;
                    details.Salary = empsalary.Salary;
                    details.Status = empsalary.Status;
                    details.SSN = empsalary.SSN;
                    details.Bonus = empsalary.Bonus;
                    details.Commisiion = empsalary.Commisiion;
                    details.MonthlyBasisID = empsalary.MonthlyBasisID;
                    details.DivisionID = empsalary.DivisionID;
                    objStatus.DataList5 = details;
                }
                else
                {
                    objStatus.Message5 = "No Salary Calculation found";
                    objStatus.MessageStatus5 = false;
                }
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetSelfandDependents(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                var DependentList = OpenEnroll.ApiGetSelfandDependents(model.OpnEnrollmentId, Convert.ToInt64(model.PlanCategoryID), model.Type, model.EmpID);
                if (DependentList.Count > 0)
                {
                    objStatus.Message1 = "Dependent List";
                    objStatus.MessageStatus1 = true;
                    objStatus.DataList1 = DependentList;
                }
                else
                {
                    DependentList = new List<API_EmployeeDependent_Result>();
                    objStatus.DataList1 = DependentList;
                    objStatus.Message1 = "No Dependent List";
                    objStatus.MessageStatus1 = false;
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        //Get Plan Details by category
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetMySelectedPlans(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                //long? planid = 0;
                //long? DepCovId = 0;
                //SelectedPlan obj = null;
                //int i = 0;
                //List<SelectedPlan> list = new List<SelectedPlan>();
                //long TierID = OpenEnroll.GetTierID(model.EmpID);
                //  long TierID = OpenEnroll.GetTierIDBand(model.EmpID, 40091);
                // var OpenEnrollSelection = OpenEnroll.GetOpenEnrollMentCategoryMySelection(model.EmpID, model.OpnEnrollmentId, model.Type);
                var OpenEnrollSelection = OpenEnroll.GetMonthlyCostMySelection(model.EmpID, model.OpnEnrollmentId, model.Type);
                //if ((!object.Equals(OpenEnrollSelection, null)) && (OpenEnrollSelection.Count() > 0))
                //{

                //    objStatus.SuccessMessage = true;
                //    objStatus.Message = "No Selected Plan Found";

                //    foreach (var row in OpenEnrollSelection)
                //    {

                //        planid = row.PlanID;
                //        if (TierID == 2)
                //        {
                //            if (row.DependentCoveragrID == 1)
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, 1);
                //            }
                //            else
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, 4);
                //            }
                //        }
                //        else if (TierID == 3)
                //        {
                //            if (row.DependentCoveragrID == 1)
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, 1);
                //            }
                //            else if (row.DependentCoveragrID == 2)
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, 3);
                //            }
                //            else
                //            {
                //                var checkeddep = OpenEnroll.GetCheckedDependents(model.OpnEnrollmentId, Convert.ToInt64(row.PlanCategoryID), model.Type, model.EmpID).ToList();

                //                if (checkeddep.Count == 1)
                //                {
                //                    DepCovId = OpenEnroll.GetMainID(TierID, 3);
                //                }
                //                else
                //                {
                //                    DepCovId = OpenEnroll.GetMainID(TierID, 4);
                //                }

                //            }
                //        }
                //        else if (TierID == 4)
                //        {
                //            if (row.DependentCoveragrID == 1)
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, 1);
                //            }
                //            else if (row.DependentCoveragrID == 2)
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, 2);
                //            }
                //            else if (row.DependentCoveragrID == 3)
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, 3);
                //            }
                //            else
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, 4);
                //            }
                //        }

                //        else
                //        {
                //            if (row.DependentCoveragrID == 1)
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, 1);
                //            }
                //            else
                //            {
                //                DepCovId = OpenEnroll.GetMainID(TierID, Convert.ToInt64(row.DependentCoveragrID));
                //            }
                //        }


                //        if (DepCovId == null)
                //        {
                //            DepCovId = 0;
                //        }
                //        if (row.PlanCategoryID == 1 || row.PlanCategoryID == 2 || row.PlanCategoryID == 3)
                //        {
                //            var plancost = OpenEnroll.GetCostByOpenEnrollment(Convert.ToInt64(planid), Convert.ToInt64(row.DependentCoveragrID), 0);


                //            obj = new SelectedPlan();
                //            obj.PlanCategory = OpenEnrollSelection[i].CategoryName.ToString();
                //            obj.Status = Convert.ToBoolean(OpenEnrollSelection[i].IsActive);

                //            if (OpenEnrollSelection[i].IsActive == true)
                //                obj.PlanStatus = "1";
                //            if (OpenEnrollSelection[i].IsActive == false)
                //                obj.PlanStatus = "2";
                //            if (OpenEnrollSelection[i].IsActive == null)
                //                obj.PlanStatus = "0";

                //            if ((!object.Equals(plancost, null)))
                //            {
                //                obj.MonthlyCost = plancost.MonthlyCost.ToString();
                //                obj.SemiMonthly = plancost.SemiMonthlyCost.ToString();
                //                obj.WeeklyCost = plancost.WeeklyCost.ToString();
                //                obj.BiWeeklyCost = plancost.BiWeekly.ToString();

                //                if (OpenEnrollSelection[0].MonthlyBasisID == 1)
                //                {
                //                    obj.DisplayCost = plancost.WeeklyCost.ToString();
                //                }
                //                else if (OpenEnrollSelection[0].MonthlyBasisID == 2)
                //                {
                //                    obj.DisplayCost = plancost.BiWeekly.ToString();
                //                }
                //                else if (OpenEnrollSelection[0].MonthlyBasisID == 3)
                //                {
                //                    obj.DisplayCost = plancost.SemiMonthlyCost.ToString();
                //                }
                //                else if (OpenEnrollSelection[0].MonthlyBasisID == 4)
                //                {
                //                    obj.DisplayCost = plancost.MonthlyCost.ToString();
                //                }
                //                else
                //                {
                //                    obj.DisplayCost = "0.00";
                //                }

                //                if (OpenEnrollSelection[i].IsActive == false)
                //                {
                //                    obj.MonthlyCost = "0.00";
                //                    obj.SemiMonthly = "0.00";
                //                    obj.WeeklyCost = "0.00";
                //                    obj.BiWeeklyCost = "0.00";
                //                    obj.DisplayCost = "0.00";
                //                }

                //                //list.Add(obj);
                //            }
                //            else
                //            {
                //                obj.MonthlyCost = "0.00";
                //                obj.SemiMonthly = "0.00";
                //                obj.WeeklyCost = "0.00";
                //                obj.BiWeeklyCost = "0.00";
                //                obj.DisplayCost = "0.00";
                //            }
                //        }
                //        else
                //        {
                //            var plancost = OpenEnroll.GetCostByOpenEnrollmentandEmpID(Convert.ToInt64(planid), 1, row.OpenEnrollID, model.EmpID, row.Type);


                //            obj = new SelectedPlan();
                //            obj.PlanCategory = OpenEnrollSelection[i].CategoryName.ToString();
                //            obj.Status = Convert.ToBoolean(OpenEnrollSelection[i].IsActive);

                //            if (OpenEnrollSelection[i].IsActive == true)
                //                obj.PlanStatus = "1";
                //            if (OpenEnrollSelection[i].IsActive == false)
                //                obj.PlanStatus = "2";
                //            if (OpenEnrollSelection[i].IsActive == null)
                //                obj.PlanStatus = "0";

                //            if ((!object.Equals(plancost, null)))
                //            {
                //                obj.MonthlyCost = plancost.MonthlyCost.ToString();
                //                obj.SemiMonthly = plancost.SemiMonthlyCost.ToString();
                //                obj.WeeklyCost = plancost.WeeklyCost.ToString();
                //                obj.BiWeeklyCost = plancost.BiWeekly.ToString();

                //                if (OpenEnrollSelection[0].MonthlyBasisID == 1)
                //                {
                //                    obj.DisplayCost = plancost.WeeklyCost.ToString();
                //                }
                //                else if (OpenEnrollSelection[0].MonthlyBasisID == 2)
                //                {
                //                    obj.DisplayCost = plancost.BiWeekly.ToString();
                //                }
                //                else if (OpenEnrollSelection[0].MonthlyBasisID == 3)
                //                {
                //                    obj.DisplayCost = plancost.SemiMonthlyCost.ToString();
                //                }
                //                else if (OpenEnrollSelection[0].MonthlyBasisID == 4)
                //                {
                //                    obj.DisplayCost = plancost.MonthlyCost.ToString();
                //                }
                //                else
                //                {
                //                    obj.DisplayCost = "0.00";
                //                }

                //                if (OpenEnrollSelection[i].IsActive == false)
                //                {
                //                    obj.MonthlyCost = "0.00";
                //                    obj.SemiMonthly = "0.00";
                //                    obj.WeeklyCost = "0.00";
                //                    obj.BiWeeklyCost = "0.00";
                //                    obj.DisplayCost = "0.00";
                //                }

                //                //list.Add(obj);
                //            }
                //            else
                //            {
                //                obj.MonthlyCost = "0.00";
                //                obj.SemiMonthly = "0.00";
                //                obj.WeeklyCost = "0.00";
                //                obj.BiWeeklyCost = "0.00";
                //                obj.DisplayCost = "0.00";
                //            }
                //        }
                //        list.Add(obj);
                //        i = i + 1;
                //    }


                //    if (list.Count > 0)
                //    {
                //        objStatus.Message = "Select Plan List";
                //        objStatus.MessageStatus = true;
                //        objStatus.DataList = list;

                //    }
                //    else
                //    {
                //        objStatus.Message = "No Selected Plan List Found";
                //        objStatus.MessageStatus = false;
                //    }
                //}
                if ((!object.Equals(OpenEnrollSelection, null)) && (OpenEnrollSelection.Count() > 0))
                {
                    var TotalSum = OpenEnrollSelection.Where(v => v.IsActive == true).Sum(item => item.MonthlyCost);
                    if (OpenEnrollSelection.Count > 0)
                    {

                        objStatus.Message = "Select Plan List";
                        objStatus.MessageStatus = true;
                        objStatus.DataList = OpenEnrollSelection;
                        objStatus.TotalSum = TotalSum;

                    }
                }
                else
                {
                    objStatus.SuccessMessage = false;
                    objStatus.Message = "No Selected Plan Found";

                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
                //return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage WaiveBenefitOmEmployee(EmgApiModels model)
        {
            int result = 0;
            tblEmployee_Benefits benefits = new tblEmployee_Benefits();
            if (model.EmpID != 0)
            {
                result = OpenEnroll.WaiveBenefitOmEmployee(model.EmpID, model.OpnEnrollmentId, Convert.ToInt64(model.PlanCategoryID), model.Type, model.WaiveReason, Convert.ToInt64(model.WaiveOptionID), Convert.ToInt32(model.EmpID), "Employee");
                objStatus = GetCommonData(model);
                //Paypercostyamit
                var OpenEnrollSelection = OpenEnroll.GetMonthlyCostMySelection(model.EmpID, model.OpnEnrollmentId, model.Type);
                if ((!object.Equals(OpenEnrollSelection, null)) && (OpenEnrollSelection.Count() > 0))
                {
                    var TotalSum = OpenEnrollSelection.Where(v => v.IsActive == true).Sum(item => item.MonthlyCost);
                    if (OpenEnrollSelection.Count > 0)
                    {
                        objStatus.Paypercost = Convert.ToString(TotalSum);
                        //  objStatus.TotalSum = TotalSum;
                    }
                }
                //string Paypercost = GetPayperCost(model.EmpID, model.OpnEnrollmentId, model.Type);
                //objStatus.Paypercost = Paypercost;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [HttpPost]
        public HttpResponseMessage GetCheckedDependents(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                var data = OpenEnroll.GetCheckedDependents(model.OpnEnrollmentId, 1, model.Type, model.EmpID);
                objStatus.Message = "Dependent List";
                objStatus.MessageStatus = true;
                objStatus.GetDependents = data;
                //GetDependents

            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        //long? PlanID, long EmpID, string DependentIDs, long EnrollID, string Type, long CatID, long CovID, string Unselect = "", string Monthlycost = "", string Slidselectedval = "0", string Spval = "0"
        public HttpResponseMessage SelectBenefitsOnEmployee(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                long Result1 = 0;
                int result = -1;
                bool checkdepSpouse = false;
                bool SpouseSurcharge = false;
                bool Coverageoption = false;
                string strEmployeeOrgImg = "";
                long SalaryBandId = 0;
                long covID = 0;
                tblEmployee_Benefits benefits = new tblEmployee_Benefits();
                long TierID = OpenEnroll.GetTierIDBand(Convert.ToInt64(model.EmpID), Convert.ToInt64(model.PlanID));

                long fakeDepCovID = 0;
                if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Medical || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Dental || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Vision)
                {
                    fakeDepCovID = OpenEnroll.GetMainID(TierID, 1);
                }

                string Spouseval = model.Spval.ToString();
                if (model.EmpID != 0)
                {
                    if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Medical || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Dental || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Vision)
                    {

                        var EnrollType = "";
                        var compnyID = OpenEnroll.GetCompanyID(Convert.ToInt64(model.EmpID));
                        var OpenEnrollData = OpenEnroll.GetOpenEnrollMent(Convert.ToInt64(model.EmpID), compnyID);
                        if (OpenEnrollData != null)
                        {
                            var GetEnrollType = OpenEnrollData.Where(a => a.ID == model.OpnEnrollmentId).FirstOrDefault();
                            EnrollType = GetEnrollType.EnrollType.ToUpper();
                        }

                        fakeDepCovID = OpenEnroll.CommonDependents(Convert.ToInt64(model.OpnEnrollmentId), "OE", Convert.ToInt32(model.PlanCategoryID), model.DependentIDs, Convert.ToInt64(TierID), Convert.ToInt64(model.EmpID));

                        var employmentDetails = OpenEnroll.GetEmployeeSalaryByEmpID(model.EmpID);
                        var PlanData = OpenEnroll.GetPlanDetailsByCatAndEnrollID(Convert.ToInt64(model.OpnEnrollmentId), Convert.ToInt64(model.PlanCategoryID), "OE", model.EmpID, EnrollType);
                        long bonus = 0; //(plan.IsBonus == true ? employmentDetails.Bonus : 0);
                        long commision = 0; //(plan.IsCommission == true ? employmentDetails.Commisiion : 0);

                        if (PlanData != null && PlanData.Count > 0)
                        {
                            var planDetails = PlanData.Where(a => a.ID == model.PlanID).FirstOrDefault();
                            bonus = (planDetails.IsBonus == true ? employmentDetails.Bonus : 0);
                            commision = (planDetails.IsCommission == true ? employmentDetails.Commisiion : 0);
                        }
                        long empsalary = Convert.ToInt64(employmentDetails.Salary) + bonus + commision;
                        var LstSalaryBand = OpenEnroll.GetSalaryBandBaseOnSalary(Convert.ToInt64(model.EmpID), Convert.ToInt64(model.PlanID));
                        if (LstSalaryBand != null && LstSalaryBand.Count > 0)
                        {
                            var item = (LstSalaryBand).Where(a => a.SalaryStart <= empsalary && a.SalaryEnd >= empsalary).FirstOrDefault();
                            if (item == null)
                            {
                                item = (LstSalaryBand).Where(a => a.SalaryEnd == -1).FirstOrDefault();
                            }
                            SalaryBandId = item.ID;
                        }


                        var detailsdata = OpenEnroll.GetCostByOpenEnrollment(Convert.ToInt64(model.PlanID), fakeDepCovID, Convert.ToInt64(SalaryBandId));
                        covID = (detailsdata != null ? detailsdata.ID : 0);
                        model.CovID = covID;
                    }
                    else
                    {
                        var cost = OpenEnroll.GetCostByOpenEnrollmentandEmpID(Convert.ToInt64(model.PlanID), 1, model.OpnEnrollmentId, model.EmpID, "OE");
                        covID = (cost != null ? cost.ID : 0);
                        model.CovID = covID;
                    }

                    if (model.Unselect == "Y")
                    {
                        result = OpenEnroll.UnSelectBenefitsOnEmployee(model.EmpID, "0", model.PlanID, model.OpnEnrollmentId, model.Type, Convert.ToInt64(model.PlanCategoryID), Convert.ToInt64(model.CovID), Convert.ToInt32(model.EmpID), "Employee");
                    }
                    else
                    {
                        if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.VoluntaryLife || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.OtherVoluntary || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.VoluntaryLongTermDisability)
                        {
                            string strCompanyID = string.Empty;
                            EMG_EmployeeCoverage ObjCoverOpt = new EMG_EmployeeCoverage();
                            ObjCoverOpt.EmployeeID = model.EmpID;
                            ObjCoverOpt.OpenEnrollID = model.OpnEnrollmentId;
                            ObjCoverOpt.Type = model.Type;
                            ObjCoverOpt.MonthlyCost = Convert.ToDecimal(model.Monthlycost);
                            ObjCoverOpt.WeeklyCost = Convert.ToDecimal((ObjCoverOpt.MonthlyCost * 12) / 52);
                            ObjCoverOpt.BiWeekly = Convert.ToDecimal((ObjCoverOpt.MonthlyCost * 12) / 26);
                            ObjCoverOpt.SemiMonthlyCost = Convert.ToDecimal((ObjCoverOpt.MonthlyCost * 12) / 24);
                            ObjCoverOpt.InsurancePlanID = Convert.ToInt64(model.PlanID);
                            ObjCoverOpt.CoverageOption = EnumString.Employee_Only;
                            if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                            {
                                string AddedValue = "Employee";
                                var details = OpenEnroll.GetMinMaxByInsurancePlanID(Convert.ToInt64(model.PlanID));

                                if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.VoluntaryLife && details.IsSpouse == true && (model.Spval != "0.00" && model.Spval != "0.0" && model.Spval != "0"))
                                {
                                    AddedValue = AddedValue + " + Spouse";
                                }
                                else
                                {
                                    model.SpouseSelectedRange = 0;
                                }
                                if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.VoluntaryLife && details.IsChild == true && (model.childval != "0.00" && model.childval != "0.0" && model.childval != "0"))
                                {
                                    AddedValue = AddedValue + " + Child";
                                }
                                else
                                {
                                    model.childval = "0";
                                }
                                if (AddedValue == "Employee")
                                {
                                    AddedValue = EnumString.Employee_Only;
                                }
                                ObjCoverOpt.CoverageOption = AddedValue;

                            }
                            Result1 = OpenEnroll.SaveCostOption(ObjCoverOpt);
                            // model.CovID = Result1;
                        }
                        if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Life || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.ShortTermDisability || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.LongTermDisability)
                        {
                            string strCompanyID = string.Empty;
                            EMG_EmployeeCoverage ObjCoverOpt = new EMG_EmployeeCoverage();
                            ObjCoverOpt.EmployeeID = model.EmpID;
                            ObjCoverOpt.OpenEnrollID = model.OpnEnrollmentId;
                            ObjCoverOpt.Type = model.Type;
                            ObjCoverOpt.MonthlyCost = Convert.ToDecimal(model.Monthlycost);
                            ObjCoverOpt.WeeklyCost = Convert.ToDecimal((ObjCoverOpt.MonthlyCost * 12) / 52);
                            ObjCoverOpt.BiWeekly = Convert.ToDecimal((ObjCoverOpt.MonthlyCost * 12) / 26);
                            ObjCoverOpt.SemiMonthlyCost = Convert.ToDecimal((ObjCoverOpt.MonthlyCost * 12) / 24);
                            ObjCoverOpt.InsurancePlanID = Convert.ToInt64(model.PlanID);
                            ObjCoverOpt.CoverageOption = EnumString.Employee_Only;
                            Coverageoption = true;

                            Result1 = OpenEnroll.SaveCostOption(ObjCoverOpt);
                            // model.CovID = Result1;
                        }
                        if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.FSA || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.HSA || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.TSA)
                        {
                            string strCompanyID = string.Empty;
                            EMG_EmployeeCoverage ObjCoverOpt = new EMG_EmployeeCoverage();
                            ObjCoverOpt.EmployeeID = model.EmpID;
                            ObjCoverOpt.OpenEnrollID = model.OpnEnrollmentId;
                            ObjCoverOpt.Type = model.Type;
                            ObjCoverOpt.MonthlyCost = Convert.ToDecimal(model.Monthlycost);
                            ObjCoverOpt.CoverageOption = EnumString.Employee_Only;
                            var details = OpenEnroll.GetFSAMinMaxByInsurancePlanID(Convert.ToInt64(model.PlanID));
                            if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.FSA && details.SponsoredVal == true)
                            {
                                ObjCoverOpt.CoverageOption = EnumString.Employee_Child;
                            }
                            if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.TSA)
                            {
                                ObjCoverOpt.WeeklyCost = Convert.ToDecimal((ObjCoverOpt.MonthlyCost * 12) / 52);
                                ObjCoverOpt.BiWeekly = Convert.ToDecimal((ObjCoverOpt.MonthlyCost * 12) / 26);
                                ObjCoverOpt.SemiMonthlyCost = Convert.ToDecimal((ObjCoverOpt.MonthlyCost * 12) / 24);
                                if (details.TSAType == 1)
                                {
                                    ObjCoverOpt.CoverageOption = EnumString.Transit;
                                }
                                else if (details.TSAType == 2)
                                {
                                    ObjCoverOpt.CoverageOption = EnumString.Parking;
                                }
                                else
                                {
                                    ObjCoverOpt.CoverageOption = EnumString.Transit_Parking;
                                }
                            }
                            else
                            {
                                ObjCoverOpt.WeeklyCost = Convert.ToDecimal((ObjCoverOpt.MonthlyCost) / 52);
                                ObjCoverOpt.BiWeekly = Convert.ToDecimal((ObjCoverOpt.MonthlyCost) / 26);
                                ObjCoverOpt.SemiMonthlyCost = Convert.ToDecimal((ObjCoverOpt.MonthlyCost) / 24);

                            }
                            ObjCoverOpt.InsurancePlanID = Convert.ToInt64(model.PlanID);
                            Coverageoption = true;
                            Result1 = OpenEnroll.SaveCostOption(ObjCoverOpt);
                            // model.CovID = Result1;
                        }

                        if (model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Medical || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Dental || model.PlanCategoryID == (int)CategoryEnum.PlanCategory.Vision)
                        {
                            var SpouseSurchargePopup = OpenEnroll.SpouseSurchargePopup(model.EmpID, model.OpnEnrollmentId);
                            var SpouseHasCoverage = OpenEnroll.GetEmployeeSpouseHasCoverage(model.EmpID);
                            var DependentData = OpenEnroll.GetSelfandDependentsbyIDs(model.DependentIDs);
                            if (DependentData.Count > 0)
                            {
                                int conspouse = DependentData.Where(a => a.Dependent.Contains("Spouse")).ToList().Count();
                                checkdepSpouse = (conspouse > 0 ? true : false);
                            }
                            var covDEtails = OpenEnroll.GetCovCostByOpenEnrollment(Convert.ToInt64(covID));

                            string strCompanyID = string.Empty;
                            EMG_EmployeeCoverage ObjCoverOpt = new EMG_EmployeeCoverage();

                            if (SpouseSurchargePopup == true && SpouseHasCoverage == true && checkdepSpouse == true)
                            {
                                SpouseSurcharge = true;
                                if (covDEtails.CoverageOption == EnumString.Employee_Spouse || covDEtails.CoverageOption == EnumString.Employee_1 || covDEtails.CoverageOption == EnumString.Family)
                                {
                                    string NewSurchargeOptions = string.Empty;
                                    NewSurchargeOptions = (covDEtails.CoverageOption == EnumString.Family ? EnumString.Family_Surcharge : covDEtails.CoverageOption == EnumString.Employee_1 ? EnumString.E1_Surcharge : covDEtails.CoverageOption == EnumString.Employee_Spouse ? EnumString.ES_Surcharge : "");
                                    var Editrecord1 = OpenEnroll.GetCostNewSurcharge(Convert.ToInt64(covDEtails.InsurancePlanID), NewSurchargeOptions, Convert.ToInt64(covDEtails.SalaryBandId));
                                    if (Editrecord1 != null)
                                    {
                                        var detailsdataSurcharge = OpenEnroll.GetCostByOpenEnrollmentSurcharge(Editrecord1.ID);
                                        if (detailsdataSurcharge != null)
                                        {
                                            ObjCoverOpt.MonthlyCost = detailsdataSurcharge.MonthlyCost;
                                            ObjCoverOpt.WeeklyCost = detailsdataSurcharge.WeeklyCost;
                                            ObjCoverOpt.BiWeekly = detailsdataSurcharge.BiWeekly;
                                            ObjCoverOpt.SemiMonthlyCost = detailsdataSurcharge.SemiMonthlyCost;
                                        }
                                        else
                                        {
                                            ObjCoverOpt.MonthlyCost = (covDEtails.MonthlyCost == null ? Convert.ToDecimal("0.00") : covDEtails.MonthlyCost);
                                            ObjCoverOpt.WeeklyCost = (covDEtails.WeeklyCost == null ? Convert.ToDecimal("0.00") : covDEtails.WeeklyCost);
                                            ObjCoverOpt.BiWeekly = (covDEtails.BiWeekly == null ? Convert.ToDecimal("0.00") : covDEtails.BiWeekly);
                                            ObjCoverOpt.SemiMonthlyCost = (covDEtails.SemiMonthlyCost == null ? Convert.ToDecimal("0.00") : covDEtails.SemiMonthlyCost);
                                        }
                                    }
                                    else
                                    {
                                        ObjCoverOpt.MonthlyCost = (covDEtails.MonthlyCost == null ? Convert.ToDecimal("0.00") : covDEtails.MonthlyCost);
                                        ObjCoverOpt.WeeklyCost = (covDEtails.WeeklyCost == null ? Convert.ToDecimal("0.00") : covDEtails.WeeklyCost);
                                        ObjCoverOpt.BiWeekly = (covDEtails.BiWeekly == null ? Convert.ToDecimal("0.00") : covDEtails.BiWeekly);
                                        ObjCoverOpt.SemiMonthlyCost = (covDEtails.SemiMonthlyCost == null ? Convert.ToDecimal("0.00") : covDEtails.SemiMonthlyCost);
                                    }
                                }
                                else
                                {
                                    ObjCoverOpt.MonthlyCost = (covDEtails.MonthlyCost == null ? Convert.ToDecimal("0.00") : covDEtails.MonthlyCost);
                                    ObjCoverOpt.WeeklyCost = (covDEtails.WeeklyCost == null ? Convert.ToDecimal("0.00") : covDEtails.WeeklyCost);
                                    ObjCoverOpt.BiWeekly = (covDEtails.BiWeekly == null ? Convert.ToDecimal("0.00") : covDEtails.BiWeekly);
                                    ObjCoverOpt.SemiMonthlyCost = (covDEtails.SemiMonthlyCost == null ? Convert.ToDecimal("0.00") : covDEtails.SemiMonthlyCost);
                                }

                            }
                            else
                            {
                                ObjCoverOpt.MonthlyCost = (covDEtails.MonthlyCost == null ? Convert.ToDecimal("0.00") : covDEtails.MonthlyCost);
                                ObjCoverOpt.WeeklyCost = (covDEtails.WeeklyCost == null ? Convert.ToDecimal("0.00") : covDEtails.WeeklyCost);
                                ObjCoverOpt.BiWeekly = (covDEtails.BiWeekly == null ? Convert.ToDecimal("0.00") : covDEtails.BiWeekly);
                                ObjCoverOpt.SemiMonthlyCost = (covDEtails.SemiMonthlyCost == null ? Convert.ToDecimal("0.00") : covDEtails.SemiMonthlyCost);
                            }

                            ObjCoverOpt.EmployeeID = model.EmpID;
                            ObjCoverOpt.OpenEnrollID = model.OpnEnrollmentId;
                            ObjCoverOpt.Type = "OE";
                            ObjCoverOpt.TotalRate = covDEtails.TotalRate;
                            ObjCoverOpt.EmpContibution = covDEtails.EmpContibution;
                            ObjCoverOpt.InsurancePlanID = Convert.ToInt64(model.PlanID);
                            Coverageoption = true;
                            ObjCoverOpt.CreatedBy = Convert.ToInt32(model.EmpID);
                            ObjCoverOpt.ModifiedBy = Convert.ToInt32(model.EmpID);
                            ObjCoverOpt.CheckUser = "Employee";
                            ObjCoverOpt.CoverageOption = covDEtails.CoverageOption;
                            Result1 = OpenEnroll.SaveCostOption(ObjCoverOpt);
                        }
                        result = OpenEnroll.SelectBenefitsOnEmployee(model.EmpID, model.DependentIDs, model.PlanID, model.OpnEnrollmentId, model.Type, Convert.ToInt64(model.PlanCategoryID), Convert.ToInt64(Result1), Convert.ToInt64(covID), Convert.ToDecimal(model.Slidselectedval), model.Spval, model.childval, Coverageoption, model.flatdays, model.ImputedIncome, Convert.ToInt32(model.EmpID), "Employee", model.SpouseFlatDays, model.SpouseSelectedRange);
                    }

                    if (result != -1)
                    {
                        objStatus.SuccessMessage = true;
                        objStatus.Message = "Record Successfully updated";
                    }
                    else
                    {
                        objStatus.SuccessMessage = false;
                        objStatus.Message = "Error Occurred";
                    }

                }

                var OpenEnrollSelection = OpenEnroll.GetMonthlyCostMySelection(model.EmpID, model.OpnEnrollmentId, model.Type);
                if ((!object.Equals(OpenEnrollSelection, null)) && (OpenEnrollSelection.Count() > 0))
                {
                    var TotalSum = OpenEnrollSelection.Where(v => v.IsActive == true).Sum(item => item.MonthlyCost);
                    if (OpenEnrollSelection.Count > 0)
                    {
                        objStatus.Paypercost = Convert.ToString(TotalSum);
                        objStatus.TotalSum = TotalSum;
                    }
                }

                //string Paypercost = GetPayperCost(model.EmpID, model.OpnEnrollmentId, model.Type);
                //objStatus.Paypercost = Paypercost;
                var OpenEnrollCatSel = OpenEnroll.GetOpenEnrollMentCategoryMySelection(model.EmpID, model.OpnEnrollmentId, model.Type);
                var PlanSelected = OpenEnrollCatSel.Where(a => a.IsActive == true).ToList();
                if (PlanSelected.Count > 0)
                {
                    objStatus.PlanCount = PlanSelected.Count;
                }
                else
                {
                    objStatus.PlanCount = 0;
                }
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }
        }
        //public string GetPayperCost(long EmpID, long OpnEnrollmentId, string Type)
        //{
        //    Status objStatus = new Status();
        //    long? planid = 0;
        //    long? DepCovId = 0;
        //    SelectedPlan obj = new SelectedPlan();
        //    int i = 0;
        //    List<SelectedPlan> list = new List<SelectedPlan>();

        //    var OpenEnrollSelection = OpenEnroll.GetOpenEnrollMentCategoryMySelection(EmpID, OpnEnrollmentId, Type);
        //    decimal? totalcost = 0;

        //    if ((!object.Equals(OpenEnrollSelection, null)) && (OpenEnrollSelection.Count() > 0))
        //    {

        //        foreach (var row in OpenEnrollSelection)
        //        {
        //            if (row.IsActive == true)
        //            {
        //                planid = row.PlanID;

        //                DepCovId = row.DependentCoveragrID;
        //                if (DepCovId == null)
        //                {
        //                    DepCovId = 0;
        //                }
        //                if (row.PlanCategoryID == 1 || row.PlanCategoryID == 2 || row.PlanCategoryID == 3)
        //                {
        //                    var plancost = OpenEnroll.GetCostByOpenEnrollment(Convert.ToInt64(planid), Convert.ToInt64(DepCovId), 0);
        //                    obj = new SelectedPlan();

        //                    if ((!object.Equals(plancost, null)))
        //                    {
        //                        obj.MonthlyCost = plancost.MonthlyCost.ToString();
        //                        obj.SemiMonthly = plancost.SemiMonthlyCost.ToString();
        //                        obj.WeeklyCost = plancost.WeeklyCost.ToString();
        //                        obj.BiWeeklyCost = plancost.BiWeekly.ToString();

        //                        if (OpenEnrollSelection[0].MonthlyBasisID == 1)
        //                        {
        //                            obj.DisplayCost = plancost.WeeklyCost.ToString();

        //                        }
        //                        else if (OpenEnrollSelection[0].MonthlyBasisID == 2)
        //                        {
        //                            obj.DisplayCost = plancost.BiWeekly.ToString();

        //                        }
        //                        else if (OpenEnrollSelection[0].MonthlyBasisID == 3)
        //                        {
        //                            obj.DisplayCost = plancost.SemiMonthlyCost.ToString();

        //                        }
        //                        else if (OpenEnrollSelection[0].MonthlyBasisID == 4)
        //                        {
        //                            obj.DisplayCost = plancost.MonthlyCost.ToString();

        //                        }
        //                        else
        //                        {
        //                            obj.DisplayCost = "0.00";
        //                            totalcost = 0;
        //                        }
        //                        totalcost += Convert.ToDecimal(obj.DisplayCost);
        //                    }

        //                }
        //                else
        //                {
        //                    var plancost = OpenEnroll.GetCostByOpenEnrollmentandEmpID(Convert.ToInt64(planid), 1, row.OpenEnrollID, EmpID, row.Type);
        //                    obj = new SelectedPlan();

        //                    if ((!object.Equals(plancost, null)))
        //                    {
        //                        obj.MonthlyCost = plancost.MonthlyCost.ToString();
        //                        obj.SemiMonthly = plancost.SemiMonthlyCost.ToString();
        //                        obj.WeeklyCost = plancost.WeeklyCost.ToString();
        //                        obj.BiWeeklyCost = plancost.BiWeekly.ToString();

        //                        if (OpenEnrollSelection[0].MonthlyBasisID == 1)
        //                        {
        //                            obj.DisplayCost = plancost.WeeklyCost.ToString();

        //                        }
        //                        else if (OpenEnrollSelection[0].MonthlyBasisID == 2)
        //                        {
        //                            obj.DisplayCost = plancost.BiWeekly.ToString();

        //                        }
        //                        else if (OpenEnrollSelection[0].MonthlyBasisID == 3)
        //                        {
        //                            obj.DisplayCost = plancost.SemiMonthlyCost.ToString();

        //                        }
        //                        else if (OpenEnrollSelection[0].MonthlyBasisID == 4)
        //                        {
        //                            obj.DisplayCost = plancost.MonthlyCost.ToString();

        //                        }
        //                        else
        //                        {
        //                            obj.DisplayCost = "0.00";
        //                            totalcost = 0;
        //                        }
        //                        totalcost += Convert.ToDecimal(obj.DisplayCost);
        //                    }
        //                }

        //                list.Add(obj);
        //                i = i + 1;
        //            }
        //        }

        //        obj.DisplayCost = Math.Round(Convert.ToDecimal(totalcost), 2).ToString();


        //    }
        //    else
        //    {
        //        totalcost = 0;

        //    }

        //    return obj.DisplayCost;
        //}
        #region "Plan Details"
        //Plan Details API
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage PlanDetails(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                var PlanData = OpenEnroll.GetPlanDetailsByPlanID(Convert.ToInt64(model.OpnEnrollmentId), Convert.ToInt64(model.PlanCategoryID), model.Type, Convert.ToInt64(model.PlanID), Convert.ToInt32(model.Flag));
                objStatus.PlanInfo = PlanData;
                objStatus.Message = "Plan Details";
                objStatus.MessageStatus = true;
                List<CostDetails> ListCostDetails = new List<CostDetails>();
                List<psp_GetInsPlanResourse_Result> ListPlanResourse = new List<psp_GetInsPlanResourse_Result>();
                List<Proc_GetPlanDetails_Result> planInfo = new List<Proc_GetPlanDetails_Result>();
                var groups = new PowerIBrokerBusinessLayer.Company.OpenEnrollment().GetPlanDetails(0, model.PlanID.ToString(), 4).OrderBy(a => a.SortOrder).ThenBy(a => a.PropertyGroupName).ToList().ToList();
                foreach (var item in groups.ToList())
                {
                    Proc_GetPlanDetails_Result grpName = new Proc_GetPlanDetails_Result();
                    grpName.GroupId = (item.GroupId == null ? 0 : item.GroupId);
                    grpName.PropertyName = item.PropertyGroupName;
                    planInfo.Add(grpName);
                    string groupname = item.PropertyGroupName;
                    var Prodata = new PowerIBrokerBusinessLayer.Company.OpenEnrollment().GetPlanDetails(0, model.PlanID.ToString(), 3, Convert.ToInt32(item.GroupId)).ToList();
                    foreach (var prop in Prodata.ToList())
                    {
                        int y = 0;
                        var Data = new PowerIBrokerBusinessLayer.Company.OpenEnrollment().GetPlanDetails(0, model.PlanID.ToString(), 1, Convert.ToInt32(item.GroupId)).ToList();
                        foreach (var data in Data.ToList())
                        {
                            if (data.PropertyId == prop.PropertyId)
                            {
                                Proc_GetPlanDetails_Result record = new Proc_GetPlanDetails_Result();
                                record.PropertyGroupName = groupname;
                                record.GroupId = 0;
                                record.PropertyName = (y == 0 ? prop.PropertyName : "");
                                record.DependentCoverage = (data.DependentCoverage == "N/A" ? "" : data.DependentCoverage);
                                record.InNetwork = data.InNetwork;
                                record.OutNetwork = data.OutNetwork;
                                record.InNetworkValue = data.InNetworkValue;
                                record.OutNetworkValue = data.OutNetworkValue;
                                planInfo.Add(record);
                                y++;
                            }

                        }

                    }

                    objStatus.PlanDetails = planInfo;
                }
                if (planInfo.Count == 0)
                {
                    planInfo = new List<Proc_GetPlanDetails_Result>();
                    objStatus.PlanDetails = planInfo;
                }
                for (int i = 0; i < PlanData.Count; i++)
                {
                    //Plan Coverage Option
                    var PlanCost = OpenEnroll.GetPlanCostByPlanID(PlanData[i].ID, Convert.ToInt64(model.OpnEnrollmentId), Convert.ToInt32(model.EmpID));

                    for (int p = 0; p < PlanCost.Count; p++)
                    {
                        ListCostDetails.Add(PlanCost[p]);
                    }


                    //Plan Resourses Details
                    var PlanResource = OpenEnroll.GetPlanResourceByPlanID(PlanData[i].ID);

                    for (int y = 0; y < PlanResource.Count; y++)
                    {
                        //~/Areas/Company/Content/Upload/PlanDocs/
                        if (!string.IsNullOrEmpty(PlanResource[y].DocMapName))
                        {
                            PlanResource[y].FullName = "/Areas/Company/Content/Upload/PlanDocs/" + PlanResource[y].FullName;

                        }
                        ListPlanResourse.Add(PlanResource[y]);
                    }

                }

                objStatus.PlanCostDetails = ListCostDetails;
                objStatus.PlanResourse = ListPlanResourse;
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        #endregion
        #region "Plan Comparision"


        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage PlanComparisonEmgApiModels(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                string PlanID = model.StrPlanID;
                var planName = OpenEnroll.GetInsurancePlanName(PlanID);
                objStatus.MessageStatus = true;
                objStatus.Message = "Plan Comparison";
                objStatus.Plans = planName;
                var GroupMaster = new PowerIBrokerBusinessLayer.Company.OpenEnrollment().GetPlanDetails(0, PlanID, 4).OrderBy(a => a.SortOrder).ThenBy(a => a.PropertyGroupName).ToList().ToList();
                List<Proc_GetPlanDetails_Result> planInfo = new List<Proc_GetPlanDetails_Result>();
                List<Proc_GetPlanDetails_Result> planDetails = new List<Proc_GetPlanDetails_Result>();
                if (GroupMaster != null)
                {
                    if (GroupMaster.Count > 0)
                    {

                        //string planids = "";
                        foreach (var item in GroupMaster.ToList())
                        {
                            Proc_GetPlanDetails_Result grpName = new Proc_GetPlanDetails_Result();
                            grpName.GroupId = (item.GroupId == null ? 0 : item.GroupId);
                            grpName.PropertyName = item.PropertyGroupName;
                            planInfo.Add(grpName);
                            planDetails.Add(grpName);
                            var Prodata = new PowerIBrokerBusinessLayer.Company.OpenEnrollment().GetPlanDetails(0, PlanID, 5, Convert.ToInt32(item.GroupId));
                            foreach (var prop in Prodata)
                            {
                                var Data = new PowerIBrokerBusinessLayer.Company.OpenEnrollment().GetPlanDetails(0, PlanID, 6, Convert.ToInt32(item.GroupId), prop.PropertyId);
                                int i = 0;
                                foreach (var data in Data)
                                {
                                    Proc_GetPlanDetails_Result record = new Proc_GetPlanDetails_Result();
                                    record.PropertyGroupName = "";
                                    record.GroupId = 0;
                                    record.PropertyName = (i == 0 ? prop.PropertyName : "");
                                    record.PropertyId = prop.PropertyId;
                                    record.DependentCoverage = (data.DependentCoverage == "N/A" ? "" : data.DependentCoverage);
                                    record.DependentCoverageID = data.DependentCoverageID;

                                    var details = new PowerIBrokerBusinessLayer.Company.OpenEnrollment().GetPlanDetails(0, planName[0].ID.ToString(), 7, Convert.ToInt32(data.GroupId), data.PropertyId, data.DependentCoverageID).FirstOrDefault();
                                    var details1 = new PowerIBrokerBusinessLayer.Company.OpenEnrollment().GetPlanDetails(0, planName[1].ID.ToString(), 7, Convert.ToInt32(data.GroupId), data.PropertyId, data.DependentCoverageID).FirstOrDefault();

                                    if (details == null)
                                    {
                                        details = new Proc_GetPlanDetails_Result();
                                    }
                                    if (details1 == null)
                                    {
                                        details1 = new Proc_GetPlanDetails_Result();
                                    }
                                    details.PropertyGroupName = item.PropertyGroupName;
                                    details.GroupId = item.GroupId;
                                    details.PropertyName = (i == 0 ? prop.PropertyName : "");
                                    details.PropertyId = prop.PropertyId;
                                    details.DependentCoverage = (data.DependentCoverage == "N/A" ? "" : data.DependentCoverage);
                                    details.DependentCoverageID = data.DependentCoverageID;
                                    if (model.Flag == 0)
                                    {
                                        details.InNetwork = details.InNetwork;
                                        details.OutNetwork = details1.InNetwork;
                                        details.InNetworkValue = Convert.ToString(details.InNetworkValue);
                                        details.OutNetworkValue = Convert.ToString(details1.InNetworkValue);
                                    }
                                    else
                                    {
                                        details.InNetwork = details.OutNetwork;
                                        details.OutNetwork = details1.OutNetwork;
                                        details.InNetworkValue = Convert.ToString(details.OutNetworkValue);
                                        details.OutNetworkValue = Convert.ToString(details1.OutNetworkValue);
                                    }

                                    planDetails.Add(details);
                                    planInfo.Add(record);
                                    i++;
                                }
                            }

                        }

                    }
                    if (planInfo.Count == 0)
                    {
                        planInfo = new List<Proc_GetPlanDetails_Result>();
                    }
                    if (planDetails.Count == 0)
                    {
                        planDetails = new List<Proc_GetPlanDetails_Result>();
                    }
                    objStatus.PropertyDetails = planInfo;
                    objStatus.NetworkDetails = planDetails;
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        #endregion
        //Get Plan Details by category
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetReviewInfo(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                long? CovId = 0;
                ReviewPlanList obj = null;
                ReviewPlanList objPendingEmp = null;
                ReviewPlanList objPendingSpouse = null;
                AddBeneficiary objbenf = new AddBeneficiary();
                int i = 0;
                List<ReviewPlanList> list = new List<ReviewPlanList>();
                List<ReviewPlanList> listPending = new List<ReviewPlanList>();
                var SelectedPlan = OpenEnroll.GetInsuranceBenefitsByEmployeeID(model.EmpID, model.OpnEnrollmentId, model.Type);
                List<DependentsViewBenefits> depnCat = new List<DependentsViewBenefits>();
                var compnyID = OpenEnroll.GetCompanyID(model.EmpID);
                var OpenEnrollData = OpenEnroll.GetOpenEnrollMent(model.EmpID, compnyID);
                var GetEnrollType = OpenEnrollData.Where(a => a.ID == model.OpnEnrollmentId).FirstOrDefault();
                var EnrollType = "OE";
                if (GetEnrollType != null)
                {
                    EnrollType = GetEnrollType.EnrollType.ToUpper();
                }
                long EMP_GranteedIssueAmt = 0;
                long Spouse_GranteedIssueAmt = 0;
                string PendingMedicalEmp = string.Empty;
                string PendingMedicalSpouse = string.Empty;
                if (SelectedPlan != null)
                {
                    if (SelectedPlan.Count > 0)
                    {
                        foreach (var benefitsPending in SelectedPlan.Where(a => a.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife).ToList())
                        {
                            var dataPending = OpenEnroll.GetMinMaxByInsurancePlanID(Convert.ToInt64(benefitsPending.PlanID));
                            long EmpSeletedBenefit = Convert.ToInt64(benefitsPending.SelectedRange);
                            long SpouseSeletedBenefit = Convert.ToInt64(benefitsPending.SpouseSelectedRange);
                            if (dataPending != null)
                            {
                                EMP_GranteedIssueAmt = (EnrollType == "LE" ? Convert.ToInt64(dataPending.PendingMedicalLE) : EnrollType == "NH" ? Convert.ToInt64(dataPending.PendingMedicalNH) : Convert.ToInt64(dataPending.PendingMedical));
                                Spouse_GranteedIssueAmt = (EnrollType == "LE" ? Convert.ToInt64(dataPending.SIssueAmountLE) : EnrollType == "NH" ? Convert.ToInt64(dataPending.SIssueAmountNH) : Convert.ToInt64(dataPending.SIssueAmount));
                            }
                            if (EmpSeletedBenefit > EMP_GranteedIssueAmt)
                            {
                                PendingMedicalEmp = "Yes";
                            }
                            else
                            {
                                PendingMedicalEmp = "No";
                            }
                            if (SpouseSeletedBenefit > Spouse_GranteedIssueAmt)
                            {
                                PendingMedicalSpouse = "Yes";
                            }
                            else
                            {
                                PendingMedicalSpouse = "No";
                            }
                            if (PendingMedicalEmp == "Yes" || PendingMedicalSpouse == "Yes")
                            {

                                if (SelectedPlan.Where(a => a.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife && a.EOIStatus.ToUpper() == "P" || a.EOIStatus.ToUpper() == "D").Any())
                                {
                                    var InsuranceBenefitsPendingBenefits = SelectedPlan.Where(a => a.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife && a.EOIStatus.ToUpper() == "P" || a.EOIStatus.ToUpper() == "D").ToList();
                                    foreach (var benefits in InsuranceBenefitsPendingBenefits)
                                    {
                                        objPendingEmp = new ReviewPlanList();
                                        objPendingSpouse = new ReviewPlanList();
                                        var details = OpenEnroll.GetCovCostByOpenEnrollmentandEmployeeID(Convert.ToInt64(benefits.CoverageID));
                                        var data = OpenEnroll.GetMinMaxByInsurancePlanID(Convert.ToInt64(benefits.PlanID));
                                        if (PendingMedicalEmp == "Yes")
                                        {
                                            if (benefits.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                                            {
                                                objPendingEmp.PlanCategory = benefits.PlanCategory;
                                                objPendingEmp.Carrier = benefits.Provider;
                                                objPendingEmp.PlanName = benefits.PlanName;
                                                objPendingEmp.Coverage = EnumString.Employee;
                                                objPendingEmp.EffectiveOn = EnumString.Pending;
                                                objPendingEmp.DisplayCost = Convert.ToString((benefits.EmpCostPerPayPeriod == null ? 0 : (benefits.EmpCostPerPayPeriod)));
                                                objPendingEmp.Benefit = Convert.ToString((EmpSeletedBenefit - EMP_GranteedIssueAmt));
                                                listPending.Add(objPendingEmp);
                                            }
                                        }
                                        if (PendingMedicalSpouse == "Yes")
                                        {
                                            if (benefits.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                                            {
                                                objPendingSpouse.PlanCategory = benefits.PlanCategory;
                                                objPendingSpouse.Carrier = benefits.Provider;
                                                objPendingSpouse.PlanName = benefits.PlanName;
                                                objPendingSpouse.Coverage = EnumString.Spouse;
                                                objPendingSpouse.EffectiveOn = EnumString.Pending;
                                                objPendingSpouse.DisplayCost = Convert.ToString((benefits.SpouseCostPerPayPeriod == null ? 0 : (benefits.SpouseCostPerPayPeriod)));
                                                objPendingSpouse.Benefit = Convert.ToString((SpouseSeletedBenefit - Spouse_GranteedIssueAmt));
                                                listPending.Add(objPendingSpouse);
                                            }
                                        }

                                    }
                                    if (listPending.Count > 0)
                                    {
                                        objStatus.Message5 = "Pending Benefits";
                                        objStatus.MessageStatus5 = true;
                                        objStatus.DataList5 = listPending;
                                    }
                                    else
                                    {
                                        objStatus.Message5 = "Pending Benefits";
                                        objStatus.MessageStatus5 = false;
                                        objStatus.DataList5 = "";
                                    }

                                }
                            }
                            else
                            {
                                objStatus.Message5 = "Pending Benefits";
                                objStatus.MessageStatus5 = false;
                                objStatus.DataList5 = "";
                            }
                        }

                        List<DependentsViewBenefits> depnList = new List<DependentsViewBenefits>();
                        foreach (var cat in SelectedPlan)
                        {
                            if (cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.Medical || cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.Dental || cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.Vision)
                            {
                                DependentsViewBenefits singleCat = new DependentsViewBenefits();
                                List<psp_GetVieBenefitsDependentsByEmployeeID_Result> subList = new List<psp_GetVieBenefitsDependentsByEmployeeID_Result>();
                                singleCat.Category = cat.PlanCategory;
                                var Dependentlist = OpenEnroll.GetViewBenefitsDependents(model.EmpID, model.OpnEnrollmentId, model.Type).Where(a => a.PlanCategoryId == cat.PlanCategoryId).ToList();
                                if (Dependentlist != null)
                                {
                                    foreach (var item in Dependentlist)
                                    {
                                        if (!string.IsNullOrEmpty(item.FirstName))
                                        {
                                            psp_GetVieBenefitsDependentsByEmployeeID_Result singleDept = new psp_GetVieBenefitsDependentsByEmployeeID_Result();
                                            singleDept.FirstName = item.FirstName;
                                            singleDept.SSN = item.SSN;
                                            //if (!string.IsNullOrEmpty(item.SSN))
                                            //{
                                            //    cat.FirstName = (!string.IsNullOrEmpty(item.FirstName)) + "(" + OpenEnroll.SSNXXXFormatAPI(item.SSN) + ")";
                                            //}
                                            //else
                                            //{
                                            //    cat.FirstName = item.FirstName;

                                            //}

                                            subList.Add(singleDept);
                                        }

                                    }
                                    singleCat.Dependents = subList;
                                }
                                depnCat.Add(singleCat);
                            }
                            if (cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.Life)
                            {
                                var BeneficiaryDataLife = objbenf.GetBeneficirayDetails(Convert.ToInt64(model.EmpID), Convert.ToInt64(model.OpnEnrollmentId), "OE", null, Convert.ToInt32(cat.PlanCategoryId), 2, 0);
                                if (BeneficiaryDataLife != null && BeneficiaryDataLife.Count > 0)
                                {
                                    objStatus.Message3 = "Beneficiary Life ";
                                    objStatus.MessageStatus3 = true;
                                    objStatus.DataList3 = BeneficiaryDataLife;

                                }
                                else
                                {
                                    objStatus.Message3 = "No Beneficiary Life ";
                                    objStatus.MessageStatus3 = false;
                                    objStatus.DataList3 = "";

                                }

                            }
                            if (cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                            {
                                var BeneficiaryDataVLife = objbenf.GetBeneficirayDetails(Convert.ToInt64(model.EmpID), Convert.ToInt64(model.OpnEnrollmentId), "OE", null, Convert.ToInt32(cat.PlanCategoryId), 2, 0);
                                if (BeneficiaryDataVLife != null && BeneficiaryDataVLife.Count > 0)
                                {
                                    objStatus.Message4 = "Beneficiary Voluntary Life";
                                    objStatus.MessageStatus4 = true;
                                    objStatus.DataList4 = BeneficiaryDataVLife;

                                }
                                else
                                {
                                    objStatus.Message4 = "No Beneficiary Voluntary Life";
                                    objStatus.MessageStatus4 = false;
                                    objStatus.DataList4 = "";

                                }
                            }
                        }
                    }
                }

                if ((!object.Equals(SelectedPlan, null)) && (SelectedPlan.Count() > 0))
                {
                    objStatus.SuccessMessage = true;
                    objStatus.Message = "No Selected Plan Found";

                    foreach (var row in SelectedPlan)
                    {
                        CovId = row.CoverageID;
                        if (CovId == null)
                        {
                            CovId = 0;
                        }
                        obj = new ReviewPlanList();

                        if ((!object.Equals(SelectedPlan, null)))
                        {
                            obj.PlanCategory = (SelectedPlan[i].PlanCategory != null ? SelectedPlan[i].PlanCategory.ToString() : "");
                            obj.PlanName = (SelectedPlan[i].PlanName != null ? SelectedPlan[i].PlanName.ToString() : "");
                            obj.Carrier = (SelectedPlan[i].Provider != null ? SelectedPlan[i].Provider.ToString() : "");
                            obj.Coverage = (SelectedPlan[i].CoverageOption != null ? SelectedPlan[i].CoverageOption.ToString() : "");
                            obj.DeptName = (SelectedPlan[i].FirstName != null ? SelectedPlan[i].FirstName.ToString() : "");
                            obj.SelectedRange = (SelectedPlan[i].SelectedRange != null ? SelectedPlan[i].SelectedRange.ToString() : "");
                            obj.SpouseSelectedRange = (SelectedPlan[i].SpouseSelectedRange != null ? SelectedPlan[i].SpouseSelectedRange.ToString() : "");
                            obj.EmpCostPerPayPeriod = (SelectedPlan[i].EmpCostPerPayPeriod != null ? SelectedPlan[i].EmpCostPerPayPeriod.ToString() : "");
                            obj.SpouseCostPerPayPeriod = (SelectedPlan[i].SpouseCostPerPayPeriod != null ? SelectedPlan[i].SpouseCostPerPayPeriod.ToString() : "");
                            obj.EmpCostPerPayPeriodEnrollType = (SelectedPlan[i].EmpCostPerPayPeriodEnrollType != null ? SelectedPlan[i].EmpCostPerPayPeriodEnrollType.ToString() : "");
                            obj.SpouseCostPerPayPeriodEnrollType = (SelectedPlan[i].SpouseCostPerPayPeriodEnrollType != null ? SelectedPlan[i].SpouseCostPerPayPeriodEnrollType.ToString() : "");
                            obj.ChildCostPerPayPeriod = (SelectedPlan[i].ChildCostPerPayPeriod != null ? SelectedPlan[i].ChildCostPerPayPeriod.ToString() : "");
                            obj.ChildCost = (SelectedPlan[i].ChildCost != null ? SelectedPlan[i].ChildCost.ToString() : "");
                            obj.EOIStatus = (SelectedPlan[i].EOIStatus != null ? SelectedPlan[i].EOIStatus.ToString() : "");
                            obj.EffectiveOn = (SelectedPlan[i].StartDate != null ? SelectedPlan[i].StartDate.ToString() : "");

                            if (row.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                            {
                                var dataPending = OpenEnroll.GetMinMaxByInsurancePlanID(Convert.ToInt64(row.PlanID));
                                if (dataPending != null)
                                {
                                    EMP_GranteedIssueAmt = (EnrollType == "LE" ? Convert.ToInt64(dataPending.PendingMedicalLE) : EnrollType == "NH" ? Convert.ToInt64(dataPending.PendingMedicalNH) : Convert.ToInt64(dataPending.PendingMedical));
                                    Spouse_GranteedIssueAmt = (EnrollType == "LE" ? Convert.ToInt64(dataPending.SIssueAmountLE) : EnrollType == "NH" ? Convert.ToInt64(dataPending.SIssueAmountNH) : Convert.ToInt64(dataPending.SIssueAmount));
                                    obj.EMP_GranteedIssueAmt = EMP_GranteedIssueAmt;
                                    obj.Spouse_GranteedIssueAmt = Spouse_GranteedIssueAmt;
                                    obj.ChildAmount = Convert.ToInt64(dataPending.ChildAmount);



                                }
                            }

                            if (row.PlanCategoryId == (int)CategoryEnum.PlanCategory.Medical || row.PlanCategoryId == (int)CategoryEnum.PlanCategory.Dental || row.PlanCategoryId == (int)CategoryEnum.PlanCategory.Vision)
                            {
                                var plancost = OpenEnroll.GetCovCostByOpenEnrollmentandEmployeeID(Convert.ToInt64(row.CoverageID));
                                if ((!object.Equals(plancost, null)))
                                {
                                    obj.MonthlyCost = (plancost.MonthlyCost != null ? plancost.MonthlyCost.ToString() : "");
                                    obj.SemiMonthly = (plancost.SemiMonthlyCost != null ? plancost.SemiMonthlyCost.ToString() : "");
                                    obj.WeeklyCost = (plancost.WeeklyCost != null ? plancost.WeeklyCost.ToString() : "");
                                    obj.BiWeeklyCost = (plancost.BiWeekly != null ? plancost.BiWeekly.ToString() : "");

                                    if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.WeeklyCost)
                                    {
                                        obj.DisplayCost = plancost.WeeklyCost.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.BiWeekly)
                                    {
                                        obj.DisplayCost = plancost.BiWeekly.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.SemiMonthlyCost)
                                    {
                                        obj.DisplayCost = plancost.SemiMonthlyCost.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.MonthlyCost)
                                    {
                                        obj.DisplayCost = plancost.MonthlyCost.ToString();
                                    }
                                    else
                                    {
                                        obj.DisplayCost = "0.00";
                                    }

                                }
                                else
                                {
                                    obj.MonthlyCost = "0.00";
                                    obj.SemiMonthly = "0.00";
                                    obj.WeeklyCost = "0.00";
                                    obj.BiWeeklyCost = "0.00";
                                    obj.DisplayCost = "0.00";
                                }
                            }
                            else
                            {
                                var plancost = OpenEnroll.GetCovCostByOpenEnrollmentandEmployeeID(Convert.ToInt64(row.CoverageID));
                                if ((!object.Equals(plancost, null)))
                                {
                                    obj.MonthlyCost = (plancost.MonthlyCost != null ? plancost.MonthlyCost.ToString() : "");
                                    obj.SemiMonthly = (plancost.SemiMonthlyCost != null ? plancost.SemiMonthlyCost.ToString() : "");
                                    obj.WeeklyCost = (plancost.WeeklyCost != null ? plancost.WeeklyCost.ToString() : "");
                                    obj.BiWeeklyCost = (plancost.BiWeekly != null ? plancost.BiWeekly.ToString() : "");

                                    if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.WeeklyCost)
                                    {
                                        obj.DisplayCost = plancost.WeeklyCost.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.BiWeekly)
                                    {
                                        obj.DisplayCost = plancost.BiWeekly.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.SemiMonthlyCost)
                                    {
                                        obj.DisplayCost = plancost.SemiMonthlyCost.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.MonthlyCost)
                                    {
                                        obj.DisplayCost = plancost.MonthlyCost.ToString();
                                    }
                                    else
                                    {
                                        obj.DisplayCost = "0.00";
                                    }

                                }
                                else
                                {
                                    obj.MonthlyCost = "0.00";
                                    obj.SemiMonthly = "0.00";
                                    obj.WeeklyCost = "0.00";
                                    obj.BiWeeklyCost = "0.00";
                                    obj.DisplayCost = "0.00";
                                }


                            }



                        }

                        list.Add(obj);
                        i = i + 1;
                    }


                    if (list.Count > 0)
                    {
                        objStatus.Message = "Select Plan List";
                        objStatus.MessageStatus = true;
                        objStatus.DataList = list;
                        if (depnCat.Count == 0)
                        {
                            objStatus.DataList2 = new List<DependentsViewBenefits>();
                        }
                        else
                        {
                            objStatus.DataList2 = depnCat;
                        }

                    }
                    else
                    {
                        objStatus.Message = "No Selected Plan List Found";
                        objStatus.DataList = new List<ReviewPlanList>();
                        objStatus.MessageStatus = false;
                        objStatus.DataList2 = new List<DependentsViewBenefits>();
                    }




                }
                else
                {
                    objStatus.SuccessMessage = false;
                    objStatus.Message = "No Selected Plan Found";
                    objStatus.DataList2 = new List<DependentsViewBenefits>();

                }
                var OpenEnrollWavCat = OpenEnroll.GetOpenEnrollMentWaivedCategory(model.EmpID, model.OpnEnrollmentId, model.Type);
                if ((!object.Equals(OpenEnrollWavCat, null)) && (OpenEnrollWavCat.Count() > 0))
                {
                    objStatus.Message1 = "Waived Plan List";
                    objStatus.MessageStatus1 = true;
                    objStatus.DataList1 = OpenEnrollWavCat;
                }
                else
                {
                    objStatus.Message1 = "No Waived Plan List Found";
                    objStatus.MessageStatus1 = false;
                }


            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]

        public HttpResponseMessage FinalSubmission(EmgApiModels modelapi)
        {

            if (ModelState.IsValid)
            {
                int result = 0;
                Comman objComman = new Comman();
                try
                {
                    bool IsEoiSkipped = false;
                    string Password = OpenEnroll.GetPassword(modelapi.EmpID);
                    string firstName = OpenEnroll.GetEmployees(modelapi.EmpID).FirstName;
                    string LastName = OpenEnroll.GetEmployees(modelapi.EmpID).LastName;
                    string ContactNo = OpenEnroll.GetEmployeesPersonalInfo(modelapi.EmpID).Phone;
                    IsEoiSkipped = OpenEnroll.GetEmployees(modelapi.EmpID).IsEOISkiped;
                    if (!string.IsNullOrEmpty(ContactNo))
                    {
                        ContactNo = objComman.RemoveMasking(ContactNo);
                    }
                    else
                    {
                        ContactNo = "";
                    }


                    long CompanyId = OpenEnroll.GetCompanyID(modelapi.EmpID); ;
                    //string CompanyAdminPassword = OpenEnroll.GetPassword(CompanyAdminId);     
                    string CompanyAdminPassword = string.Empty;
                    string CmpnyAdminPWD = string.Empty;
                    string AdminPassword = OpenEnroll.GetPasswordAdmin(1);
                    List<tblCompany_Employee_BasicInfo> objCompAdminPwd = new List<tblCompany_Employee_BasicInfo>();
                    using (PowerIBrokerEntities context = new PowerIBrokerEntities())
                    {
                        objCompAdminPwd = context.tblCompany_Employee_BasicInfo.Where(a => a.CompanyId == CompanyId && a.RoleID == 1 && a.IsActive == true && a.IsTerminate == false).ToList();
                    }

                    foreach (var item in objCompAdminPwd)
                    {
                        CompanyAdminPassword = item.Password;
                        CmpnyAdminPWD = CmpnyAdminPWD + ',' + CompanyAdminPassword;

                    }
                    CmpnyAdminPWD = CmpnyAdminPWD.TrimStart(',');

                    if ((Password == (Helper.Encrypt(modelapi.Password))) || AdminPassword == (Helper.Encrypt(modelapi.Password)) || CmpnyAdminPWD.Contains(Helper.Encrypt(modelapi.Password)))
                    {
                        Random r = new Random();
                        int randNum = r.Next(1000000);
                        string sixDigitNumber = randNum.ToString("D6");
                        string UidSession = sixDigitNumber;
                        result = OpenEnroll.FinalSubmission(Convert.ToInt64(modelapi.EmpID), Convert.ToInt64(modelapi.OpnEnrollmentId), "OE", Convert.ToInt32(modelapi.EmpID), "Employee", UidSession);
                        if (result == 1)
                        {
                            OpenEnroll.InsertDataAudit(modelapi.EmpID, modelapi.OpnEnrollmentId, UidSession);
                            using (PowerIBrokerEntities context = new PowerIBrokerEntities())
                            {
                                var details = context.tblCompany_Employee_DependentInfo.Where(p => p.EmployeeId == modelapi.EmpID && p.Dependent.Contains("Spouse") && p.IsActive == true).FirstOrDefault();
                                if (details != null)
                                {
                                    OpenEnroll.UpdateSpouseCoverageFlag(modelapi.EmpID, modelapi.OpnEnrollmentId, UidSession, details.SpouseIsEmployed, details.SpouseHasCoverage);
                                    result = 1;
                                    context.SaveChanges();
                                }
                            }

                            var compnyID = OpenEnroll.GetCompanyID(modelapi.EmpID);
                            var OpenEnrollData = OpenEnroll.GetOpenEnrollMent(modelapi.EmpID, compnyID);
                            var GetEnrollType = OpenEnrollData.Where(a => a.ID == modelapi.OpnEnrollmentId).FirstOrDefault();
                            var CompanyAdminEmail = "";

                            var SubBrokerEmail = "";
                            string BccEmail = string.Empty;
                            if (GetEnrollType.EnrollType.ToUpper() == "NH")
                            {
                                using (PowerIBrokerEntities context = new PowerIBrokerEntities())
                                {
                                    List<tblCompany_Employee_BasicInfo> objCompAdminEmail = new List<tblCompany_Employee_BasicInfo>();
                                    List<BrokerUserAssign> objBrokerUserAssign = new List<BrokerUserAssign>();
                                    objCompAdminEmail = context.tblCompany_Employee_BasicInfo.Where(a => a.CompanyId == compnyID && a.RoleID == 1 && a.IsActive == true && a.IsTerminate == false).ToList();
                                    objBrokerUserAssign = context.BrokerUserAssigns.Where(a => a.CompanyId == compnyID && a.Status == true).ToList();

                                    foreach (var item in objCompAdminEmail)
                                    {
                                        CompanyAdminEmail = item.Email;
                                        BccEmail = BccEmail + ',' + CompanyAdminEmail;

                                    }
                                    foreach (var bua in objBrokerUserAssign)
                                    {
                                        var SubBrokerEmailId = context.tblSubBrokers.Where(a => a.SubBrokerId == bua.SubBrokerId).FirstOrDefault();
                                        SubBrokerEmail = SubBrokerEmailId.Email;
                                        BccEmail = BccEmail + ',' + SubBrokerEmail;


                                    }
                                    BccEmail = BccEmail.TrimStart(',');
                                }
                            }

                            if (GetEnrollType.EnrollType.ToUpper() == "LE" || GetEnrollType.EnrollType.ToUpper() == "OE")
                            {
                                using (PowerIBrokerEntities context = new PowerIBrokerEntities())
                                {
                                    List<tblCompany_Employee_BasicInfo> objCompAdminEmail = new List<tblCompany_Employee_BasicInfo>();
                                    objCompAdminEmail = context.tblCompany_Employee_BasicInfo.Where(a => a.CompanyId == compnyID && a.RoleID == 1 && a.IsActive == true && a.IsTerminate == false).ToList();
                                    foreach (var item in objCompAdminEmail)
                                    {
                                        CompanyAdminEmail = item.Email;
                                        BccEmail = BccEmail + ',' + CompanyAdminEmail;
                                    }
                                    BccEmail = BccEmail.TrimStart(',');
                                }
                            }

                            OpenEnroll.DownloadPdfFinalSubmissionAPI(modelapi.EmpID, modelapi.OpnEnrollmentId, "OE", firstName, GetEnrollType.EnrollType, BccEmail);
                            var companyName = OpenEnroll.GetCompanyNameByCompanyID(compnyID);
                            PowerIBrokerBusinessLayer.DownloadPdf model = new PowerIBrokerBusinessLayer.DownloadPdf();
                            model.BenefitList = OpenEnroll.GetInsuranceBenefitsByEmployeeID(modelapi.EmpID, modelapi.OpnEnrollmentId, "OE");
                            int statusPhone = 0;
                            if (model.BenefitList.Count > 0)
                            {
                                if (GetEnrollType.EnrollType.ToUpper() == "LE")
                                {
                                    if (!string.IsNullOrEmpty(ContactNo))
                                    {
                                                                                
                                        statusPhone = SendTwilloMessage.SendmessageTwillo(ContactNo, "Dear " + firstName + ", \n \n You just submitted a life event and benefist change.  It’s been sent to Human Resources for review. Standby");
                                        if (!string.IsNullOrEmpty(BccEmail))
                                        {
                                            var splitdataEmail = BccEmail.Split(',');
                                            string Emailidcompany = string.Empty;
                                            if (splitdataEmail.Length > 0)
                                            {
                                                for (int i = 0; i < splitdataEmail.Length; i++)
                                                {
                                                    using (PowerIBrokerEntities context = new PowerIBrokerEntities())
                                                    {
                                                        Emailidcompany = splitdataEmail[i];
                                                        var EmpIdCompany = context.tblCompany_Employee_BasicInfo.Where(a => a.Email == Emailidcompany).FirstOrDefault();
                                                        if (EmpIdCompany != null)
                                                        {
                                                            long EmpployeeId = EmpIdCompany.ID;
                                                            var CompanyAdminPhone = context.tblCompany_Employee_PersonalInfo.Where(a => a.EmployeeID == EmpployeeId).FirstOrDefault();
                                                            if (CompanyAdminPhone != null)
                                                            {
                                                                if (!string.IsNullOrEmpty(CompanyAdminPhone.Phone))
                                                                {
                                                                    var PhoneNo = objComman.RemoveMasking(CompanyAdminPhone.Phone);
                                                                    statusPhone = SendTwilloMessage.SendmessageTwillo(PhoneNo, "EMG New Notice - \n \n" + firstName + " completed a Life Event enrollment. It’s waiting in the administrator’s queue for review and approval.");
                                                                }
                                                            }
                                                        }


                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(ContactNo))
                                    {
                                        statusPhone = SendTwilloMessage.SendmessageTwillo(ContactNo, "Dear " + firstName + ", You just completed benefits enrollment. Your benefits summary has been emailed to you or can be viewed at My Benefits on the EMG portal.");
                                    }
                                }

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ContactNo))
                                {
                                    statusPhone = SendTwilloMessage.SendmessageTwillo(ContactNo, "Thanks for letting us know that you won’t need benefits from " + companyName + ". You’ve waived all benefit plans and coverage. If you change your mind, just re-enroll using the EnrollMyGroup App.");
                                }
                            }
                            if (model.BenefitList.Count > 0)
                            {
                                // for vol life  Company Admin text Message
                                foreach (var item in model.BenefitList.Where(m => m.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife))
                                {
                                    if (!string.IsNullOrEmpty(BccEmail))
                                    {
                                        var splitdataEmail = BccEmail.Split(',');
                                        string Emailidcompany = string.Empty;
                                        if (splitdataEmail.Length > 0)
                                        {
                                            for (int i = 0; i < splitdataEmail.Length; i++)
                                            {
                                                using (PowerIBrokerEntities context = new PowerIBrokerEntities())
                                                {
                                                    Emailidcompany = splitdataEmail[i];
                                                    var EmpIdCompany = context.tblCompany_Employee_BasicInfo.Where(a => a.Email == Emailidcompany).FirstOrDefault();
                                                    if (EmpIdCompany != null)
                                                    {
                                                        long EmpployeeId = EmpIdCompany.ID;
                                                        var CompanyAdminPhone = context.tblCompany_Employee_PersonalInfo.Where(a => a.EmployeeID == EmpployeeId).FirstOrDefault();
                                                        if (CompanyAdminPhone != null)
                                                        {
                                                            if (!string.IsNullOrEmpty(CompanyAdminPhone.Phone))
                                                            {
                                                                var PhoneNo = objComman.RemoveMasking(CompanyAdminPhone.Phone);
                                                                if (GetEnrollType.EnrollType.ToUpper() == "LE" || GetEnrollType.EnrollType.ToUpper() == "NH")
                                                                {
                                                                    //  statusPhone = SendTwilloMessage.SendmessageTwillo(PhoneNo, "" + firstName + ", \n \n completed an enrollment needing EOI. It’s waiting in the administrator’s queue for review and approval.\n \n Thanks \n EnrollMyGroup Team");
                                                                    statusPhone = SendTwilloMessage.SendmessageTwillo(PhoneNo, "EMG New Notice - \n \n" + firstName + " completed an enrollment needing EOI. It’s waiting in the administrator’s queue for review and approval.");
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
                        objStatus.MessageStatus = true;
                    }
                    else
                    {
                        objStatus.Message = "Please check Password.";
                        objStatus.MessageStatus = false; //result = -1;// password not matched;
                    }
                }
                catch (Exception ex)
                {
                    objStatus.Message = "Some error occured";
                    objStatus.MessageStatus = false;
                    ObjElog.CustomErrorLog(ex.Message, "EMGAPI Controller", "FinalSubmission", "");
                    ModelState.AddModelError("Employee", ex.Message);
                }
                // objStatus.MessageStatus = true;
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
                return Request.CreateResponse(HttpStatusCode.OK, objStatus);
            }

            //
        }

        #endregion
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetEmployeeLifeEvents(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                int empid = Convert.ToInt32(model.EmpID);
                var GetLifeEvent = OpenEnroll.GetLifeChangeEvents(empid);

                if ((!object.Equals(GetLifeEvent, null)) && (GetLifeEvent.Count() > 0))
                {
                    objStatus.SuccessMessage = true;
                    objStatus.Message = "Employee Life Events";
                    objStatus.DataList = GetLifeEvent;
                }
                else
                {
                    objStatus.SuccessMessage = false;
                    objStatus.Message = "No Life Events found";
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;

            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetLifeEventType(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                var AllLifeEventType = OpenEnroll.GetLifeEvents();


                if ((!object.Equals(AllLifeEventType, null)) && (AllLifeEventType.Count() > 0))
                {
                    objStatus.SuccessMessage = true;
                    objStatus.Message = "All Life Events";
                    objStatus.DataList = AllLifeEventType;

                }
                else
                {
                    objStatus.SuccessMessage = false;
                    objStatus.Message = "No Life Events";

                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage AddNewLifeEvent()
        {
            bool IsDependenetSuccessAdd = true;
            int resultDepMsg = 0;
            EmgApiModels model = new EmgApiModels();
            var httpRequest = HttpContext.Current.Request;
            //string postedImage = httpRequest["Image"];
            model.EmpID = Convert.ToInt64(httpRequest["EmpID"]);
            model.CompanyID = Convert.ToInt64(httpRequest["CompanyID"]);
            model.LEventId = Convert.ToInt64(httpRequest["LEventId"]);
            model.LifeEventDate = Convert.ToDateTime(httpRequest["LifeEventDate"]);
            model.EventID = Convert.ToInt32(httpRequest["EventID"]);
            model.FirstNameDependent = Convert.ToString(httpRequest["FirstNameDependent"]);
            model.MiddleNameDependent = Convert.ToString(httpRequest["MiddleNameDependent"]);
            model.LastNameDependent = Convert.ToString(httpRequest["LastNameDependent"]);
            model.NameTitle = Convert.ToString(httpRequest["NameTitle"]);
            model.SSNDependent = Convert.ToString(httpRequest["SSNDependent"]);
            model.SpouseIsEmployed = (Convert.ToString(httpRequest["SpouseIsEmployed"]) == "1" ? true : false);
            model.SpouseHasCoverage = (Convert.ToString(httpRequest["SpouseHasCoverage"]) == "1" ? true : false);
            model.Dependent = Convert.ToString(httpRequest["Dependent"]);
            model.Gender = Convert.ToString(httpRequest["Gender"]);
            model.IsDisable = (Convert.ToString(httpRequest["IsDisable"]) == "1" ? true : false);
            model.IsSmoker = (Convert.ToString(httpRequest["IsSmoker"]) == "1" ? true : false);
            model.DOBDependent = Convert.ToDateTime(httpRequest["DOBDependent"]);
            model.IsStudent = (Convert.ToString(httpRequest["IsStudent"]) == "1" ? true : false);


            var files = httpRequest.Files;
            HttpPostedFile file = null;

            tblCompany_Employee_DependentInfo dependentInfo = new tblCompany_Employee_DependentInfo();
            if (model.EmpID > 0 && (model.EventID == 1 || model.EventID == 3))
            {
                dependentInfo.EmployeeId = model.EmpID;
                dependentInfo.CompanyId = model.CompanyID;
                dependentInfo.ID = 0;
                dependentInfo.FirstName = model.FirstNameDependent;
                dependentInfo.MiddleName = model.MiddleNameDependent;
                dependentInfo.LastName = model.LastNameDependent;
                dependentInfo.NameTitle = model.NameTitle;
                if (!string.IsNullOrEmpty(model.SSNDependent))
                {
                    dependentInfo.SSN = objComman.RemoveMasking(model.SSNDependent);
                }


                if (model.Dependent == "1")
                {
                    dependentInfo.Dependent = "Spouse";
                    dependentInfo.SpouseIsEmployed = model.SpouseIsEmployed;
                    dependentInfo.SpouseHasCoverage = model.SpouseHasCoverage;

                }
                if (model.Dependent == "2")
                {
                    //dependentInfo.Dependent = "Children";
                    dependentInfo.Dependent = "Child";
                }
                if (model.Dependent == "3")
                {
                    dependentInfo.Dependent = "Other";
                }
                if (model.Dependent == "0")
                {
                    dependentInfo.Dependent = "Other";
                }
                dependentInfo.Gender = model.Gender;
                dependentInfo.IsDisable = model.IsDisable;
                dependentInfo.IsSmoker = model.IsSmoker;
                dependentInfo.DOB = model.DOBDependent;
                dependentInfo.IsStudent = model.IsStudent;
                dependentInfo.IsActive = true;
                dependentInfo.CreatedBy = Convert.ToInt32(model.EmpID);
                dependentInfo.ModifiedBy = Convert.ToInt32(model.EmpID);
                dependentInfo.CheckUser = "Employee";
                int resultDep = OpenEnroll.AddDependents(dependentInfo);
                if (resultDep == 0)
                {
                    objStatus.SuccessMessage = false;
                    objStatus.Message = "You cannot add more than one spouse.";
                    IsDependenetSuccessAdd = false;
                    resultDepMsg = 0;
                }
                else if (resultDep == -5)
                {
                    objStatus.SuccessMessage = false;
                    objStatus.Message = "SSN already exists.";
                    IsDependenetSuccessAdd = false;
                    resultDepMsg = -5;
                }
            }
            if (model.LEventId == 0 && (Convert.ToString(model.LifeEventDate) != "" || model.LifeEventDate != null) && model.EmpID > 0 && model.CompanyID > 0 && model.EventID > 0 && files.Count > 0)
            {
                if (IsDependenetSuccessAdd == true)
                {
                    long result = 0;
                    string fileName = string.Empty;
                    EMG_LifeChangeEvent objdetails = new EMG_LifeChangeEvent();
                    objdetails.ID = model.LEventId;
                    objdetails.IsActive = true;
                    objdetails.EmployeeID = model.EmpID;
                    objdetails.LifeEventID = model.EventID;
                    objdetails.EventDate = Convert.ToDateTime(model.LifeEventDate);
                    objdetails.CompanyID = model.CompanyID;
                    objdetails.Status = "P";
                    objdetails.DateCreated = DateTime.Now;
                    objdetails.CreatedBy = Convert.ToInt32(model.EmpID);
                    objdetails.ModifiedBy = Convert.ToInt32(model.EmpID);
                    objdetails.CheckUser = "Employee";
                    result = OpenEnroll.SaveLifeChangeEvent(objdetails);
                    if (result > 0)
                    {
                        int resultsupport;
                        EMG_LifeChangeEvent_SupportDocument objSupport = new EMG_LifeChangeEvent_SupportDocument();
                        HttpResponseMessage response = new HttpResponseMessage();

                        if (files.Count > 0)
                        {
                            //foreach (HttpPostedFile file in files)
                            //{
                            int index = 0;
                            foreach (string key in httpRequest.Files.AllKeys)
                            {
                                file = httpRequest.Files[index];
                                if (file.ContentLength > 0)
                                {
                                    fileName = DateTime.UtcNow.Ticks + "-" + file.FileName;
                                    var path = HttpContext.Current.Server.MapPath("~/Uploads/SupportDocument/");
                                    if (!Directory.Exists(path))
                                        Directory.CreateDirectory(path);
                                    path = Path.Combine(path, System.IO.Path.GetFileName(fileName));
                                    file.SaveAs(path);
                                    objSupport.LifeChangeEventID = result;
                                    objSupport.CompanyID = model.CompanyID;
                                    objSupport.EmployeeID = model.EmpID;
                                    objSupport.SupportDocument = fileName;
                                    objSupport.DateCreated = DateTime.UtcNow;
                                    objSupport.OrgFileName = file.FileName;
                                    resultsupport = OpenEnroll.SaveLifeChangeEventSupportDocument(objSupport);
                                }
                                index++;
                            }


                        }
                        else
                        {
                            objStatus.SuccessMessage = true;
                            objStatus.Message = "No file was added";
                        }
                        objStatus.SuccessMessage = true;
                        objStatus.Message = "Life Events successfully created";
                    }
                    else
                    {
                        objStatus.SuccessMessage = false;
                        objStatus.Message = "some Error occurred";

                    }
                }
                else
                {
                    if (resultDepMsg == 0)
                    {
                        objStatus.SuccessMessage = false;
                        objStatus.Message = "You cannot add more than one spouse.";

                    }
                    else if (resultDepMsg == -5)
                    {
                        objStatus.SuccessMessage = false;
                        objStatus.Message = "SSN already exists.";

                    }
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpGet]
        public HttpResponseMessage EditBenefitLifeEvents(int ID = 0)
        {
            PowerIBrokerBusinessLayer.Company.LifeEventsDependentValidations details = null;
            try
            {
                var alllyfevents = OpenEnroll.GetLifeEvents();
                System.Web.Mvc.SelectList lyfevents = new System.Web.Mvc.SelectList(alllyfevents, "ID", "LifeEvent");
                //  ViewBag.lifeEvents = lyfevents;
                if (lyfevents != null)
                {
                    objStatus.MessageStatus3 = true;
                    objStatus.Message3 = "All Events List";
                    objStatus.DataList3 = lyfevents;
                }
                else
                {
                    objStatus.MessageStatus3 = false;
                    objStatus.Message3 = "All Events List";
                    objStatus.DataList3 = "";
                }


                if (ID != 0)
                {
                    var supportDoc = OpenEnroll.GetLifeEventsSupportingDoc(ID);


                    if ((!object.Equals(supportDoc, null)) && (supportDoc.Count() > 0))
                    {
                        objStatus.MessageStatus1 = true;
                        objStatus.Message1 = "Support document";
                        objStatus.DataList1 = supportDoc;
                    }
                    else
                    {
                        objStatus.MessageStatus1 = false;
                        objStatus.Message1 = "No Support document";
                        objStatus.DataList1 = "";
                    }

                    details = OpenEnroll.EditLifeChangeEvents(ID);
                    if (!object.Equals(details, null))
                    {
                        objStatus.MessageStatus2 = true;
                        objStatus.Message2 = "Life Event Details";
                        objStatus.DataList2 = details;
                    }
                    else
                    {
                        objStatus.MessageStatus2 = false;
                        objStatus.Message2 = "No Life Event Details";
                        objStatus.DataList2 = "";
                    }
                }
                else
                {
                    objStatus.MessageStatus = false;
                    objStatus.Message = "Please pass the valid input type.";

                }


            }
            catch (Exception ex)
            {
                objStatus.MessageStatus = false;
                objStatus.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage WaiveBenefit(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {
                int result = 0;

                tblEmployee_Benefits benefits = new tblEmployee_Benefits();

                result = OpenEnroll.WaiveBenefitOmEmployee(Convert.ToInt64(model.EmpID), Convert.ToInt64(model.OpnEnrollmentId), Convert.ToInt64(model.PlanCategoryID), model.Type.ToString(), model.WaiveReason.ToString(), Convert.ToInt64(model.WaiveOptionID), Convert.ToInt32(model.EmpID), "Employee");

                if (result > 0)
                {
                    objStatus.SuccessMessage = true;
                    objStatus.Message = "Plan successfully Waived";

                }
                else
                {
                    objStatus.SuccessMessage = false;
                    objStatus.Message = "some Error occurred";

                }
                //string Paypercost = GetPayperCost(model.EmpID, model.OpnEnrollmentId, model.Type);
                //objStatus.Paypercost = Paypercost;
                var OpenEnrollSelection = OpenEnroll.GetMonthlyCostMySelection(model.EmpID, model.OpnEnrollmentId, model.Type);
                if ((!object.Equals(OpenEnrollSelection, null)) && (OpenEnrollSelection.Count() > 0))
                {
                    var TotalSum = OpenEnrollSelection.Where(v => v.IsActive == true).Sum(item => item.MonthlyCost);
                    if (OpenEnrollSelection.Count > 0)
                    {
                        objStatus.Paypercost = Convert.ToString(TotalSum);

                    }
                }

                var OpenEnrollCatSel = OpenEnroll.GetOpenEnrollMentCategoryMySelection(model.EmpID, model.OpnEnrollmentId, model.Type);
                var PlanSelected = OpenEnrollCatSel.Where(a => a.IsActive == true).ToList();
                if (PlanSelected.Count > 0)
                {
                    objStatus.PlanCount = PlanSelected.Count;
                }
                else
                {
                    objStatus.PlanCount = 0;
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);

        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage DeleteNotification(long ID, long EmpID = 0)
        {
            int result = OpenEnroll.DeleteNotification(ID, EmpID);
            if (result == 1)
            {
                objStatus.Message = "Notification Deleted successfully";
                objStatus.MessageStatus = true;
            }
            else
            {
                objStatus.Message = "Some error occurred while processing your request";
                objStatus.MessageStatus = false;

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);


        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage GetAllNotification(long EmpID)
        {
            var notificationList = OpenEnroll.GetAllNotification(EmpID);
            if (notificationList.Count > 0)
            {
                objStatus.DataList = notificationList;
                objStatus.Message = "Notification List Data";
                objStatus.MessageStatus = true;
            }
            else
            {
                objStatus.Message = "No notication is in the list.";
                objStatus.MessageStatus = false;

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);


        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage GetTotalCount(long EmpID)
        {
            var notificationList = OpenEnroll.GetTotalCount(EmpID);
            if (notificationList.Count > 0)
            {
                objStatus.DataList = notificationList;
                objStatus.TotalCount = notificationList.Count;
                objStatus.Message = "Notification List Data";
                objStatus.MessageStatus = true;
            }
            else
            {
                objStatus.Message = "No notication is in the list.";
                objStatus.MessageStatus = false;

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);


        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage UpdateNotificationSetting(long EmpID)
        {
            var result = OpenEnroll.UpdateNotificationSetting(EmpID);
            if (result == 1)
            {
                objStatus.Message = "Notification setting updated.";
                objStatus.MessageStatus = true;
                objStatus.SuccessMessage = true;
            }
            else
            {
                objStatus.Message = "Some error occurred while processing the request. Please try again";
                objStatus.MessageStatus = false;
                objStatus.SuccessMessage = false;

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);


        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage LogOutApi(long EmpID)
        {
            int result = objuser.LogOutApi(EmpID);
            if (result != 0)
            {
                objStatus.Message = "User successfully logout.";
                objStatus.MessageStatus = true;
            }
            else
            {
                objStatus.Message = "Some error occurred while processing the request. Please try again";
                objStatus.MessageStatus = false;

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);


        }
        #region Resource Library
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage ResourceLibrary(long EmpId)
        {
            string strCompanyID = string.Empty;
            long lngCompanyID = 0;
            lngCompanyID = OpenEnroll.GetCompanyID(EmpId);
            var chkPlanRes = OpenEnroll.GetAPIPlanResources(lngCompanyID).GroupBy(x => x.PlanCategoryID).Select(x => x.FirstOrDefault());
            var PlanCategory = new List<EMG_PlanCategory>();
            var details = OpenEnroll.GetPlanCategory();
            PlanCategory = (from e in chkPlanRes
                            select new EMG_PlanCategory
                            {
                                ID = (int)e.PlanCategoryID,
                                PlanCategory = e.PlanCategory

                            }
                ).ToList();

            if (PlanCategory.Count > 0)
            {
                objStatus.DataList = PlanCategory;
                objStatus.Message = "Plan category List.";
                objStatus.MessageStatus = true;

            }
            else
            {
                objStatus.Message = "No Plan category in the list.";
                objStatus.MessageStatus = false;
            }

            var CompanyResourceData = OpenEnroll.GetCompanyResources(lngCompanyID);
            if (CompanyResourceData.Count > 0)
            {//~/Areas/Company/Content/Upload/ResourceLib/
                CompanyResourceData.Select(a => a.DocName = (!string.IsNullOrEmpty(a.DocMapName) ? "/Areas/Company/Content/Upload/ResourceLib/" + (string.IsNullOrEmpty(a.DocName) ? "" : a.DocMapName) : (string.IsNullOrEmpty(a.DocName) ? "" : a.DocMapName))).ToList();
                objStatus.DataList1 = CompanyResourceData;
                objStatus.Message1 = "Company Resource List.";
                objStatus.MessageStatus1 = true;

            }
            else
            {
                objStatus.Message1 = "No Company Resource in the list.";
                objStatus.MessageStatus1 = false;
            }
            var PlanResource = OpenEnroll.GetAPIPlanResources(lngCompanyID);
            if (PlanResource.Count > 0)
            {
                PlanResource.Select(a => a.FullName = (string.IsNullOrEmpty(a.DocMapName) ? a.FullName : "/Areas/Company/Content/Upload/PlanDocs/" + a.FullName)).ToList();
                // PlanResource.Select(a => a.FullName = "/Areas/Company/Content/Upload/PlanDocs/" + (string.IsNullOrEmpty(a.FullName) ? "" : a.FullName)).ToList();
                objStatus.DataList2 = PlanResource;
                objStatus.Message2 = "Plan Resource List.";
                objStatus.MessageStatus2 = true;

            }
            else
            {
                objStatus.Message2 = "No Plan Resource in the list.";
                objStatus.MessageStatus2 = false;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }

        //[System.Web.Http.AcceptVerbs("GET", "POST")]
        //public HttpResponseMessage GetPlanResource(long EmpId, long CatId)
        //{
        //    string strCompanyID = string.Empty;
        //    long lngCompanyID = 0;
        //    lngCompanyID = OpenEnroll.GetCompanyID(EmpId);


        //    var PlanResource = OpenEnroll.GetPlanResources(EmpId, CatId);
        //    if (PlanResource.Count > 0)
        //    {
        //        objStatus.DataList = PlanResource;
        //        objStatus.Message = "Plan Resource List.";
        //        objStatus.MessageStatus = true;

        //    }
        //    else
        //    {
        //        objStatus.Message = "No Plan Resource in the list.";
        //        objStatus.MessageStatus = false;
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        //}


        #endregion
        #region Beneficiary in open Enrollment  Uncomment by avdhesh

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpGet]
        public HttpResponseMessage GetBeneficiaryByCategory(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    GetBeneficiaryData(model);

                }

                catch (Exception ex)
                {
                    ObjElog.CustomErrorLog(ex.Message, "OpenEnrollment Controller", " AddBeneficiary", "");
                    ModelState.AddModelError("Employee", ex.Message);
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }

        public void GetBeneficiaryData(EmgApiModels model)
        {
            var EmployeeDependent = Objddl.GetEmployeeDependentList("", model.EmpID).ToList();
            if (EmployeeDependent != null)
            {
                if (EmployeeDependent.Count > 0)
                {
                    objStatus.DataList = EmployeeDependent;
                    objStatus.Message = "Dependent List";
                    objStatus.MessageStatus = true;
                }
                else
                {
                    objStatus.DataList = EmployeeDependent;
                    objStatus.Message = "No Dependent in List";
                    objStatus.MessageStatus = false;
                }
            }
            else
            {
                //objStatus.DataList1 = new List<AddBeneficiary>();
                //EmployeeDependent = new List<SelectList>();
                objStatus.DataList = EmployeeDependent;
                objStatus.Message = "No Dependent in List";
                objStatus.MessageStatus = false;
            }



            AddBeneficiary objbenf = new AddBeneficiary();
            var BeneficiaryData = objbenf.GetBeneficirayDetails(Convert.ToInt64(model.EmpID), Convert.ToInt64(model.OpnEnrollmentId), model.Type, null, Convert.ToInt32(model.PlanCategoryID), 2, Convert.ToInt32(model.IsBeneficiary));
            if (BeneficiaryData != null)
            {
                if (BeneficiaryData.Count > 0)
                {
                    objStatus.DataList1 = BeneficiaryData;
                    objStatus.Message1 = "Beneficiary Data";
                    objStatus.MessageStatus1 = true;
                }
                else
                {
                    objStatus.DataList1 = new List<AddBeneficiary>();
                    objStatus.Message1 = "No Beneficiary Data in List";
                    objStatus.MessageStatus1 = false;
                }
            }
            else
            {
                objStatus.DataList1 = new List<AddBeneficiary>();
                objStatus.Message1 = "No Beneficiary Data in List";
                objStatus.MessageStatus1 = false;
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpGet]
        public HttpResponseMessage GetBeneficiaryDetails(EmgApiModels model)//1 for dependent change, 5 for Edit Beneficiary data in flag // when dependent change- Dependent id, when record edit row id in DependentIDs
        {
            if (ModelState.IsValid)
            {

                AddBeneficiary objbenf = new AddBeneficiary();
                var data = objbenf.GetBeneficirayDetails(Convert.ToInt64(model.EmpID), Convert.ToInt64(model.OpnEnrollmentId), model.Type, Convert.ToInt64(model.DependentIDs), Convert.ToInt32(model.PlanCategoryID), Convert.ToInt32(model.Flag), Convert.ToInt32(model.IsBeneficiary)).ToList();
                if (data.Count > 0)
                {
                    objStatus.DataList = data;
                    objStatus.MessageStatus = true;
                    objStatus.Message = "Dependent Details";
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);

        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage DeleteBeneficiary(EmgApiModels model)
        {
            if (ModelState.IsValid)
            {

                int result = 0;
                try
                {
                    bool IsBeneficiary = false;
                    if (model.IsBeneficiary > 0)
                    {
                        IsBeneficiary = true;
                    }
                    AddBeneficiary objbenf = new AddBeneficiary();
                    result = objbenf.DeleteBeneficiary(Convert.ToInt64(model.DependentIDs), Convert.ToInt32(model.PlanCategoryID), IsBeneficiary, Convert.ToInt32(model.EmpID), "Employee");
                    if (result == 1)
                    {
                        objStatus.MessageStatus2 = true;
                        objStatus.Message2 = "Beneficiary deleted Successfully.";
                    }
                    else
                    {
                        objStatus.MessageStatus2 = false;
                        objStatus.Message2 = "Some error occurred while processing the request. Please try again";
                    }
                    GetBeneficiaryData(model);
                }

                catch (Exception ex)
                {
                    ObjElog.CustomErrorLog(ex.Message, "OpenEnrollment Controller", " AddBeneficiary", "");
                    ModelState.AddModelError("Employee", ex.Message);
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpGet]
        public HttpResponseMessage GetTotalAllocation(EmgApiModels model)
        {
            decimal result = 0;
            AddBeneficiary objbenf = new AddBeneficiary();
            var BeneficiaryData = objbenf.GetBeneficirayDetails(model.EmpID, Convert.ToInt64(model.OpnEnrollmentId), model.Type, null, Convert.ToInt32(model.PlanCategoryID), 2, Convert.ToInt32(model.IsBeneficiary)).ToList();
            if (BeneficiaryData != null)
            {
                for (int i = 0; i < BeneficiaryData.Count; i++)
                {
                    result = result + BeneficiaryData[i].Allocation;
                }
            }

            if (result > 0)
            {
                objStatus.MessageStatus1 = true;
                objStatus.Message1 = "Total Allocation is " + result + ".";
                objStatus.DataList = result;
            }
            else
            {
                objStatus.MessageStatus1 = false;
                objStatus.Message1 = "Allocation is required";
                objStatus.DataList = result;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage AddPlanBeneficiary(EMGPlanBeneficiary model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(model.DependentId.ToString()))
                    {
                        model.DependentId = 0;
                    }
                    if (model.Gender.ToString().ToLower() == "male")
                    {
                        model.Gender = "1";
                    }
                    else
                    {
                        model.Gender = "2";
                    }

                    var data = new AddBeneficiary().AddBeneficiaryPlan(Convert.ToInt64(model.EmployeeId), Convert.ToInt32(model.EmployeeId), "Employee", model.DependentId.ToString(), Convert.ToInt64(model.OpenEnrollId), model.Type, Convert.ToInt64(model.CatId), Convert.ToDecimal(model.Allocation), 0, model.FirstName, model.MiddleName, model.LastName, model.NameTitle, model.DOB, model.Dependent, model.Gender, model.SSN, Convert.ToInt32(model.IsBeneficiary));

                    if (data == 0)
                    {
                        objStatus.MessageStatus = false;
                        objStatus.Message = "Please enter another dependent. Spouse already added.";
                    }
                    else if (data == -5)
                    {
                        objStatus.MessageStatus = false;
                        objStatus.Message = "SSN already exist.";
                    }
                    else if (data > 0)
                    {
                        EmgApiModels modellist = new EmgApiModels();
                        modellist.EmpID = Convert.ToInt64(model.EmployeeId);
                        modellist.OpnEnrollmentId = Convert.ToInt64(model.OpenEnrollId);
                        modellist.Type = model.Type;
                        modellist.PlanCategoryID = model.CatId;
                        modellist.DependentIDs = null;
                        modellist.Flag = 2;
                        GetBeneficiaryData(modellist);
                        objStatus.MessageStatus = true;
                        objStatus.Message = "Beneficiary List";
                    }



                    //AddBeneficiary objbenf = new AddBeneficiary();
                    //EMG_PlanBeneficiary objplanBen = new EMG_PlanBeneficiary();
                    //objplanBen.DependentId = model.DependentId;
                    //objplanBen.Allocation = model.Allocation;
                    //objplanBen.EmployeeId = model.EmployeeId;
                    //objplanBen.OpenEnrollId = model.OpenEnrollId;
                    //objplanBen.CatId = model.CatId;
                    //objplanBen.Type = model.Type;
                    //objplanBen.IsActive = true;
                    //objplanBen.ID = model.ID;
                    //objplanBen.IsBeneficiary =


                    //objplanBen.FirstName = model.FirstName;
                    //objplanBen.LastName = model.LastName;
                    //objplanBen.MiddleName = model.MiddleName;
                    //objplanBen.NameTitle = model.NameTitle;
                    //objplanBen.DOB = Convert.ToDateTime(model.DOB);
                    //objplanBen.SSN = model.SSN;
                    //objplanBen.Gender = model.Gender;
                    //objplanBen.Dependent = model.Dependent;
                    //var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone("", "", (Convert.ToInt32(objplanBen.DependentId) == 0 ? Convert.ToInt32(objplanBen.ID) : Convert.ToInt32(objplanBen.DependentId)), (Convert.ToInt32(objplanBen.DependentId) == 0 ? 5 : 4), objplanBen.SSN);
                    //if (duplicateComRecord.Count == 0)
                    //{
                    //    var data = objbenf.AddDependents(objplanBen);
                    //    if (data == -1)
                    //    {
                    //        objStatus.MessageStatus = false;
                    //        objStatus.Message = "We cannot add multiple spouse in same category.";

                    //    }
                    //    else if (data > 0)
                    //    {
                    //        EmgApiModels modellist = new EmgApiModels();
                    //        modellist.EmpID = Convert.ToInt64(model.EmployeeId);
                    //        modellist.OpnEnrollmentId = Convert.ToInt64(model.OpenEnrollId);
                    //        modellist.Type = model.Type;
                    //        modellist.PlanCategoryID = model.CatId;
                    //        modellist.DependentIDs = null;
                    //        modellist.Flag = 2;
                    //        GetBeneficiaryData(modellist);
                    //        objStatus.MessageStatus = true;
                    //        objStatus.Message = "Beneficiary List";
                    //    }
                    //    else
                    //    {
                    //        objStatus.MessageStatus = false;
                    //        objStatus.Message = "Some error occurred while processing the request. Please try again";
                    //    }

                    //}
                    //else
                    //{
                    //    objStatus.MessageStatus = false;
                    //    objStatus.Message = "SSN already exist.";
                    //}
                }

                catch (Exception ex)
                {
                    ObjElog.CustomErrorLog(ex.Message, "OpenEnrollment Controller", " AddBeneficiary", "");
                    ModelState.AddModelError("Employee", ex.Message);
                }
            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        #endregion
        public static bool ValidateServerCertificate(
        object sender,
       X509Certificate certificate,
       X509Chain chain,
       SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage UploadEOPdf()
        {

            string status = string.Empty;
            string OrgFileName = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;

            try
            {
                string postedImage = httpRequest["Image"];
                string strEmpId = httpRequest["EmpID"];
                long lngEmployeeID = Convert.ToInt64(strEmpId);

                long EnrollID = Convert.ToInt64(httpRequest["EnrollID"]);
                long lngCompanyID = Convert.ToInt64(httpRequest["CompanyID"]);

                var files = httpRequest.Files;
                if (files.Count > 0)
                {


                    var ImagepathOrg = HttpContext.Current.Server.MapPath("~/Areas/Company/Content/Upload/Employee/EOIForms"); // (!string.IsNullOrEmpty(flagvalue) ? flagvalue == "emp" ? HttpContext.Current.Server.MapPath("~/Areas/Company/Content/Upload/Employee/EOIForms") : HttpContext.Current.Server.MapPath("~/Areas/Company/Content/Upload/Spouse/EOIForms") : "");
                    //Save image in directory
                    if (!Directory.Exists(ImagepathOrg))
                        Directory.CreateDirectory(ImagepathOrg);
                    string Filename = "EOI-C" + lngCompanyID + "-E" + lngEmployeeID + "-OE" + EnrollID;
                    string searchPattern = string.Format("*{0}*", Filename);
                    var filesToDelete = Directory.EnumerateFiles(HttpContext.Current.Server.MapPath("~/Areas/Company/Content/Upload/Employee/EOIForms/"), searchPattern);
                    if (filesToDelete.Count() > 0)
                    {
                        foreach (var fileToDelete in filesToDelete)
                        {
                            try
                            {
                                System.IO.File.Delete(fileToDelete);
                            }
                            catch (Exception ex)
                            {
                                // log this...
                            }
                        }
                    }


                    var file = httpRequest.Files[0];

                    string strEmployeeOrgImg = "EOI-C" + lngCompanyID + "-E" + lngEmployeeID + "-OE" + EnrollID + file.FileName;
                    OrgFileName = Convert.ToString(file.FileName);
                    int result = OpenEnroll.UpdateFileName(lngEmployeeID, EnrollID, strEmployeeOrgImg, OrgFileName);

                    if (result == 1)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(ImagepathOrg, System.IO.Path.GetFileName(strEmployeeOrgImg));
                        file.SaveAs(path);
                        objStatus.Message = "File uploaded successfully";
                        objStatus.MessageStatus = true;
                    }
                    else
                    {
                        objStatus.Message = "Some error occurred while processing the request. Please try again";
                        objStatus.MessageStatus = false;
                    }

                }
                else
                {
                    objStatus.Message = "blank file";
                    objStatus.MessageStatus = false;
                }

                var filename = OpenEnroll.GetFileName(lngEmployeeID, EnrollID);
                if (!string.IsNullOrEmpty(filename.EOIForm))
                {
                    objStatus.EOiFileName = "/Areas/Company/Content/Upload/Employee/EOIForms/" + filename.EOIForm;
                }

            }
            catch (Exception ex)
            {
                objStatus.Message = "Some error occurred";
                objStatus.MessageStatus = false;
                ObjElog.CustomErrorLog(ex.Message, "EMg Api Controller", "UploadEOPdf", "JsonCalling function");
                ModelState.AddModelError("Employee", ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage UploadEOISpousePdf()
        {

            string status = string.Empty;
            string OrgFileName = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;

            try
            {
                string postedImage = httpRequest["Image"];
                string strEmpId = httpRequest["EmpID"];
                long lngEmployeeID = Convert.ToInt64(strEmpId);

                long EnrollID = Convert.ToInt64(httpRequest["EnrollID"]);
                long lngCompanyID = Convert.ToInt64(httpRequest["CompanyID"]);
                var files = httpRequest.Files;
                if (files.Count > 0)
                {

                    var ImagepathOrg = HttpContext.Current.Server.MapPath("~/Areas/Company/Content/Upload/Spouse/EOIForms");
                    //Save image in directory
                    if (!Directory.Exists(ImagepathOrg))
                        Directory.CreateDirectory(ImagepathOrg);
                    string Filename = "EOIS-C" + lngCompanyID + "-E" + lngEmployeeID + "-OE" + EnrollID;
                    string searchPattern = string.Format("*{0}*", Filename);
                    var filesToDelete = Directory.EnumerateFiles(HttpContext.Current.Server.MapPath("~/Areas/Company/Content/Upload/Spouse/EOIForms/"), searchPattern);
                    if (filesToDelete.Count() > 0)
                    {
                        foreach (var fileToDelete in filesToDelete)
                        {
                            try
                            {
                                System.IO.File.Delete(fileToDelete);
                            }
                            catch (Exception ex)
                            {
                                // log this...
                                ObjElog.CustomErrorLog(ex.Message, "EMg Api Controller", "UploadEOISpousePdf", "JsonCalling function");
                            }
                        }
                    }


                    var file = httpRequest.Files[0];

                    string strEmployeeOrgImg = "EOIS-C" + lngCompanyID + "-E" + lngEmployeeID + "-OE" + EnrollID + file.FileName;
                    OrgFileName = Convert.ToString(file.FileName);
                    int result = OpenEnroll.UpdateFileNameSpouse(lngEmployeeID, EnrollID, strEmployeeOrgImg, OrgFileName);

                    if (result == 1)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(ImagepathOrg, System.IO.Path.GetFileName(strEmployeeOrgImg));
                        file.SaveAs(path);
                        objStatus.Message = "File uploaded successfully";  //"Redirect Dependent";
                        objStatus.MessageStatus = true;
                    }
                    else
                    {
                        objStatus.Message = "Some error occurred while processing the request. Please try again";  //"Redirect Dependent";
                        objStatus.MessageStatus = false;
                    }

                }
                else
                {
                    objStatus.Message = "blank file";  //"Redirect Dependent";
                    objStatus.MessageStatus = false;
                }

                var filename = OpenEnroll.GetFileName(lngEmployeeID, EnrollID);
                if (!string.IsNullOrEmpty(filename.EOISpouseForm))
                {
                    objStatus.EOiFileName = "/Areas/Company/Content/Upload/Spouse/EOIForms/" + filename.EOISpouseForm;
                }

            }
            catch (Exception ex)
            {
                objStatus.Message = "Some error occurred";  //"Redirect Dependent";
                objStatus.MessageStatus = false;
                ObjElog.CustomErrorLog(ex.Message, "EMg Api Controller", "UploadEOISpousePdf", "JsonCalling function");
                ModelState.AddModelError("Employee", ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetEoiFileName(long EnrollID = 0, long EmpID = 0, long PlanId = 0)
        {

            if (EnrollID > 0 && EmpID > 0 && PlanId > 0)
            {
                var imageName = OpenEnroll.GetFileName(EmpID, EnrollID);
                var Eoidetails = OpenEnroll.GetMinMaxByInsurancePlanID(Convert.ToInt64(PlanId));
                if (Eoidetails != null)
                {
                    objStatus.EOIDesc = (!string.IsNullOrEmpty(Eoidetails.EOIDesc) ? Eoidetails.EOIDesc : "");
                    objStatus.EOIDownloadFileName = (!string.IsNullOrEmpty(Eoidetails.EOIFileName) ? "~/Areas/Company/Content/Upload/EOIDocs/" + Eoidetails.EOIFileName : "");
                    objStatus.EOIUrlDesc = (!string.IsNullOrEmpty(Eoidetails.EOIUrlDesc) ? Eoidetails.EOIUrlDesc : "");
                    objStatus.EOIUrlLink = (!string.IsNullOrEmpty(Eoidetails.EOIUrlLink) ? Eoidetails.EOIUrlLink : "");
                    objStatus.IsEOIUpload = Eoidetails.EOIUpload;
                    objStatus.IsEOIUrl = Eoidetails.EOIUrl;
                }
                else
                {
                    objStatus.EOIDesc = "";
                    objStatus.EOIDownloadFileName = "";
                    objStatus.EOIUrlDesc = "";
                    objStatus.EOIUrlLink = "";
                    objStatus.IsEOIUpload = Eoidetails.EOIUpload;
                    objStatus.IsEOIUrl = Eoidetails.EOIUrl;
                }
                if ((!string.IsNullOrEmpty(imageName.EOIForm)) || (!string.IsNullOrEmpty(imageName.EOISpouseForm)))
                {
                    objStatus.Message = "File available";
                    objStatus.MessageStatus = true;
                    objStatus.EOiFileName = (!string.IsNullOrEmpty(imageName.EOIForm) ? "~/Areas/Company/Content/Upload/Employee/EOIForms/" + imageName.EOIForm : "");
                    objStatus.EOISpouseFileName = (!string.IsNullOrEmpty(imageName.EOISpouseForm) ? "~/Areas/Company/Content/Upload/Spouse/EOIForms/" + imageName.EOISpouseForm : "");
                    objStatus.Id = imageName.ID;
                }
                else
                {
                    objStatus.Message = "No file attached.";
                    objStatus.MessageStatus = true;
                    objStatus.EOiFileName = "";
                    objStatus.EOISpouseFileName = "";
                    objStatus.Id = imageName.ID;
                }
            }
            else
            {
                objStatus.Message = "Please pass the valid input type.";
                objStatus.MessageStatus = false;

            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage DeleteImageEOI(long EmpID = 0, long Id = 0, string FlagValue = "")
        {

            int result = 0;
            if (EmpID > 0 && Id > 0 && FlagValue != "")
            {
                try
                {
                    result = OpenEnroll.UpdateEOIFile(EmpID, Id, FlagValue);
                    if (result == 1)
                    {
                        objStatus.Message = "File Deleted Successfully.";
                        objStatus.MessageStatus = true;
                    }
                    else
                    {
                        objStatus.Message = "Some Error Occured.";
                        objStatus.MessageStatus = false;
                    }
                }
                catch (Exception ex)
                {
                    objStatus.Message = "Some error occurred";  //"Redirect Dependent";
                    objStatus.MessageStatus = false;
                    ObjElog.CustomErrorLog(ex.Message, "EMg Api Controller", "DeleteImageEOI", "JsonCalling function");
                    ModelState.AddModelError("Employee", ex.Message);

                }
            }
            else
            {
                objStatus.Message = "Please pass the valid input type.";
                objStatus.MessageStatus = false;
            }


            return Request.CreateResponse(HttpStatusCode.OK, objStatus);

        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetGender()
        {
            GetDropDownValues ObjEmpProfile = new GetDropDownValues();
            var list = ObjEmpProfile.GetGenderType("");
            objStatus.Message = "Gender List";  //"Redirect Dependent";
            objStatus.MessageStatus = true;
            objStatus.DataList = list;
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage EoiSkip(long EmpID = 0)
        {
            bool ISEOISkip = false;
            if (EmpID > 0)
            {
                try
                {
                    var data = OpenEnroll.GetEmployees(EmpID);
                    if (data != null)
                    {
                        ISEOISkip = data.IsEOISkiped;
                        objStatus.ISEOISkip = ISEOISkip;
                    }
                    else
                    {
                        objStatus.ISEOISkip = false;
                    }

                }
                catch (Exception ex)
                {
                    objStatus.Message = "Some error occurred";
                    objStatus.ISEOISkip = false;
                    ObjElog.CustomErrorLog(ex.Message, "EMg Api Controller", "IsEoiSkip", "JsonCalling function");
                    ModelState.AddModelError("Employee", ex.Message);

                }
            }
            else
            {
                objStatus.Message = "Please pass the valid input type.";
                objStatus.ISEOISkip = false;
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage SpouseSurchargePopup(long EmpID = 0, long EnrollId = 0)
        {
            bool SpouseSurchargePopup = false;
            if (EmpID > 0 && EnrollId > 0)
            {
                try
                {
                    SpouseSurchargePopup = OpenEnroll.SpouseSurchargePopup(EmpID, EnrollId);
                    objStatus.Message = "SpouseSurchargePopup";
                    objStatus.MessageStatus = SpouseSurchargePopup;
                }
                catch (Exception ex)
                {
                    objStatus.Message = "Some error occurred";
                    objStatus.MessageStatus = false;
                    ObjElog.CustomErrorLog(ex.Message, "EMg Api Controller", "SpouseSurchargePopup", "JsonCalling function");
                    ModelState.AddModelError("Employee", ex.Message);
                }
            }
            else
            {
                objStatus.Message = "Please pass the valid input type.";
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage UpdateSurchargeDependent(long EmpID = 0, string ISEmpValue = "0", string HasCoverageValue = "0")
        {
            if (EmpID > 0)
            {
                objStatus.MessageStatus = true;
                int result = 0;
                bool IsEmp = false;
                bool HasCoverage = false;
                IsEmp = (ISEmpValue == "1" ? IsEmp = true : IsEmp = false);
                HasCoverage = (HasCoverageValue == "1" ? HasCoverage = true : HasCoverage = false);
                try
                {
                    result = OpenEnroll.SurchargeDependent(EmpID, IsEmp, HasCoverage);
                    if (result == 1)
                    {
                        objStatus.Message = "Surcharge Dependent Value Update Successfully.";
                        objStatus.MessageStatus = true;
                    }
                }
                catch (Exception ex)
                {
                    objStatus.Message = "Some error occured.";
                    objStatus.MessageStatus = false;
                    ObjElog.CustomErrorLog(ex.Message, "EMGAPI Controller", "UpdateSurchargeDependent", "JsonCalling function");
                    ModelState.AddModelError("Employee", ex.Message);
                }

            }
            else
            {
                objStatus.Message = "Please pass the valid input type.";
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage DeleteSupportDocument(string ID)
        {
            try
            {
                long supDocId = long.Parse(ID);
                int result = OpenEnroll.DeleteSupportDocumentLife(supDocId);
                if (result == 1)
                {
                    objStatus.Message = "Document Deleted Successfully.";
                    objStatus.MessageStatus = true;
                }
                if (result == 0)
                {
                    objStatus.Message = "No File Deleted.";
                    objStatus.MessageStatus = false;
                }
            }
            catch (Exception ex)
            {
                objStatus.Message = "Some error occured.";
                objStatus.MessageStatus = false;
                ObjElog.CustomErrorLog(ex.Message, "EMGAPI Controller", "DeleteSupportDocument", "JsonCalling function");
                ModelState.AddModelError("Employee", ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        public HttpResponseMessage UpdateLifeEvent()
        {
            var httpRequest = HttpContext.Current.Request;
            string fileName = string.Empty;
            EMG_LifeChangeEvent objdetails = new EMG_LifeChangeEvent();

            //string postedImage = httpRequest["Image"];
            var EmpID = Convert.ToInt64(httpRequest["EmpID"]);
            var CompanyID = Convert.ToInt64(httpRequest["CompanyID"]);
            var LEventId = Convert.ToInt64(httpRequest["LEventId"]);
            var LifeEventDate = Convert.ToDateTime(httpRequest["LifeEventDate"]);
            var EventID = Convert.ToInt32(httpRequest["EventID"]);


            objdetails.ID = LEventId;
            objdetails.LifeEventID = Convert.ToInt32(EventID);
            objdetails.EventDate = LifeEventDate;
            objdetails.Status = "P";
            objdetails.DateModified = DateTime.UtcNow;

            int result = OpenEnroll.SaveLifeChangeEvent(objdetails);
            int resultsupport;
            EMG_LifeChangeEvent_SupportDocument objSupport = new EMG_LifeChangeEvent_SupportDocument();
            HttpResponseMessage response = new HttpResponseMessage();
            var files = httpRequest.Files;
            HttpPostedFile file = null;
            if (result > 0)
            {
                if (files.Count > 0)
                {
                    int index = 0;
                    foreach (string key in httpRequest.Files.AllKeys)
                    {
                        file = httpRequest.Files[index];
                        if (file.ContentLength > 0)
                        {
                            fileName = DateTime.UtcNow.Ticks + "-" + file.FileName;
                            var path = HttpContext.Current.Server.MapPath("~/Uploads/SupportDocument/");
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            path = Path.Combine(path, System.IO.Path.GetFileName(fileName));
                            file.SaveAs(path);
                            objSupport.LifeChangeEventID = result;
                            objSupport.CompanyID = CompanyID;
                            objSupport.EmployeeID = EmpID;
                            objSupport.SupportDocument = fileName;
                            objSupport.DateCreated = DateTime.UtcNow;
                            objSupport.OrgFileName = file.FileName;
                            resultsupport = OpenEnroll.SaveLifeChangeEventSupportDocument(objSupport);
                        }
                        index++;
                    }

                    objStatus.SuccessMessage = true;
                    objStatus.Message = "Life Event Update Successfully.";

                }
                else
                {
                    objStatus.SuccessMessage = true;
                    objStatus.Message = "No file was added";
                }
            }
            else
            {
                objStatus.SuccessMessage = false;
                objStatus.Message = "Some Error Occured";
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        // Enrollment Listing
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public List<psp_checkEligliblitySalaryband_Result> checkEligliblitySalaryband(long EmpID, long OpenEnrollID)
        {
            using (PowerIBrokerEntities context = new PowerIBrokerEntities())
            {
                return context.psp_checkEligliblitySalaryband(EmpID, OpenEnrollID).ToList();
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public List<psp_GetOpenEnrollmentCategory_WaiverStatus_Result> GetOpenEnrollMentCategoryWaiverStatus(long EmpID, long OpenEnrollID, string Type)
        {
            using (PowerIBrokerEntities context = new PowerIBrokerEntities())
            {
                return context.psp_GetOpenEnrollmentCategory_WaiverStatus(EmpID, OpenEnrollID, "OE").ToList();

            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public bool SpouseSurchargePopupListing(long EmpId, long EnrollId)
        {
            using (PowerIBrokerEntities context = new PowerIBrokerEntities())
            {
                bool result = false;
                var SurchargePopupdata = context.SpouseSurchargePopup(EmpId, EnrollId).FirstOrDefault();
                if (SurchargePopupdata != null)
                {
                    result = SurchargePopupdata.Value;
                }
                return result;
            }
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public List<PowerIBrokerBusinessLayer.Employee.Emg_SalaryBand> GetSalaryBandBaseOnSalary(long EmpID, long InsurancePlanID)
        {
            List<PowerIBrokerBusinessLayer.Employee.Emg_SalaryBand> Objdetails = new List<PowerIBrokerBusinessLayer.Employee.Emg_SalaryBand>();

            using (PowerIBrokerEntities context = new PowerIBrokerEntities())
            {
                var companyID = context.tblCompany_Employee_BasicInfo.Where(a => a.ID == EmpID).Select(a => a.CompanyId).FirstOrDefault();
                Objdetails = (from Details in context.Emg_SalaryBand
                              where Details.CompanyId == companyID && Details.IsActive == true && Details.InsurancePlanID == InsurancePlanID
                              select new PowerIBrokerBusinessLayer.Employee.Emg_SalaryBand()
                              {
                                  ID = Details.ID,
                                  SalaryStart = Details.SalaryStart,
                                  SalaryEnd = Details.SalaryEnd,
                                  CompanyId = Details.CompanyId,
                                  IsActive = Details.IsActive,
                                  InsurancePlanID = Details.InsurancePlanID,
                              }).ToList();

            }
            return Objdetails;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage WaiveBenefitOnEmployeebulk(long EmpId = 0, long EnrollID = 0, string WaiveReason = "", long WaiveOptionID = 0)
        {
            int result = 0;

            if (EnrollID > 0 && WaiveOptionID > 0 && EmpId > 0)
            {
                try
                {
                    tblEmployee_Benefits benefits = new tblEmployee_Benefits();
                    var OpenEnrollCatWaiverStatus = OpenEnroll.GetOpenEnrollMentCategoryWaiverStatus(EmpId, EnrollID, "OE");
                    if (EmpId != 0)
                    {
                        foreach (var item in OpenEnrollCatWaiverStatus)
                        {
                            result = OpenEnroll.WaiveBenefitOmEmployee(EmpId, EnrollID, Convert.ToInt64(item.PlanCategoryID), "OE", WaiveReason, WaiveOptionID, Convert.ToInt32(EmpId), "Employee");
                        }
                        if (result > 0)
                        {
                            objStatus.Message = "All plans waived successfully.";
                            objStatus.MessageStatus = true;
                        }

                    }
                }
                catch (Exception ex)
                {
                    objStatus.Message = "Some error occurred";
                    objStatus.MessageStatus = false;
                    ObjElog.CustomErrorLog(ex.Message, "EMg Api Controller", "WaiveBenefitOnEmployeebulk", "API Calling function");

                }

            }
            else
            {
                objStatus.Message = "Please pass the valid input type.";
            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage GetEnrollmentSummary(long EmpID)
        {
            var enrollSummary = OpenEnroll.GetEnrollmentSummary(EmpID);
            if (enrollSummary != null && enrollSummary.EnrollD > 0)
            {

                objStatus.Message = "Enrollment Summary";
                objStatus.MessageStatus = true;
                objStatus.Id = Convert.ToInt64(enrollSummary.EnrollD);

            }
            else
            {
                objStatus.Message = "No record found";
                objStatus.MessageStatus = false;

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);


        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        public HttpResponseMessage GetMyBenefitEnrollment(long EmpID)
        {
            GetDropDownValues ObjEditProfileddl = new GetDropDownValues();
            try
            {
                if (EmpID > 0)
                {
                    var ExpiredEnrollment = ObjEditProfileddl.GetExpiredEnrollment(EmpID);
                    var compnyID = OpenEnroll.GetCompanyID(EmpID);
                    var CompanyName = OpenEnroll.GetCompanyNameByCompanyID(compnyID);
                    objStatus.Message = "All Enrollment List";
                    objStatus.MessageStatus = true;
                    objStatus.DataList = ExpiredEnrollment;
                    objStatus.CompanyName = CompanyName;
                }
                else
                {
                    objStatus.Message = "Please pass the valid input type.";
                    objStatus.MessageStatus = false;
                }
            }
            catch (Exception ex)
            {
                objStatus.Message = "Some error occurred";
                objStatus.MessageStatus = false;
                ObjElog.CustomErrorLog(ex.Message, "EMg Api Controller", "GetMyBenefitEnrollment", "API Calling function");

            }
            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [HttpPost]
        public HttpResponseMessage GetMyBenefitInfo(EmgApiModels model)
        {
            GetDropDownValues ObjEditProfileddl = new GetDropDownValues();
            if (ModelState.IsValid)
            {
                long EnrollID = 0;
                if (model.UidSession != "")
                {
                    long? OEID = OpenEnroll.GetEnrollID(model.UidSession);
                    EnrollID = Convert.ToInt64(OEID);
                }
                var ExpiredEnrollment = ObjEditProfileddl.GetExpiredEnrollment(model.EmpID);
                var compnyID = OpenEnroll.GetCompanyID(model.EmpID);
                var CompanyName = OpenEnroll.GetCompanyNameByCompanyID(compnyID);

                if (EnrollID == 0)
                {
                    var tempUidSession = Convert.ToInt32((new List<System.Web.Mvc.SelectListItem>(ExpiredEnrollment))[0].Value);
                    long? OEID = OpenEnroll.GetEnrollID(Convert.ToString(tempUidSession));
                    EnrollID = Convert.ToInt64(OEID);
                }

                objStatus.Message = "All Enrollment List";
                objStatus.MessageStatus = true;
                objStatus.DataList = ExpiredEnrollment;
                objStatus.CompanyName = CompanyName;

                long? CovId = 0;
                ReviewPlanList obj = null;
                ReviewPlanList objPendingEmp = null;
                ReviewPlanList objPendingSpouse = null;
                AddBeneficiary objbenf = new AddBeneficiary();
                int i = 0;
                List<ReviewPlanList> list = new List<ReviewPlanList>();
                List<ReviewPlanList> listPending = new List<ReviewPlanList>();
                var SelectedPlan = OpenEnroll.GetInsuranceMyBenefitsByEmployeeID(model.EmpID, EnrollID, "OE", model.UidSession);
                List<DependentsViewBenefits> depnCat = new List<DependentsViewBenefits>();
                var OpenEnrollData = OpenEnroll.GetEnrollType(model.EmpID, EnrollID);
                var EnrollType = "OE";
                if (OpenEnrollData != null)
                {
                    EnrollType = OpenEnrollData.EnrollType.ToUpper();
                }

                long EMP_GranteedIssueAmt = 0;
                long Spouse_GranteedIssueAmt = 0;
                string PendingMedicalEmp = string.Empty;
                string PendingMedicalSpouse = string.Empty;
                if (SelectedPlan != null)
                {
                    if (SelectedPlan.Count > 0)
                    {
                        foreach (var benefitsPending in SelectedPlan.Where(a => a.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife).ToList())
                        {
                            var dataPending = OpenEnroll.GetMinMaxByInsurancePlanID(Convert.ToInt64(benefitsPending.PlanID));
                            long EmpSeletedBenefit = Convert.ToInt64(benefitsPending.SelectedRange);
                            long SpouseSeletedBenefit = Convert.ToInt64(benefitsPending.SpouseSelectedRange);
                            if (dataPending != null)
                            {
                                EMP_GranteedIssueAmt = (EnrollType == "LE" ? Convert.ToInt64(dataPending.PendingMedicalLE) : EnrollType == "NH" ? Convert.ToInt64(dataPending.PendingMedicalNH) : Convert.ToInt64(dataPending.PendingMedical));
                                Spouse_GranteedIssueAmt = (EnrollType == "LE" ? Convert.ToInt64(dataPending.SIssueAmountLE) : EnrollType == "NH" ? Convert.ToInt64(dataPending.SIssueAmountNH) : Convert.ToInt64(dataPending.SIssueAmount));
                            }
                            if (EmpSeletedBenefit > EMP_GranteedIssueAmt)
                            {
                                PendingMedicalEmp = "Yes";
                            }
                            else
                            {
                                PendingMedicalEmp = "No";
                            }
                            if (SpouseSeletedBenefit > Spouse_GranteedIssueAmt)
                            {
                                PendingMedicalSpouse = "Yes";
                            }
                            else
                            {
                                PendingMedicalSpouse = "No";
                            }
                            if (PendingMedicalEmp == "Yes" || PendingMedicalSpouse == "Yes")
                            {

                                if (SelectedPlan.Where(a => a.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife && a.EOIStatus.ToUpper() == "P" || a.EOIStatus.ToUpper() == "D").Any())
                                {
                                    var InsuranceBenefitsPendingBenefits = SelectedPlan.Where(a => a.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife && a.EOIStatus.ToUpper() == "P" || a.EOIStatus.ToUpper() == "D").ToList();
                                    foreach (var benefits in InsuranceBenefitsPendingBenefits)
                                    {
                                        objPendingEmp = new ReviewPlanList();
                                        objPendingSpouse = new ReviewPlanList();

                                        var details = OpenEnroll.GetCPPbyEmployeeID(Convert.ToInt64(benefits.PlanID), Convert.ToInt64(model.EmpID), Convert.ToString(model.UidSession));

                                        var data = OpenEnroll.GetMinMaxByInsurancePlanID(Convert.ToInt64(benefits.PlanID));
                                        if (PendingMedicalEmp == "Yes")
                                        {
                                            if (benefits.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                                            {
                                                objPendingEmp.PlanCategory = benefits.PlanCategory;
                                                objPendingEmp.Carrier = benefits.Provider;
                                                objPendingEmp.PlanName = benefits.PlanName;
                                                objPendingEmp.Coverage = EnumString.Employee;
                                                objPendingEmp.EffectiveOn = EnumString.Pending;
                                                objPendingEmp.DisplayCost = Convert.ToString((benefits.EmpCostPerPayPeriod == null ? 0 : (benefits.EmpCostPerPayPeriod)));
                                                objPendingEmp.Benefit = Convert.ToString((EmpSeletedBenefit - EMP_GranteedIssueAmt));
                                                listPending.Add(objPendingEmp);
                                            }
                                        }
                                        if (PendingMedicalSpouse == "Yes")
                                        {
                                            if (benefits.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                                            {
                                                objPendingSpouse.PlanCategory = benefits.PlanCategory;
                                                objPendingSpouse.Carrier = benefits.Provider;
                                                objPendingSpouse.PlanName = benefits.PlanName;
                                                objPendingSpouse.Coverage = EnumString.Spouse;
                                                objPendingSpouse.EffectiveOn = EnumString.Pending;
                                                objPendingSpouse.DisplayCost = Convert.ToString((benefits.SpouseCostPerPayPeriod == null ? 0 : (benefits.SpouseCostPerPayPeriod)));
                                                objPendingSpouse.Benefit = Convert.ToString((SpouseSeletedBenefit - Spouse_GranteedIssueAmt));
                                                listPending.Add(objPendingSpouse);
                                            }
                                        }

                                    }
                                    if (listPending.Count > 0)
                                    {
                                        objStatus.Message5 = "Pending Benefits";
                                        objStatus.MessageStatus5 = true;
                                        objStatus.DataList5 = listPending;
                                    }
                                    else
                                    {
                                        objStatus.Message5 = "Pending Benefits";
                                        objStatus.MessageStatus5 = false;
                                        objStatus.DataList5 = "";
                                    }

                                }
                            }
                            else
                            {
                                objStatus.Message5 = "Pending Benefits";
                                objStatus.MessageStatus5 = false;
                                objStatus.DataList5 = "";
                            }
                        }

                        List<DependentsViewBenefits> depnList = new List<DependentsViewBenefits>();
                        foreach (var cat in SelectedPlan)
                        {
                            if (cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.Medical || cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.Dental || cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.Vision)
                            {
                                DependentsViewBenefits singleCat = new DependentsViewBenefits();
                                List<psp_GetVieBenefitsDependentsByEmployeeID_Result> subList = new List<psp_GetVieBenefitsDependentsByEmployeeID_Result>();
                                singleCat.Category = cat.PlanCategory;
                                var Dependentlist = OpenEnroll.GetViewBenefitsDependentsMyBenefit(model.EmpID, EnrollID, "OE", model.UidSession).Where(a => a.PlanCategoryId == cat.PlanCategoryId).ToList();

                                if (Dependentlist != null)
                                {
                                    foreach (var item in Dependentlist)
                                    {
                                        if (!string.IsNullOrEmpty(item.FirstName))
                                        {
                                            psp_GetVieBenefitsDependentsByEmployeeID_Result singleDept = new psp_GetVieBenefitsDependentsByEmployeeID_Result();
                                            singleDept.FirstName = item.FirstName;
                                            singleDept.SSN = item.SSN;
                                            subList.Add(singleDept);
                                        }

                                    }
                                    singleCat.Dependents = subList;
                                }
                                depnCat.Add(singleCat);
                            }
                            if (cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.Life)
                            {
                                var BeneficiaryDataLife = objbenf.GetBeneficirayDetails(Convert.ToInt64(model.EmpID), EnrollID, "OE", null, Convert.ToInt32(cat.PlanCategoryId), 2, 0);
                                if (BeneficiaryDataLife != null && BeneficiaryDataLife.Count > 0)
                                {
                                    objStatus.Message3 = "Beneficiary Life ";
                                    objStatus.MessageStatus3 = true;
                                    objStatus.DataList3 = BeneficiaryDataLife;

                                }
                                else
                                {
                                    objStatus.Message3 = "No Beneficiary Life ";
                                    objStatus.MessageStatus3 = false;
                                    objStatus.DataList3 = "";

                                }

                            }
                            if (cat.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                            {
                                var BeneficiaryDataVLife = objbenf.GetBeneficirayDetails(Convert.ToInt64(model.EmpID), EnrollID, "OE", null, Convert.ToInt32(cat.PlanCategoryId), 2, 0);
                                if (BeneficiaryDataVLife != null && BeneficiaryDataVLife.Count > 0)
                                {
                                    objStatus.Message4 = "Beneficiary Voluntary Life";
                                    objStatus.MessageStatus4 = true;
                                    objStatus.DataList4 = BeneficiaryDataVLife;

                                }
                                else
                                {
                                    objStatus.Message4 = "No Beneficiary Voluntary Life";
                                    objStatus.MessageStatus4 = false;
                                    objStatus.DataList4 = "";

                                }
                            }
                        }
                    }
                }

                if ((!object.Equals(SelectedPlan, null)) && (SelectedPlan.Count() > 0))
                {
                    objStatus.SuccessMessage = true;
                    objStatus.Message = "No Selected Plan Found";

                    foreach (var row in SelectedPlan)
                    {
                        CovId = row.CoverageID;
                        if (CovId == null)
                        {
                            CovId = 0;
                        }
                        obj = new ReviewPlanList();

                        if ((!object.Equals(SelectedPlan, null)))
                        {
                            obj.PlanCategory = (SelectedPlan[i].PlanCategory != null ? SelectedPlan[i].PlanCategory.ToString() : "");
                            obj.PlanName = (SelectedPlan[i].PlanName != null ? SelectedPlan[i].PlanName.ToString() : "");
                            obj.Carrier = (SelectedPlan[i].Provider != null ? SelectedPlan[i].Provider.ToString() : "");
                            obj.Coverage = (SelectedPlan[i].CoverageOption != null ? SelectedPlan[i].CoverageOption.ToString() : "");
                            obj.DeptName = (SelectedPlan[i].FirstName != null ? SelectedPlan[i].FirstName.ToString() : "");
                            obj.SelectedRange = (SelectedPlan[i].SelectedRange != null ? SelectedPlan[i].SelectedRange.ToString() : "");
                            obj.SpouseSelectedRange = (SelectedPlan[i].SpouseSelectedRange != null ? SelectedPlan[i].SpouseSelectedRange.ToString() : "");
                            obj.EmpCostPerPayPeriod = (SelectedPlan[i].EmpCostPerPayPeriod != null ? SelectedPlan[i].EmpCostPerPayPeriod.ToString() : "");
                            obj.SpouseCostPerPayPeriod = (SelectedPlan[i].SpouseCostPerPayPeriod != null ? SelectedPlan[i].SpouseCostPerPayPeriod.ToString() : "");
                            obj.EmpCostPerPayPeriodEnrollType = (SelectedPlan[i].EmpCostPerPayPeriodEnrollType != null ? SelectedPlan[i].EmpCostPerPayPeriodEnrollType.ToString() : "");
                            obj.SpouseCostPerPayPeriodEnrollType = (SelectedPlan[i].SpouseCostPerPayPeriodEnrollType != null ? SelectedPlan[i].SpouseCostPerPayPeriodEnrollType.ToString() : "");
                            obj.ChildCostPerPayPeriod = (SelectedPlan[i].ChildCostPerPayPeriod != null ? SelectedPlan[i].ChildCostPerPayPeriod.ToString() : "");
                            obj.ChildCost = (SelectedPlan[i].ChildCost != null ? SelectedPlan[i].ChildCost.ToString() : "");
                            obj.EOIStatus = (SelectedPlan[i].EOIStatus != null ? SelectedPlan[i].EOIStatus.ToString() : "");
                            obj.EffectiveOn = (SelectedPlan[i].StartDate != null ? SelectedPlan[i].StartDate.ToString() : "");

                            if (row.PlanCategoryId == (int)CategoryEnum.PlanCategory.VoluntaryLife)
                            {
                                var dataPending = OpenEnroll.GetMinMaxByInsurancePlanID(Convert.ToInt64(row.PlanID));
                                if (dataPending != null)
                                {
                                    EMP_GranteedIssueAmt = (EnrollType == "LE" ? Convert.ToInt64(dataPending.PendingMedicalLE) : EnrollType == "NH" ? Convert.ToInt64(dataPending.PendingMedicalNH) : Convert.ToInt64(dataPending.PendingMedical));
                                    Spouse_GranteedIssueAmt = (EnrollType == "LE" ? Convert.ToInt64(dataPending.SIssueAmountLE) : EnrollType == "NH" ? Convert.ToInt64(dataPending.SIssueAmountNH) : Convert.ToInt64(dataPending.SIssueAmount));
                                    obj.EMP_GranteedIssueAmt = EMP_GranteedIssueAmt;
                                    obj.Spouse_GranteedIssueAmt = Spouse_GranteedIssueAmt;
                                    obj.ChildAmount = Convert.ToInt64(dataPending.ChildAmount);
                                }
                            }

                            if (row.PlanCategoryId == (int)CategoryEnum.PlanCategory.Medical || row.PlanCategoryId == (int)CategoryEnum.PlanCategory.Dental || row.PlanCategoryId == (int)CategoryEnum.PlanCategory.Vision)
                            {
                                var plancost = OpenEnroll.GetCPPbyEmployeeID(Convert.ToInt64(row.PlanID), model.EmpID, model.UidSession);
                                if ((!object.Equals(plancost, null)))
                                {
                                    obj.MonthlyCost = (plancost.MonthlyCost != null ? plancost.MonthlyCost.ToString() : "");
                                    obj.SemiMonthly = (plancost.SemiMonthlyCost != null ? plancost.SemiMonthlyCost.ToString() : "");
                                    obj.WeeklyCost = (plancost.WeeklyCost != null ? plancost.WeeklyCost.ToString() : "");
                                    obj.BiWeeklyCost = (plancost.BiWeekly != null ? plancost.BiWeekly.ToString() : "");

                                    if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.WeeklyCost)
                                    {
                                        obj.DisplayCost = plancost.WeeklyCost.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.BiWeekly)
                                    {
                                        obj.DisplayCost = plancost.BiWeekly.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.SemiMonthlyCost)
                                    {
                                        obj.DisplayCost = plancost.SemiMonthlyCost.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.MonthlyCost)
                                    {
                                        obj.DisplayCost = plancost.MonthlyCost.ToString();
                                    }
                                    else
                                    {
                                        obj.DisplayCost = "0.00";
                                    }

                                }
                                else
                                {
                                    obj.MonthlyCost = "0.00";
                                    obj.SemiMonthly = "0.00";
                                    obj.WeeklyCost = "0.00";
                                    obj.BiWeeklyCost = "0.00";
                                    obj.DisplayCost = "0.00";
                                }
                            }
                            else
                            {

                                var plancost = OpenEnroll.GetCPPbyEmployeeID(Convert.ToInt64(row.PlanID), model.EmpID, model.UidSession);
                                if ((!object.Equals(plancost, null)))
                                {
                                    obj.MonthlyCost = (plancost.MonthlyCost != null ? plancost.MonthlyCost.ToString() : "");
                                    obj.SemiMonthly = (plancost.SemiMonthlyCost != null ? plancost.SemiMonthlyCost.ToString() : "");
                                    obj.WeeklyCost = (plancost.WeeklyCost != null ? plancost.WeeklyCost.ToString() : "");
                                    obj.BiWeeklyCost = (plancost.BiWeekly != null ? plancost.BiWeekly.ToString() : "");

                                    if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.WeeklyCost)
                                    {
                                        obj.DisplayCost = plancost.WeeklyCost.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.BiWeekly)
                                    {
                                        obj.DisplayCost = plancost.BiWeekly.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.SemiMonthlyCost)
                                    {
                                        obj.DisplayCost = plancost.SemiMonthlyCost.ToString();
                                    }
                                    else if (SelectedPlan[0].MonthlyBasisID == (int)CategoryEnum.MonthlyBasisID.MonthlyCost)
                                    {
                                        obj.DisplayCost = plancost.MonthlyCost.ToString();
                                    }
                                    else
                                    {
                                        obj.DisplayCost = "0.00";
                                    }

                                }
                                else
                                {
                                    obj.MonthlyCost = "0.00";
                                    obj.SemiMonthly = "0.00";
                                    obj.WeeklyCost = "0.00";
                                    obj.BiWeeklyCost = "0.00";
                                    obj.DisplayCost = "0.00";
                                }


                            }



                        }

                        list.Add(obj);
                        i = i + 1;
                    }


                    if (list.Count > 0)
                    {
                        objStatus.Message = "Select Plan List";
                        objStatus.MessageStatus = true;
                        objStatus.DataList = list;
                        if (depnCat.Count == 0)
                        {
                            objStatus.DataList2 = new List<DependentsViewBenefits>();
                        }
                        else
                        {
                            objStatus.DataList2 = depnCat;
                        }

                    }
                    else
                    {
                        objStatus.Message = "No Selected Plan List Found";
                        objStatus.DataList = new List<ReviewPlanList>();
                        objStatus.MessageStatus = false;
                        objStatus.DataList2 = new List<DependentsViewBenefits>();
                    }




                }
                else
                {
                    objStatus.SuccessMessage = false;
                    objStatus.Message = "No Selected Plan Found";
                    objStatus.DataList2 = new List<DependentsViewBenefits>();

                }
                var OpenEnrollWavCat = OpenEnroll.GetOpenEnrollMentWaivedCategoryMyBenefit(model.EmpID, EnrollID, "OE", model.UidSession);
                if ((!object.Equals(OpenEnrollWavCat, null)) && (OpenEnrollWavCat.Count() > 0))
                {
                    objStatus.Message1 = "Waived Plan List";
                    objStatus.MessageStatus1 = true;
                    objStatus.DataList1 = OpenEnrollWavCat;
                }
                else
                {
                    objStatus.Message1 = "No Waived Plan List Found";
                    objStatus.MessageStatus1 = false;
                }


            }
            else
            {
                objStatus.MessageStatus = false;
                objStatus.Message = "Please pass the valid input type.";
                objStatus.DataList = ModelState.Values;
            }

            return Request.CreateResponse(HttpStatusCode.OK, objStatus);
        }


    }
    public class SelectedPlan
    {
        public string PlanCategory { get; set; }
        public string MonthlyCost { get; set; }
        public string SemiMonthly { get; set; }
        public string WeeklyCost { get; set; }
        public string BiWeeklyCost { get; set; }
        public string DisplayCost { get; set; }
        public bool Status { get; set; }
        public string PlanStatus { get; set; }
        //public string Paypercost { get; set; }

    }
    public class ReviewPlanList
    {
        public string PlanCategory { get; set; }
        public string PlanName { get; set; }
        public string DeptName { get; set; }
        public string Carrier { get; set; }
        public string Coverage { get; set; }
        public string EffectiveOn { get; set; }
        public string MonthlyCost { get; set; }
        public string SemiMonthly { get; set; }
        public string WeeklyCost { get; set; }
        public string BiWeeklyCost { get; set; }
        public string DisplayCost { get; set; }
        public string Benefit { get; set; }
        public bool Status { get; set; }
        public string SelectedRange { get; set; }
        public string SpouseSelectedRange { get; set; }
        public string EmpCostPerPayPeriod { get; set; }
        public string SpouseCostPerPayPeriod { get; set; }
        public string ChildCostPerPayPeriod { get; set; }
        public string ChildCost { get; set; }
        public string EmpCostPerPayPeriodEnrollType { get; set; }
        public string SpouseCostPerPayPeriodEnrollType { get; set; }
        public string EOIStatus { get; set; }
        public long EMP_GranteedIssueAmt { get; set; }
        public long Spouse_GranteedIssueAmt { get; set; }
        public long ChildAmount { get; set; }




    }

}

