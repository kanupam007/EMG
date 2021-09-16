using PowerIBroker.Models;
using PowerIBrokerBusinessLayer;
using PowerIBrokerDataLayer;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /ContactUs/
        PowerIBrokerBusinessLayer.Comman objComman = new PowerIBrokerBusinessLayer.Comman();
        public ActionResult Index()
        {
            CmsManagement cmsObj = new CmsManagement();
            var contactUsContent = cmsObj.GetContactUsContent();
            ViewBag.ContactUsContent = contactUsContent;
            ViewBag.MetaTitle = contactUsContent.MetaTitle;
            ViewBag.MetaDescription = contactUsContent.MetaDescriptions;
            ViewBag.MetaKeywords = contactUsContent.MetaKeywords;
            ViewBag.MetaRobots = contactUsContent.MetaKeyPhrase;
            Administration Admin = new Administration();
            ViewBag.IsCpatcha = Admin.IsCpatcha("CaptchaOnContactUsPage");
            return View();
        }
        [HttpPost]
        public ActionResult Index(EnquiryForm form)
        {
            try
            {
                UserManagement um = new UserManagement();

                if (!string.IsNullOrEmpty(form.ContactUsPhone))
                {
                    form.ContactUsPhone = objComman.RemoveMasking(form.ContactUsPhone);
                }
                else
                {
                    form.ContactUsPhone = null;
                }

                ContactUsEnquiry obj = new ContactUsEnquiry()
                {
                    Email = form.ContactUsEmail,
                    Message = form.Query,
                    Name = form.ContactUsName,
                    Phone = form.ContactUsPhone,
                    IsActive = true,
                    AddedOn = DateTime.UtcNow,

                };
                int i = um.SaveEnquiry(obj);
                if (i > 0)
                {
                    System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();                  
                    string baseurl = Helper.GetBaseUrl();
                    string MailbodyUser = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/ContactUs.html"));  // email template User
                    MailbodyUser = MailbodyUser.Replace("###HostUrl####", baseurl);
                    MailbodyUser = MailbodyUser.Replace("###UserName###", form.ContactUsName);
                    if (!string.IsNullOrEmpty(form.ContactUsEmail))
                    {
                        Helper.SendEmail(form.ContactUsEmail, MailbodyUser, "Contact Us Email", ""); //email User
                    }

                    string AdminMail = (string)settingsReader.GetValue("AdminMail", typeof(String));
                    string AdminName = (string)settingsReader.GetValue("AdminName", typeof(String));
                    string MailbodyAdmin = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/ContactUsAdmin.html"));  // email template Admin
                    MailbodyAdmin = MailbodyAdmin.Replace("###HostUrl####", baseurl);
                    MailbodyAdmin = MailbodyAdmin.Replace("###AdminUserName###", AdminName);
                    MailbodyAdmin = MailbodyAdmin.Replace("###UserName###", form.ContactUsName);
                    MailbodyAdmin = MailbodyAdmin.Replace("###UserPhone###", form.ContactUsPhone);
                    MailbodyAdmin = MailbodyAdmin.Replace("###QueryMessage###", form.Query);
                    if (!string.IsNullOrEmpty(form.ContactUsEmail))
                    {
                       // Helper.SendEmail(AdminMail, MailbodyAdmin, "Contact Us Email", ""); //email template Admin
                    }

                    TempData["MessageTypeCont"] = "success";
                    return RedirectToAction("Index", "ContactUs");
                }
                else
                {
                    TempData["MessageTypeCont"] = "error";
                    CmsManagement cmsObj = new CmsManagement();
                    var contactUsContent = cmsObj.GetContactUsContent();
                    ViewBag.ContactUsContent = contactUsContent;
                    return View();

                }

            }
            catch (Exception ex)
            {
                TempData["MessageTypeCont"] = "error";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return View();
        }

        public ActionResult GotoHome()
        {
            return RedirectToAction("Index", "Home");
        }
        //[HttpPost]
        //public ActionResult ContactUs(EnquiryForm form)
        //{
        //    try
        //    {
        //        UserManagement um = new UserManagement();
        //        ContactUsEnquiry obj = new ContactUsEnquiry()
        //        {
        //            Email = form.ContactUsEmail,
        //            Message = form.Query,
        //            Name = form.ContactUsName,
        //            Phone = form.ContactUsPhone,
        //            IsActive = true,
        //            AddedOn = DateTime.UtcNow,

        //        };
        //        int i = um.SaveEnquiry(obj);
        //        if (i > 0)
        //        {
        //            //Helper.ContactMail(form.ContactUsName, form.ContactUsPhone, form.ContactUsEmail, form.Query);
        //            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
        //            string baseurl = Helper.GetBaseUrl();
        //            string MailbodyUser = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/ContactUs.html"));  // email template User
        //            MailbodyUser = MailbodyUser.Replace("###HostUrl####", baseurl);
        //            MailbodyUser = MailbodyUser.Replace("###UserName###", form.ContactUsName);
        //            if (!string.IsNullOrEmpty(form.ContactUsEmail))
        //            {
        //                Helper.SendEmail(form.ContactUsEmail, MailbodyUser, "Contact Us Email", ""); //email User
        //            }

        //            string AdminMail = (string)settingsReader.GetValue("AdminMail", typeof(String));
        //            string AdminName = (string)settingsReader.GetValue("AdminName", typeof(String));
        //            string MailbodyAdmin = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/ContactUsAdmin.html"));  // email template Admin
        //            MailbodyAdmin = MailbodyAdmin.Replace("###HostUrl####", baseurl);
        //            MailbodyAdmin = MailbodyAdmin.Replace("###AdminUserName###", AdminName);
        //            MailbodyAdmin = MailbodyAdmin.Replace("###UserName###", form.ContactUsName);
        //            MailbodyAdmin = MailbodyAdmin.Replace("###UserPhone###", form.ContactUsPhone);
        //            MailbodyAdmin = MailbodyAdmin.Replace("###QueryMessage###", form.Query);
        //            if (!string.IsNullOrEmpty(form.ContactUsEmail))
        //            {
        //                // Helper.SendEmail(AdminMail, MailbodyAdmin, "Contact Us Email", ""); //email template Admin
        //            }
        //            TempData["MessageType"] = "success";
        //            TempData["EnquiryMessage"] = "Thank you for contacting us. We will contact you soon.";
        //        }
        //        else
        //        {
        //            TempData["MessageType"] = "error";
        //            TempData["EnquiryMessage"] = "Some error occurred while processing the request. Please try again";

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["MessageType"] = "Error";
        //        TempData["EnquiryMessage"] = "Some error occurred while processing request.";
        //        Helper.ExceptionHandler.WriteToLogFile(ex.Message);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

    }
}
