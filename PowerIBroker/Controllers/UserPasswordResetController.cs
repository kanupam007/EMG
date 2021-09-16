using PowerIBrokerDataLayer;
using System;
using System.Web.Mvc;
using PowerIBrokerBusinessLayer;
namespace PowerIBroker.Controllers
{
    public class UserPasswordResetController : Controller
    {
        //
        // GET: /UserPasswordReset/

        public ActionResult Index()
        {
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
                    int i = um.ResetUserPassword(new User { Email = email, Password = password }, verifier);
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
    }
}
