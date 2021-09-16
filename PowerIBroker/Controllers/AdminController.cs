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
using System.Web;
using System.Web.Mvc;
using PagedList;

using PowerIBrokerBusinessLayer.Admin;
using PowerIBrokerBusinessLayer.UserLogin;
using PowerIBroker.Areas.Broker.Models;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace PowerIBroker.Controllers
{

    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        ErrorLog ObjElog = new ErrorLog();
        CompanyManagement objClientMngmt = new CompanyManagement();
        ManagePageSize pagesize = new ManagePageSize();
        CommonMasters cmm = new CommonMasters();
        Comman objComman = new Comman();
        UserLoginManagement objuser = new UserLoginManagement();
        ClientMasterValidation objClientValid = new ClientMasterValidation();
        Administration objAdmin = new Administration();
        public ActionResult Index()
        {
            try
            {
                //DataSet ds = new DataSet();
                //var connectionString = ConfigurationManager.ConnectionStrings["PowerIBrokerDb"].ConnectionString;

                //using (SqlConnection connection = new SqlConnection(connectionString))
                //{
                //    SqlCommand cmd = new SqlCommand();
                //    SqlDataAdapter da = new SqlDataAdapter();
                //    cmd = new SqlCommand("psp_GetAllEmployeeDynamicInfo", connection);
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.CommandTimeout = 200;
                //    cmd.Parameters.AddWithValue("@CompanyId", 11);
                //    da = new SqlDataAdapter(cmd);
                //    da.Fill(ds);
                    
                //    connection.Close();
                //}

                Session.Abandon();
                Session["CompanyID"] = null;
                Session["EmployeeID"] = null;

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(AdminLogin obj)
        {

            Administration objAdmin = new Administration();
            var userType = objAdmin.CheckUserExistenceForAdmin(obj.EmailOrUserName);

            if (userType != null)
            {
                if (userType.EntityType == "A")
                {
                    var adminInfo = objAdmin.AdminLogin(obj.EmailOrUserName, Helper.Encrypt(obj.Password));
                    if (Session["CompanyId"] == null && Session["EmployeeId"] == null)
                    {
                        if (adminInfo == null)
                        {
                            TempData["MessageTypeAL"] = "Error";
                            TempData["CustomMessageAL"] = "Invalid login credentials, please try again.";
                        }
                        else
                        {
                            Session["CultureName"] = obj.CultureName == null ? "en-US" : obj.CultureName;
                            Session["CultureSymbol"] = System.Globalization.CultureInfo.GetCultureInfo(Convert.ToString(Session["CultureName"])).NumberFormat.CurrencySymbol;

                            Session["AdminUsername"] = adminInfo.Username;
                            Session["AdminEmail"] = adminInfo.Email;
                            Session["AdminID"] = adminInfo.Id;
                            return Redirect("/Super-Admin");
                            //return RedirectToAction("Dashboard");
                        }
                    }
                    else
                    {
                        TempData["MessageTypeAL"] = "Error";
                        TempData["CustomMessageAL"] = "You can not log in as another admin/user already logged in.";
                    }
                }


            }
            else
            {
                TempData["MessageTypeAL"] = "Error";
                TempData["CustomMessageAL"] = "Invalid login credentials, please try again.";
            }


            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session["CompanyID"] = null;
            Session["EmployeeID"] = null;
            return RedirectToAction("Index", "Admin");
        }
        [sessionexpireattribute]

        public ActionResult Dashboard()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            ManageDashboard objdash = new ManageDashboard();

            var ComCount = objdash.GetCompanyCount();
            var EmpCount = objdash.GetEmployeeCount();
            var DreqCount = objdash.GetDemoRequestCount();
            var RegCompany = objdash.GetRecentRegCompanyList();
            var DemoRequest = objdash.GetDemoRequestList();

            ViewBag.CompanyCount = ComCount;
            ViewBag.EmployeeCount = EmpCount;
            ViewBag.DemoReqCount = DreqCount;
            ViewBag.RegisterCompany = RegCompany;
            ViewBag.DemoRequest = DemoRequest;

            return View();
        }
        [sessionexpireattribute]
        public ActionResult ManageHomePageContent()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var homePageContent = cmsObj.GetHomePageContent();
            return View(homePageContent);
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult ManageHomePageContent(HomePageContent obj, HttpPostedFileBase file)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                string fileName = string.Empty;

                if (file != null)
                    obj.Banner = DateTime.UtcNow.Ticks + "-" + file.FileName;
                int i = cmsObj.SaveHomePageContent(obj);
                if (i > 0)
                {
                    if (obj.Banner != null)
                    {

                        fileName = obj.Banner;
                    }
                    if (file != null)
                    {
                        var path = Server.MapPath("~/Uploads/Admin/HomePageBanners/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                    }

                    TempData["MessageTypeHPC"] = "Success";
                    TempData["CustomMessageHPC"] = "Page content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeHPC"] = "Error";
                    TempData["CustomMessageHPC"] = "Some error occurred while processing request.";
                }


            }
            catch (Exception ex)
            {
                TempData["MessageTypeHPC"] = "Error";
                TempData["CustomMessageHPC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageHomePageContent");
        }

        [sessionexpireattribute]
        public ActionResult ManageHomePageClientBanners(string Id = null)
        {

            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var homePageClientBanners = cmsObj.GetHomePageBannerClients(true);
            ViewBag.HomePageClientBanners = homePageClientBanners;
            ViewBag.ActionType = "save";
            if (!string.IsNullOrEmpty(Id))
            {

                var homePageClientBanner = cmsObj.GetHomePageBannerClient(Convert.ToInt32(Id));
                if (homePageClientBanner != null)
                    ViewBag.ActionType = "update";
                return View(homePageClientBanner);
            }


            return View();

        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult ManageHomePageClientBanners(string actionType, HomePageBannerClient obj, HttpPostedFileBase file, bool? IsActive = null)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                string fileName = string.Empty;
                obj.IsActive = IsActive == null ? false : IsActive;
                if (file != null)
                    obj.Banner = DateTime.UtcNow.Ticks + "-" + obj.Banner;
                int i = 0;
                if (actionType == "save")
                    i = cmsObj.SaveHomePageBannerClient(obj);
                else
                {
                    i = cmsObj.UpdateHomePageBannerClient(obj.Id, obj);
                    i++;
                }

                if (i > 0)
                {
                    if (file != null)
                    {
                        fileName = obj.Banner;
                        var path = Server.MapPath("~/Uploads/Admin/HomePageClientBanners/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                    }
                    TempData["MessageTypeHPCB"] = "Success";
                    TempData["CustomMessageHPCB"] = "Client banner successfully " + (i == 1 ? "added" : "updated");
                }
                else
                {
                    TempData["MessageTypeHPCB"] = "Error";
                    TempData["CustomMessageHPCB"] = "Some error occurred while processing request.";
                }
            }
            catch (Exception ex)
            {
                TempData["MessageTypeHPCB"] = "Error";
                TempData["CustomMessageHPCB"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageHomePageClientBanners", new { Id = "" });
        }

        [sessionexpireattribute]
        public ActionResult ManageBenefitsPageContent()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var benefitsContent = cmsObj.GetBenefitsPageContent();
            return View(benefitsContent);
        }

        [ValidateInput(false)]
        [HttpPost]
        [sessionexpireattribute]
        public ActionResult ManageBenefitsPageContent(BenefitsPageContent obj, HttpPostedFileBase file)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                string fileName = string.Empty;
                if (file != null)
                    obj.BackgroundImage = DateTime.UtcNow.Ticks + "-" + obj.BackgroundImage;
                int i = cmsObj.SaveBenefitsPageContent(obj);
                if (i > 0)
                {
                    if (obj.BackgroundImage != null)
                    {
                        fileName = obj.BackgroundImage;
                    }
                    if (file != null)
                    {
                        var path = Server.MapPath("~/Uploads/Admin/BenefitsBackground/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                    }
                    TempData["MessageTypeMBPC"] = "Success";
                    TempData["CustomMessageMBPC"] = "Benefits page content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeMBPC"] = "Error";
                    TempData["CustomMessageMBPC"] = "Some error occurred while processing request.";
                }


            }
            catch (Exception ex)
            {
                TempData["MessageTypeMBPC"] = "Error";
                TempData["CustomMessageMBPC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageBenefitsPageContent");
        }
        [sessionexpireattribute]
        public ActionResult ManageRequestDemoContent()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var requestDemoContent = cmsObj.GetRequestDemoContent();
            return View(requestDemoContent);
        }

        [HttpPost]
        [sessionexpireattribute]
        public ActionResult ManageRequestDemoContent(RequestDemoContent obj, HttpPostedFileBase[] files)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                string imagefileName = string.Empty;
                string videofileName = string.Empty;
                if (files[0] != null)
                    obj.BackgroundImage = DateTime.UtcNow.Ticks + "-" + obj.BackgroundImage;

                if (files[1] != null)
                {
                    obj.Video = DateTime.UtcNow.Ticks + "-" + obj.Video;
                    // obj.Video = Path.GetFileNameWithoutExtension(obj.Video);
                }
                int i = cmsObj.SaveRequestDemoContent(obj);
                if (i > 0)
                {
                    if (obj.BackgroundImage != null)
                    {
                        imagefileName = obj.BackgroundImage;
                    }
                    if (files[0] != null)
                    {
                        var path = Server.MapPath("~/Uploads/Admin/RequestDemoBackgrounds/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(imagefileName));
                        files[0].SaveAs(path);
                    }
                    if (obj.Video != null)
                        videofileName = obj.Video;
                    if (files[1] != null)
                    {

                        var path = Server.MapPath("~/Uploads/Admin/RequestDemoVideos/mp4");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(videofileName));
                        files[1].SaveAs(path);
                    }

                    TempData["MessageTypeMRDC"] = "Success";
                    TempData["CustomMessageMRDC"] = "Request demo content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeMRDC"] = "Error";
                    TempData["CustomMessageMRDC"] = "Some error occurred while processing request.";
                }


            }
            catch (Exception ex)
            {
                TempData["MessageTypeMRDC"] = "Error";
                TempData["CustomMessageMRDC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageRequestDemoContent");
        }
        [sessionexpireattribute]
        public ActionResult ManageFeatureContents(string Id = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var featureContents = cmsObj.GetFeaturesContent(true);
            ViewBag.FeatureContents = featureContents;
            ViewBag.ActionType = "save";
            if (!string.IsNullOrEmpty(Id))
            {

                var featureContent = cmsObj.GetFeaturesContent(Convert.ToInt32(Id));
                if (featureContent != null)
                    ViewBag.ActionType = "update";
                return View(featureContent);
            }


            return View();

        }

        [HttpPost]
        [sessionexpireattribute]
        public ActionResult ManageFeatureContents(string actionType, FeaturesPageContent obj, HttpPostedFileBase file, bool? IsActive = null)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                string fileName = string.Empty;
                obj.IsActive = IsActive == null ? false : IsActive;
                if (file != null)
                    obj.Image = DateTime.UtcNow.Ticks + "-" + obj.Image;
                int i = 0;
                if (actionType == "save")
                    i = cmsObj.SaveFeaturesContent(obj);
                else
                {
                    i = cmsObj.UpdateFeaturesPageContent(obj.Id, obj);
                    i++;
                }

                if (i > 0)
                {
                    if (file != null)
                    {
                        fileName = obj.Image;
                        var path = Server.MapPath("~/Uploads/Admin/HomePageFeatureIcons/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                    }
                    TempData["MessageTypeMFC"] = "Success";
                    TempData["CustomMessageMFC"] = "Feature content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeMFC"] = "Error";
                    TempData["CustomMessageMFC"] = "Some error occurred while processing request.";
                }
            }
            catch (Exception ex)
            {
                TempData["MessageTypeMFC"] = "Error";
                TempData["CustomMessageMFC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageFeatureContents", new { Id = "" });
        }
        [sessionexpireattribute]
        public ActionResult ManageHomePageTopContents(string Id = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var homePageTopContents = cmsObj.GetHomePageTopContent();
            ViewBag.HomePageTopContents = homePageTopContents;
            ViewBag.ActionType = "save";
            if (!string.IsNullOrEmpty(Id))
            {

                var homePageTopContent = cmsObj.GetHomePageTopContent(Convert.ToInt32(Id));
                if (homePageTopContent != null)
                    ViewBag.ActionType = "update";
                return View(homePageTopContent);
            }


            return View();

        }

        [HttpPost]
        [sessionexpireattribute]
        public ActionResult ManageHomePageTopContents(string actionType, HomePageTopContent obj, HttpPostedFileBase file)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                string fileName = string.Empty;
                if (file != null)
                    obj.Image = DateTime.UtcNow.Ticks + "-" + obj.Image;
                int i = 0;
                if (actionType == "save")
                    i = cmsObj.SaveHomePageTopContent(obj);
                else
                {
                    i = cmsObj.UpdateHomePageTopContent(obj.Id, obj);
                    i++;
                }

                if (i > 0)
                {
                    if (file != null)
                    {
                        fileName = obj.Image;
                        var path = Server.MapPath("~/Uploads/Admin/HomePageTopContentIcons/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                    }
                    TempData["MessageTypeMHPTC"] = "Success";
                    TempData["CustomMessageMHPTC"] = "Homepage top content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeMHPTC"] = "Error";
                    TempData["CustomMessageMHPTC"] = "Some error occurred while processing request.";
                }
            }
            catch (Exception ex)
            {
                TempData["MessageTypeMHPTC"] = "Error";
                TempData["CustomMessageMHPTC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageHomePageTopContents", new { Id = "" });
        }

        [sessionexpireattribute]
        public ActionResult ManagePricingPageContent()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var pricingPageContent = cmsObj.GetPricingPageContent();
            return View(pricingPageContent);
        }
        [HttpPost]
        [sessionexpireattribute]
        public ActionResult ManagePricingPageContent(PricingPageContent obj, HttpPostedFileBase file)
        {
            try
            {
                CmsManagement cmsObj = new CmsManagement();
                string fileName = string.Empty;
                if (file != null)
                    obj.BackgroundImage = DateTime.UtcNow.Ticks + "-" + obj.BackgroundImage;
                int i = cmsObj.SavePricingPageContent(obj);
                if (i > 0)
                {
                    if (obj.BackgroundImage != null)
                    {

                        fileName = obj.BackgroundImage;
                    }
                    if (file != null)
                    {
                        var path = Server.MapPath("~/Uploads/Admin/HomePagePricingContentBanners/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                    }

                    TempData["MessageTypeMPPC"] = "Success";
                    TempData["CustomMessageMPPC"] = "Pricing page content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeMPPC"] = "Error";
                    TempData["CustomMessageMPPC"] = "Some error occurred while processing request.";
                }


            }
            catch (Exception ex)
            {
                TempData["MessageTypeMPPC"] = "Error";
                TempData["CustomMessageMPPC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManagePricingPageContent");
        }
        [sessionexpireattribute]
        public ActionResult ManageAboutUsContent()
        {
            CmsManagement cmsObj = new CmsManagement();
            var aboutUsContent = cmsObj.GetAboutUsPageContent();
            return View(aboutUsContent);
        }

        [ValidateInput(false)]
        [HttpPost]
        [sessionexpireattribute]
        public ActionResult ManageAboutUsContent(string actionType, AboutUsPageContent obj, HttpPostedFileBase[] files)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                string headerBanner = string.Empty;
                string bottomLeftSectionImage = string.Empty;
                string bottomMiddleSectionImage = string.Empty;
                string bottomRightSectionImage = string.Empty;
                if (files[0] != null)
                    //obj.HeaderBanner = DateTime.UtcNow.Ticks + "-" + obj.HeaderBanner;
                    obj.HeaderBanner = DateTime.UtcNow.Ticks + "-" + files[0].FileName;
                if (files[1] != null)
                    obj.BottomLeftSectionImage = DateTime.UtcNow.Ticks + "-" + files[1].FileName;
                if (files[2] != null)
                    obj.BottomMiddleSectionImage = DateTime.UtcNow.Ticks + "-" + files[2].FileName;
                if (files[3] != null)
                    obj.BottomRightSectionImage = DateTime.UtcNow.Ticks + "-" + files[3].FileName;
                int i = cmsObj.SaveAboutUsPageContent(obj);
                if (i > 0)
                {
                    if (files[0] != null)
                    {
                        headerBanner = obj.HeaderBanner;
                        var path = Server.MapPath("~/Uploads/Admin/AboutUsBanners/Header");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(headerBanner));
                        files[0].SaveAs(path);
                    }
                    if (files[1] != null)
                    {
                        bottomLeftSectionImage = obj.BottomLeftSectionImage;
                        var path = Server.MapPath("~/Uploads/Admin/AboutUsBanners/BottomLeftSection");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(bottomLeftSectionImage));
                        files[1].SaveAs(path);
                    }
                    if (files[2] != null)
                    {
                        bottomMiddleSectionImage = obj.BottomMiddleSectionImage;
                        var path = Server.MapPath("~/Uploads/Admin/AboutUsBanners/BottomMiddleSection");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(bottomMiddleSectionImage));
                        files[2].SaveAs(path);
                    }
                    if (files[3] != null)
                    {
                        bottomRightSectionImage = obj.BottomRightSectionImage;
                        var path = Server.MapPath("~/Uploads/Admin/AboutUsBanners/BottomRightSection");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(bottomRightSectionImage));
                        files[3].SaveAs(path);
                    }
                    TempData["MessageTypeMAUC"] = "Success";
                    TempData["CustomMessageMAUC"] = "Aboutus content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeMAUC"] = "Error";
                    TempData["CustomMessageMAUC"] = "Some error occurred while processing request.";
                }
            }
            catch (Exception ex)
            {
                TempData["MessageTypeMAUC"] = "Error";
                TempData["CustomMessageMAUC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageAboutUsContent");
        }
        [sessionexpireattribute]
        public ActionResult ManageFaqContent(string Id = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var faqPageContents = cmsObj.GetFaqPageContent(true);
            ViewBag.FaqBanner = string.Empty;

            var faqBanner = cmsObj.getFaqBanner(true);
            if (!string.IsNullOrEmpty(faqBanner))
            {
                ViewBag.FaqBanner = faqBanner;
            }

            ViewBag.FaqPageContents = faqPageContents;
            ViewBag.ActionType = "save";
            if (!string.IsNullOrEmpty(Id))
            {

                var faqPageContent = cmsObj.GetFaqPageContent(Convert.ToInt32(Id));
                if (faqPageContent != null)
                    ViewBag.ActionType = "update";
                return View(faqPageContent);
            }


            return View();
        }

        [HttpPost]
        [sessionexpireattribute]
        public ActionResult ManageFaqContent(string actionType, FaqPageContent obj, HttpPostedFileBase file, bool? IsActive = null)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                string fileName = string.Empty;
                if (file != null)
                    obj.Banner = DateTime.UtcNow.Ticks + "-" + obj.Banner;
                int i = 0;
                obj.IsActive = IsActive == null ? false : IsActive;
                if (actionType == "save")
                    i = cmsObj.SaveFaqPageContent(obj);
                else
                {
                    i = cmsObj.UpdateFaqPageContent(obj.Id, obj);
                    i++;
                }

                if (i > 0)
                {
                    if (file != null)
                    {
                        fileName = obj.Banner;
                        var path = Server.MapPath("~/Uploads/Admin/FAQBannners/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                    }
                    TempData["MessageTypeMFAQC"] = "Success";
                    TempData["CustomMessageMFAQC"] = "FAQ content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeMFAQC"] = "Error";
                    TempData["CustomMessageMFAQC"] = "Some error occurred while processing request.";
                }
            }
            catch (Exception ex)
            {
                TempData["MessageTypeMFAQC"] = "Error";
                TempData["CustomMessageMFAQC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageFaqContent", new { Id = "" });
        }
        [sessionexpireattribute]
        public ActionResult ManageOtherPages(string Id = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var otherPagesContent = cmsObj.GetOtherPageContent();
            ViewBag.OtherPagesContent = otherPagesContent;
            ViewBag.ActionType = "save";
            if (!string.IsNullOrEmpty(Id))
            {

                var otherPageContent = cmsObj.GetOtherPageContent(Convert.ToInt32(Id), true);
                if (otherPageContent != null)
                    ViewBag.ActionType = "update";
                return View(otherPageContent);
            }


            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        [sessionexpireattribute]
        public ActionResult ManageOtherPages(string actionType, OtherPageContent obj, HttpPostedFileBase file, bool? IsActive = null)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                string fileName = string.Empty;
                CmsManagement cmsObj = new CmsManagement();
                obj.IsActive = IsActive == null ? false : IsActive;
                if (file != null)
                    obj.Banner = DateTime.UtcNow.Ticks + "-" + obj.Banner;
                int i = 0;
                if (actionType == "save")
                    i = cmsObj.SaveOtherPageContent(obj);
                else
                {
                    i = cmsObj.UpdateOtherPageContent(obj.Id, obj);
                    i++;
                }

                if (i > 0)
                {
                    if (file != null)
                    {
                        fileName = obj.Banner;
                        var path = Server.MapPath("~/Uploads/Admin/OtherPageBanners/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                    }
                    TempData["MessageTypeMOP"] = "Success";
                    TempData["CustomMessageMOP"] = "Page content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeMOP"] = "Error";
                    TempData["CustomMessageMOP"] = "Some error occurred while processing request.";
                }
            }
            catch (Exception ex)
            {
                TempData["MessageTypeMOP"] = "Error";
                TempData["CustomMessageMOP"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageOtherPages", new { Id = "" });
        }
        [sessionexpireattribute]
        public ActionResult ManageCompanies()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            Administration objAdmin = new Administration();
            var users = objAdmin.GetUsers();
            if (users.Count > 0)
            {
                foreach (var user in users)
                {
                    user.Password = Helper.Decrypt(user.Password);
                }
            }
            ViewBag.Users = users;
            return View();
        }
        [sessionexpireattribute]
        public ActionResult DeleteCompany(int Id)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                Administration objAdmin = new Administration();

                int i = objAdmin.DeleteUser(Id);
                if (i > 0)
                {
                    TempData["MessageType"] = "Success";
                    TempData["CustomMessage"] = "User successfully deleted";
                }
                else
                {
                    TempData["MessageType"] = "Error";
                    TempData["CustomMessage"] = "Some error occurred while processing request.";
                }
            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageCompanies");
        }
        [sessionexpireattribute]
        public ActionResult ManageContactUsContent()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var contactUsContent = cmsObj.GetContactUsContent();
            return View(contactUsContent);
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult ManageContactUsContent(ContactUsPageContent obj, HttpPostedFileBase file)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                string fileName = string.Empty;

                if (file != null)
                    obj.Banner = DateTime.UtcNow.Ticks + "-" + file.FileName;
                int i = cmsObj.SaveContactUsContent(obj);
                if (i > 0)
                {
                    if (obj.Banner != null)
                    {

                        fileName = obj.Banner;
                    }
                    if (file != null)
                    {
                        var path = Server.MapPath("~/Uploads/Admin/ContactUsBanners/");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path = Path.Combine(path,
                                            System.IO.Path.GetFileName(fileName));
                        file.SaveAs(path);
                    }

                    TempData["MessageTypeMCUC"] = "Success";
                    TempData["CustomMessageMCUC"] = "ContactUs content successfully " + (i == 1 ? "added" : "updated") + ".";
                }
                else
                {
                    TempData["MessageTypeMCUC"] = "Error";
                    TempData["CustomMessageMCUC"] = "Some error occurred while processing request.";
                }


            }
            catch (Exception ex)
            {
                TempData["MessageTypeMCUC"] = "Error";
                TempData["CustomMessageMCUC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageContactUsContent");
        }
        [sessionexpireattribute]
        public ActionResult ManageContactUsEnquiries(int page = 1)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            UserManagement um = new UserManagement();
            var contactUsEnquiries = um.GetEnquiries();

            um.UpdateEnquiryStatus();//Updating Enquiry Status
            return View(contactUsEnquiries.ToPagedList(page, pagesize.ManageContactUsEnquiriesPageSize));
        }

        [sessionexpireattribute]
        public ActionResult ManageCompanyDemos(int page = 1)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            UserManagement um = new UserManagement();
            var contactUsEnquiries = um.GetCompanyDemos();

            return View(contactUsEnquiries.ToPagedList(page, pagesize.ManageComapanyPageSize));
        }
        [sessionexpireattribute]
        public ActionResult DeleteContactUsEnquiry(int Id)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            UserManagement um = new UserManagement();
            int i = um.DeleteEnquiry(Id);
            if (i > 0)
            {
                TempData["MessageType"] = "Success";
                TempData["CustomMessage"] = "Enquiry successfully deleted.";
            }
            else
            {
                TempData["MessageType"] = "Error";
                TempData["CustomMessage"] = "Some error occurred while processing request.";
            }

            return RedirectToAction("ManageContactUsEnquiries");
        }
        [sessionexpireattribute]
        public ActionResult ManageSocialMediaLinks(string Id = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var socialMediaLinks = cmsObj.GetSocialMediaLink(true);
            ViewBag.SocialMediaLinks = socialMediaLinks;
            ViewBag.ActionType = "save";
            if (!string.IsNullOrEmpty(Id))
            {

                var socialMediaLink = cmsObj.GetSocialMediaLink(Convert.ToInt32(Id));
                if (socialMediaLink != null)
                    ViewBag.ActionType = "update";
                return View(socialMediaLink);
            }


            return View();
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult ManageSocialMediaLinks(string actionType, SocialMediaLink obj, bool? IsActive = null)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                int i = 0;
                obj.IsActive = IsActive == null ? false : IsActive;
                if (actionType == "save")
                    i = cmsObj.SaveSocialMediaLink(obj);
                else
                {
                    i = cmsObj.UpdateSocialMediaLink(obj.Id, obj);
                }

                if (i == 0)
                {
                    TempData["MessageTypeMSML"] = "Error";
                    TempData["CustomMessageMSML"] = "Some error occurred while processing request.";

                }
                else
                {
                    if (i == 2)
                    {
                        TempData["MessageTypeMSML"] = "Error";
                        TempData["CustomMessageMSML"] = "Social media link with title '<i>" + obj.Title + "</i>' already exists. !";
                    }
                    else
                    {
                        TempData["MessageTypeMSML"] = "Success";
                        TempData["CustomMessageMSML"] = "Social media link successfully " + (i == 1 ? "added" : "updated") + ".";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MessageTypeMSML"] = "Error";
                TempData["CustomMessageMSML"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageSocialMediaLinks", new { Id = "" });
        }
        [sessionexpireattribute]
        public ActionResult ManageNewsContent(string Id = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CmsManagement cmsObj = new CmsManagement();
            var newsContents = cmsObj.GetNewsContent(true);
            ViewBag.NewsContents = newsContents;
            ViewBag.ActionType = "save";
            if (!string.IsNullOrEmpty(Id))
            {

                var newsContent = cmsObj.GetNewsContent(Convert.ToInt32(Id));
                if (newsContent != null)
                    ViewBag.ActionType = "update";
                return View(newsContent);
            }

            return View();
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult ManageNewsContent(string actionType, NewsContent obj, bool? IsActive = null)
        {
            try
            {
                if (Session["AdminEmail"] == null)
                    return RedirectToAction("Index");
                CmsManagement cmsObj = new CmsManagement();
                int i = 0;
                obj.IsActive = IsActive == null ? false : IsActive;
                if (actionType == "save")
                    i = cmsObj.SaveNewsContent(obj);
                else
                {
                    i = cmsObj.UpdateNewsContent(obj.Id, obj);
                }

                if (i > 0)
                {
                    TempData["MessageTypeMNC"] = "Success";
                    TempData["CustomMessageMNC"] = "News content successfully " + (i == 1 ? "added" : "updated") + ".";

                }
                else
                {
                    TempData["MessageTypeMNC"] = "Error";
                    TempData["CustomMessageMNC"] = "Some error occurred while processing request.";
                }
            }
            catch (Exception ex)
            {
                TempData["MessageTypeMNC"] = "Error";
                TempData["CustomMessageMNC"] = "Some error occurred while processing request.";
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            return RedirectToAction("ManageNewsContent", new { Id = "" });
        }

        //Add Companies Section
        [sessionexpireattribute]
        public ActionResult AddClient()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            return View();
        }

        [sessionexpireattribute]
        [HttpPost]
        public ActionResult AddClient(CompanyMaster objCMaster, string CCityName, string CStateName, string CCountryName, string status = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            string strCountry, strState, strCity = string.Empty;
            long lngCountryID, lngStateID, lngCityID = 0;

            try
            {
                if (ModelState.IsValid)
                {
                    if (Session["AdminEmail"] == null)
                        return RedirectToAction("Index");
                    string admID = Session["AdminID"].ToString();
                    string body_Demo_request = string.Empty;
                    if (!string.IsNullOrEmpty(admID))
                    {
                        objCMaster.CContact = objCMaster.CContact.Replace("-", "");
                        string randomPassword = Helper.CreateRandomPassword(8);
                        strCountry = CCountryName;
                        strState = CStateName;
                        strCity = CCityName;
                        lngCountryID = objComman.GetCountry(strCountry);
                        lngStateID = objComman.GetState(strState, lngCountryID);
                        lngCityID = objComman.GetCity(strCity, lngStateID, lngCountryID);
                        long adminID = Convert.ToInt64(admID);
                        objCMaster.CPassword = null;/* Helper.Encrypt(randomPassword);*/
                        objCMaster.CCity = lngCityID;
                        objCMaster.CState = lngStateID;
                        objCMaster.CCountry = lngCountryID;
                        objCMaster.IsActive = objCMaster.IsActive == null ? false : true;
                        objCMaster.OwnerId = adminID;
                        objCMaster.OwnerType = "A";
                        objCMaster.Status = "A";
                        objCMaster.NoOfAttempts = 0;
                        var name = objCMaster.CName;
                        var email = objCMaster.CEmail;

                        var phone = objCMaster.CContact;
                        //var phone = objCMaster.CContact;

                        objCMaster.CName = null;
                        //objCMaster.CEmail = null;
                        //objCMaster.CContact = null;
                        int result = objClientMngmt.AddClient(objCMaster);
                        if (result > 0)
                        {

                            CompanyLocation cl = new CompanyLocation()
                            {
                                CCity = objCMaster.CCity,
                                CState = objCMaster.CState,
                                CStreetName = objCMaster.CStreetName,
                                CContact = objCMaster.CContact,
                                CCountry = objCMaster.CCountry,
                                CZipCode = objCMaster.CZipCode,
                                CompanyId = objCMaster.ID,
                                CreatedOn = DateTime.UtcNow
                            };
                            //adding company location information to the master location
                            objClientMngmt.AddCompanyLocationInfo(cl);
                            if (result == 1 && objCMaster.ID > 0)//
                            {

                                TempData["MessageType"] = "Success";
                                TempData["CustomMessage"] = "Company saved successfully.";

                                //Helper.EmailConfirmation(objCMaster.CEmail, 1, randomPassword);

                                //////////////////////////////////////////////////////////////


                                tblCompany_Employee_BasicInfo objComEmpBasicInfo = new tblCompany_Employee_BasicInfo();
                                tblCompany_Employee_PersonalInfo objComEmpPerInfo = new tblCompany_Employee_PersonalInfo();
                                objComEmpBasicInfo.FirstName = name;
                                objComEmpBasicInfo.Email = email;
                                objComEmpPerInfo.Phone = phone;
                                objComEmpBasicInfo.IsActive = false;
                                objComEmpBasicInfo.Password = null;
                                objComEmpBasicInfo.RoleID = 1;
                                objComEmpBasicInfo.CompanyId = objCMaster.ID;
                                objComEmpBasicInfo.IsArchive = false;
                                objComEmpBasicInfo.IsTerminate = false;
                                tblCompany_Employee_EmploymentInfo objComEmpEmpInfo = new tblCompany_Employee_EmploymentInfo();
                                new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement().AddEmployeeBulkDetails(objComEmpBasicInfo, objComEmpPerInfo, objComEmpEmpInfo);
                                //result = objComEmpMgnt.AddEmployeeBulkDetails(objComEmpBasicInfo, objComEmpPerInfo, null);

                                //string baseurl = Helper.GetBaseUrl();
                                //string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/Add_New_Company.html"));  // email template
                                //Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                                //Mailbody = Mailbody.Replace("###LoginUserName###", objCMaster.CEmail);
                                //Mailbody = Mailbody.Replace("###UserName###", objCMaster.CName);
                                //Mailbody = Mailbody.Replace("###Password###", Helper.Decrypt(objCMaster.CPassword));
                                //Mailbody = Mailbody.Replace("###UserNAME###", Helper.Encrypt(objCMaster.CEmail));
                                //Mailbody = Mailbody.Replace("###CompanyNAME###", objCMaster.CCompanyName);
                                //Helper.SendEmail(objCMaster.CEmail, Mailbody, "Welcome to EnrollMyGroup");

                                //int statusPhone = 0;
                                //if (!string.IsNullOrEmpty(objCMaster.CContact))
                                //{
                                //    statusPhone = SendTwilloMessage.SendmessageTwillo(objCMaster.CContact, "Dear " + objCMaster.CCompanyName + ", \n \n Welcome to Enroll My Group. You have been designated for this account.\n \n Thanks \n Enroll My Group Team");
                                //}

                            }
                            else
                            {
                                TempData["MessageType"] = "Error";
                                TempData["CustomMessage"] = "Some error while processing your request.";

                            }
                        }
                        else
                        {
                            TempData["MessageType"] = "Error";
                            TempData["CustomMessage"] = "Email or phone no already exists.";
                        }
                        if (result == 1)
                        {
                            ModelState.AddModelError("AddClient", "Client added successfully.");
                            return RedirectToAction("AddClient", "Admin");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("AddClient", ex.Message);

            }
            return View();
        }
        [sessionexpireattribute]
        public ActionResult ManageClients(int? page, string sort = null, string filter = null, string pageSize = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            long admID = Convert.ToInt64(Session["AdminID"].ToString());
            string search = Request.Form["search"];
            //ViewBag.ClientList = objClientMngmt.GetAllClient();
            //return View();
            int _PageSize = pagesize.ManageComapanyPageSize;
            int _pageSize = Convert.ToInt32(_PageSize);
            int Size = Convert.ToInt32(pageSize);
            if (Size != null && Size != 0)
                Size = Convert.ToInt32(Size);
            int pageNumber = (page ?? 1);
            TempData["Page"] = pageNumber;
            var ClientData = objClientMngmt.GetAllBrokerClients(search, admID);
            ViewBag.PageSize = Size;
            ViewBag.Total = ClientData.Count;
            ViewBag.CurrentFilter = filter == null ? "clientname" : filter;
            sort = sort == null ? "asc" : sort;
            if (Size == 0)
            {
                Size = 50;
            }
            switch (filter)
            {
                case "clientname":
                    ClientData = sort == "desc" ? ClientData.OrderByDescending(s => s.CName).ToList() : ClientData.OrderBy(s => s.CName).ToList();
                    break;
                case "Employee":
                    ClientData = sort == "desc" ? ClientData.OrderByDescending(s => s.tblCompany_Employee_BasicInfo.Where(a => a.CompanyId == s.ID).Count()).ToList() : ClientData.OrderBy(s => s.tblCompany_Employee_BasicInfo.Where(a => a.CompanyId == s.ID).Count()).ToList();
                    break;
                case "status":
                    ClientData = sort == "desc" ? ClientData.OrderByDescending(s => s.IsActive).ToList() : ClientData.OrderBy(s => s.IsActive).ToList();
                    break;
            }
            ViewBag.CurrentSort = sort;
            sort = sort == "asc" ? "desc" : "asc";
            ViewBag.SortingOrder = sort;
            //ViewBag.BrokerEmpCount = objClientMngmt.GetAllClient().Count;
            if (Size != 1)
            {
                ViewBag.PageNumber = pageNumber > ClientData.Count ? ClientData.Count : pageNumber;
                ViewBag.PageLength = pageNumber * Size > ClientData.Count ? ClientData.Count : pageNumber * Size;
            }
            else
            {
                //ViewBag.PageNumber = ClientData.Count/2;
                ViewBag.PageLength = ClientData.Count;
                Size = ClientData.Count / 1000;
                if (Size == 0)
                {
                    Size = 1000;
                }

            }
            return View(ClientData.ToPagedList(pageNumber, Size));
        }


        [sessionexpireattribute]
        [HttpGet]
        public ActionResult ReportBySql()
        {
            return View();
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult ReportBySql(string txtQuery, string Command)
        {
            TempData["Query"] = txtQuery;
            if (txtQuery.ToLower().Contains("update ") || txtQuery.ToLower().Contains("delete ") || txtQuery.ToLower().Contains("truncate ") || txtQuery.ToLower().Contains("drop ") || txtQuery.ToLower().Contains("alter ") || txtQuery.ToLower().Contains("create "))
            {
                TempData["MessageTypesql"] = "Error";
                TempData["CustomMessagesql"] = "Please execute the select statement only.";

                return View();
            }
            DataTable dtFillGrid = new DataTable();
            try
            {
                string connectionstr = ConfigurationManager.ConnectionStrings["PowerIBrokerDb"].ToString();
                SqlConnection Connection = new SqlConnection(connectionstr);
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
                SqlDataAdapter da = new SqlDataAdapter(txtQuery.ToString(), Connection);
                DataSet dsFillGrid = new DataSet();
                da.Fill(dsFillGrid);
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }

                if (dsFillGrid.Tables.Count > 0)
                {
                    if (dsFillGrid.Tables[0].Rows.Count > 0)
                    {
                        if (Command == "Submit")
                        {
                            dtFillGrid = dsFillGrid.Tables[0];
                        }
                        if (Command == "Export")
                        {
                            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                            gv.DataSource = dsFillGrid;
                            gv.DataBind();
                            Response.ClearContent();
                            Response.Buffer = true;
                            Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
                            Response.ContentType = "application/ms-excel";
                            Response.Charset = "";
                            StringWriter sw = new StringWriter();
                            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
                            gv.RenderControl(htw);
                            Response.Output.Write(sw.ToString());
                            Response.Flush();
                            Response.End();

                        }
                        return View(dtFillGrid);

                    }
                    else
                    {
                        TempData["MessageTypesql"] = "Error";
                        TempData["CustomMessagesql"] = "No record found.";
                        return View();
                    }

                }
                else
                {
                    TempData["MessageTypesql"] = "Error";
                    TempData["CustomMessagesql"] = "No record found.";
                    return View();
                }



            }
            catch (Exception ex)
            {
                TempData["MessageTypesql"] = "Error";
                TempData["CustomMessagesql"] = ex.Message;
                return View();
            }
        }

        [sessionexpireattribute]
        [HttpGet]
        public ActionResult SqlQueryList()
        {
            return View();
        }

        [sessionexpireattribute]
        public ActionResult ResendEmail(string itemId)
        {

            long id = Convert.ToInt64(itemId);
            string result = string.Empty; ;
            var Compdetails = objClientMngmt.GetClientDetils(id);
            var empDetails = Compdetails.tblCompany_Employee_BasicInfo.Where(a => a.CompanyId == id && a.RoleID == 1).ToList();
            foreach (var item in empDetails)
            {
                if (!string.IsNullOrEmpty(item.Email))
                {
                    string baseurl = Helper.GetBaseUrl();
                    string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/Add_New_Company_Resend.html"));  // email template
                    Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                    Mailbody = Mailbody.Replace("###LoginUserName###", item.Email);
                    //Mailbody = Mailbody.Replace("###UserName###", Compdetails.CName);
                    Mailbody = Mailbody.Replace("###UserName###", item.FirstName);
                    //Mailbody = Mailbody.Replace("###Password###", Helper.Decrypt(objCMaster.CPassword));
                    Mailbody = Mailbody.Replace("###UserNAME###", Helper.Encrypt(item.Email));
                    Mailbody = Mailbody.Replace("###CompanyName###", Compdetails.CCompanyName);
                    if (!string.IsNullOrEmpty(item.Email))
                    {
                        // Date21Sept as per client Discussion code comment 
                        result = Helper.SendEmail(item.Email, Mailbody, "Welcome to EnrollMyGroup");
                        result = "Success";
                    }


                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [sessionexpireattribute]
        public ActionResult ManageEmployees(int? page, string sort = null, string filter = null, string pageSize = null, int CompanyID = 0)
        {

            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data = new SelectList(cdata, "ID", "CCompanyName", CompanyID);
            ViewBag.Company = data;
            long admID = Convert.ToInt64(Session["AdminID"].ToString());
            string search = Request.Form["search"];

            //ViewBag.ClientList = objClientMngmt.GetAllClient();
            //return View();

            int _PageSize = pagesize.ManageComapanyPageSize;
            int _pageSize = Convert.ToInt32(_PageSize);
            int Size = Convert.ToInt32(pageSize);
            if (Size != null && Size != 0)
                Size = Convert.ToInt32(Size);
            int pageNumber = (page ?? 1);
            TempData["Page"] = pageNumber;
            var ClientData = objClientMngmt.GetEmployees(CompanyID);
            ViewBag.PageSize = Size;
            ViewBag.Total = ClientData.Count;
            ViewBag.CurrentFilter = filter == null ? "clientname" : filter;

            if (Size == 0)
            {
                Size = 50;
            }
            if (Size != 1)
            {
                ViewBag.PageNumber = pageNumber > ClientData.Count ? ClientData.Count : pageNumber;
                ViewBag.PageLength = pageNumber * Size > ClientData.Count ? ClientData.Count : pageNumber * Size;
            }
            else
            {
                //ViewBag.PageNumber = ClientData.Count/2;
                ViewBag.PageLength = ClientData.Count;
                Size = ClientData.Count / 1000;
                if (Size == 0)
                {
                    Size = 1000;
                }

            }
            return View("ManageEmployees", ClientData.ToPagedList(pageNumber, Size));

        }

        [sessionexpireattribute]
        public ActionResult ManageCountEmployees(int? page, string sort = null, string filter = null, string pageSize = null, int ID = 0)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data = new SelectList(cdata, "ID", "CName");
            ViewBag.Company = data;
            long admID = Convert.ToInt64(Session["AdminID"].ToString());
            string search = Request.Form["search"];

            //ViewBag.ClientList = objClientMngmt.GetAllClient();
            //return View();
            long CompanyID = 0;
            CompanyID = ID;
            ViewBag.CompanyId = ID;
            int _PageSize = pagesize.ManageComapanyPageSize;
            int _pageSize = Convert.ToInt32(_PageSize);
            int Size = Convert.ToInt32(pageSize);
            if (Size != null && Size != 0)
                Size = Convert.ToInt32(Size);
            int pageNumber = (page ?? 1);
            TempData["Page"] = pageNumber;
            var ClientData = objClientMngmt.GetEmployees(CompanyID);
            ViewBag.PageSize = Size;
            ViewBag.Total = ClientData.Count;
            ViewBag.CurrentFilter = filter == null ? "clientname" : filter;

            if (Size == 0)
            {
                Size = 1000;
            }
            if (Size != 1)
            {
                ViewBag.PageNumber = pageNumber > ClientData.Count ? ClientData.Count : pageNumber;
                ViewBag.PageLength = pageNumber * Size > ClientData.Count ? ClientData.Count : pageNumber * Size;
            }
            else
            {
                //ViewBag.PageNumber = ClientData.Count/2;
                ViewBag.PageLength = ClientData.Count;
                Size = ClientData.Count / 1000;
                if (Size == 0)
                {
                    Size = 1000;
                }

            }
            return View(ClientData.ToPagedList(pageNumber, Size));
        }


        [sessionexpireattribute]
        public ActionResult EditClient(long Empid)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CompanyMaster objClientMst = new CompanyMaster();
            objClientMst = objClientMngmt.GetClientDetils(Empid);



            return View(objClientValid);
        }

        [sessionexpireattribute]
        [HttpPost]
        public ActionResult EditClient(CompanyMaster objCMaster, string CCityName, string CStateName, string CCountryName, string status = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            string strCountry, strState, strCity = string.Empty;
            long lngCountryID, lngStateID, lngCityID = 0;
            //long empID = Convert.ToInt64(Request.Form["empID"]);
            long empID = Convert.ToInt64(objCMaster.EmployeeID);
            try
            {
                if (ModelState.IsValid)
                {
                    Session["AdminID"] = "1";
                    string admID = Session["AdminID"].ToString();

                    if (!string.IsNullOrEmpty(admID))
                    {
                        var phone1 = objCMaster.CContact.Replace("-", "");
                        objCMaster.CContact = phone1;
                        var oldDetails = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement().GetCompanyInfo(objCMaster.ID);
                        strCountry = CCountryName;
                        strState = CStateName;
                        strCity = CCityName;

                        lngCountryID = objComman.GetCountry(strCountry);
                        lngStateID = objComman.GetState(strState, lngCountryID);
                        lngCityID = objComman.GetCity(strCity, lngStateID, lngCountryID);
                        long lngBrokerID = Convert.ToInt64(admID);


                        objCMaster.CCity = lngCityID;
                        objCMaster.CState = lngStateID;
                        objCMaster.CCountry = lngCountryID;
                        objCMaster.IsActive = status == null ? false : true;
                        objCMaster.OwnerId = lngBrokerID;

                        //objCMaster.Status = status;
                        int result = objClientMngmt.EditClient(objCMaster, empID);
                        CompanyMaster objClientMst = new CompanyMaster();
                        objClientMst = objClientMngmt.GetClientDetils(objCMaster.ID);


                        var name = objCMaster.CName;
                        var email = objCMaster.CEmail;
                        var phone = objCMaster.CContact;
                        objCMaster.CName = null;
                        if (result == 1)
                        {
                            //if(email!= oldDetails.Email)
                            //{
                            //    tblCompany_Employee_BasicInfo objComEmpBasicInfo = new tblCompany_Employee_BasicInfo();
                            //    tblCompany_Employee_PersonalInfo objComEmpPerInfo = new tblCompany_Employee_PersonalInfo();
                            //    objComEmpBasicInfo.FirstName = name;
                            //    objComEmpBasicInfo.Email = email;
                            //    objComEmpPerInfo.Phone = phone;
                            //    objComEmpBasicInfo.Password = objCMaster.CPassword;
                            //    objComEmpBasicInfo.RoleID = 1;
                            //    objComEmpBasicInfo.CompanyId = objCMaster.ID;
                            //    tblCompany_Employee_EmploymentInfo objComEmpEmpInfo = new tblCompany_Employee_EmploymentInfo();
                            //    new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement().AddEmployeeBulkDetails(objComEmpBasicInfo, objComEmpPerInfo, objComEmpEmpInfo);
                            //}


                            var mulAdmin = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement().GetMutipleCompanyAdmin(objCMaster.ID);
                            foreach (var item in mulAdmin)// in case of active mail will be send on multiple company admin.
                            {
                                if (string.IsNullOrEmpty(item.Password) && objCMaster.IsActive.Equals(true))
                                {
                                    string baseurl = Helper.GetBaseUrl();
                                    string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/Add_New_Company.html"));  // email template
                                    Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                                    Mailbody = Mailbody.Replace("###LoginUserName###", item.Email);
                                    Mailbody = Mailbody.Replace("###UserName###", item.FirstName);
                                    Mailbody = Mailbody.Replace("###Password###", "");//Helper.Decrypt(item.Password)
                                    Mailbody = Mailbody.Replace("###UserNAME###", Helper.Encrypt(item.Email));
                                    Mailbody = Mailbody.Replace("###CompanyName###", objCMaster.CCompanyName);
                                    if (!string.IsNullOrEmpty(item.Email))
                                    {
                                        // Date21Sept as per client Discussion code comment 
                                        Helper.SendEmail(item.Email, Mailbody, "Welcome to EnrollMyGroup Administration");
                                    }

                                }
                            }


                            TempData["MessageType"] = "Success";
                            TempData["CustomMessage"] = "Company updated successfully.";
                            //Helper.EmailConfirmation(objCMaster.CEmail, 1, randomPassword);

                        }
                        else if (result == -2)
                        {
                            TempData["MessageType"] = "Error";
                            TempData["CustomMessage"] = "Phone no already exists.";
                        }
                        else if (result == -3)
                        {
                            TempData["MessageType"] = "Error";
                            TempData["CustomMessage"] = "Email already exists.";
                        }
                        else if (result == -4)
                        {
                            TempData["MessageType"] = "Error";
                            TempData["CustomMessage"] = "Email and phone no does not match.";
                        }
                        else if (result == 2)
                        {
                            TempData["MessageType"] = "Error";
                            TempData["CustomMessage"] = "This email address or phone no is already being used, please try again.";
                        }
                        else
                        {
                            TempData["MessageType"] = "Error";
                            TempData["CustomMessage"] = "Some error while processing your request.";

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("AddClient", ex.Message);

            }
            return View(objClientValid);
            // return RedirectToAction("ManageClients", "Admin");

        }
        [sessionexpireattribute]
        public ActionResult AccessCompany(long AdminCompanyid, long EmpId)
        {

            Session["CompanyID"] = AdminCompanyid;
            Session["ComEmployeeId"] = EmpId;
            Session["EmgAdminID"] = 1;
            if (Session["CompanyID"] != null)
                return RedirectToAction("Index", "DashBoard", new { area = "Company" });

            return null;
        }
        public void ExportClientsListToCSV()
        {


            string header = @"""Employee ID"",""Company Name"",""Contact Name"",""Contact Title"",""Employee Address"",""Postal Code""";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header);
            UserManagement um = new UserManagement();
            var contactUsEnquiries = um.GetEnquiries();
            foreach (var i in contactUsEnquiries)
            {

                sb.AppendLine(string.Join(",",
                             string.Format(@"""{0}""", i.Name),
                             string.Format(@"""{0}""", i.Email),
                             string.Format(@"""{0}""", i.Message)));
            }
            HttpContext context = System.Web.HttpContext.Current;
            context.Response.Write(sb.ToString());
            context.Response.ContentType = "text/csv";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=EmployeeData.csv");
            context.Response.End();
        }

        [sessionexpireattribute]
        [HttpGet]
        public ActionResult PlanCategory()
        {
            var CompanyDeptData = cmm.GetPlanCategory(0);
            ViewBag.PlanCategory = CompanyDeptData;
            return View();
        }

        [sessionexpireattribute]
        public ActionResult LifeEvents()
        {
            var lifeeventData = cmm.GetLifeEvents();
            ViewBag.LifeEvents = lifeeventData;
            return View();
        }
        [sessionexpireattribute]
        public ActionResult PlanDeduction()
        {

            var plandeductionData = cmm.GetPlanDeduction(0);
            ViewBag.PlanDeduction = plandeductionData;
            return View();
        }

        [sessionexpireattribute]
        [HttpPost]
        public ActionResult PlanCategory(EMG_PlanCategory plancat)
        {
            EMG_PlanCategory obj = new EMG_PlanCategory();
            long result = 0;

            result = cmm.AddPlanCategory(plancat);
            if (result == 1)
            {

                TempData["MessageTypePC"] = "Success";
                TempData["CustomMessagePC"] = "Plan category saved successfully.";

            }
            else
            {
                TempData["MessageTypePC"] = "Error";
                TempData["CustomMessagePC"] = "Plan categroy already saved.";

            }


            return RedirectToAction("PlanCategory", "Admin");
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditPlanCategory(int ID)
        {
            EMG_PlanCategory details = null;
            try
            {
                details = cmm.EditPlanCategory(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var CompanyDeptData = cmm.GetPlanCategory(0);
            ViewBag.PlanCategory = CompanyDeptData;
            //  return RedirectToAction("PlanCategory", "Admin",details);
            return View("PlanCategory", details);
        }


        [sessionexpireattribute]
        [HttpPost]
        public ActionResult LifeEvents(EMG_LifeEvents obj)
        {
            EMG_LifeEvents objdetails = new EMG_LifeEvents();
            long result = 0;
            var lifeventid = 0;
            lifeventid = obj.ID;

            result = cmm.AddLifeEvents(obj);
            if (result == 1)
            {
                if (lifeventid == 0)
                {
                    TempData["MessageTypeALE"] = "Success";
                    TempData["CustomMessageALE"] = "Life event saved successfully.";
                }
                else
                {
                    TempData["MessageTypeALE"] = "Success";
                    TempData["CustomMessageALE"] = "Life event update successfully.";
                }


            }
            else if (result == 2)
            {

                TempData["MessageTypeALE"] = "Error";
                TempData["CustomMessageALE"] = "Life event already exists. Please try another name.";

            }
            else
            {
                TempData["MessageTypeALE"] = "Error";
                TempData["CustomMessageALE"] = "Some error occured.";

            }

            return RedirectToAction("LifeEvents", "Admin");
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditLifeEvents(int ID)
        {
            EMG_LifeEvents details = null;
            try
            {
                details = cmm.EditLifeEvents(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var data = cmm.GetLifeEvents();
            ViewBag.LifeEvents = data;
            //  return RedirectToAction("PlanCategory", "Admin",details);
            return View("LifeEvents", details);
        }


        [sessionexpireattribute]
        [HttpPost]
        public ActionResult PlanDeduction(EMG_PlanDeduction obj)
        {
            EMG_PlanDeduction objdetails = new EMG_PlanDeduction();
            long result = 0;

            result = cmm.AddPlanDeduction(obj);
            if (result == 1)
            {

                TempData["MessageTypePD"] = "Success";
                TempData["CustomMessagePD"] = "Plan deduction saved successfully.";

            }
            else
            {
                TempData["MessageTypePD"] = "Error";
                TempData["CustomMessagePD"] = "Some error occured.";

            }


            return RedirectToAction("PlanDeduction", "Admin");
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditPlanDeduction(int ID)
        {
            EMG_PlanDeduction details = null;
            try
            {
                details = cmm.EditPlanDeduction(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            var data = cmm.GetPlanDeduction(0);
            ViewBag.PlanDeduction = data;

            return View("PlanDeduction", details);
        }

        [sessionexpireattribute]
        [HttpGet]
        public ActionResult ManageCompanyEmployees()
        {
            long admID = Convert.ToInt64(Session["AdminID"].ToString());
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var cdata = objComEmpMgnt.GetCompanyDropdown();//objClientMngmt.GetAllBrokerClients("", admID);
            SelectList data = new SelectList(cdata, "ID", "CCompanyName");
            ViewBag.Company = data;
            return View();
        }

        [HttpPost]
        [sessionexpireattribute]
        public ActionResult ManageCompanyEmployees(HttpPostedFileBase file)
        {
            Comman objComman = new Comman();
            CompanyManagement objClientMngmt = new CompanyManagement();
            string finalflag = string.Empty;
            string strCity = string.Empty;
            long lngCountryID, lngStateID, lngCityID = 0;
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            StringBuilder msgError = new StringBuilder();
            long CompanyID = Convert.ToInt64(Request.Form["CompanyID"]);
            StringBuilder msgSuccess = new StringBuilder();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data1 = new SelectList(cdata, "ID", "CCompanyName");
            ViewBag.Company = data1;
            string EmailNHandOE = string.Empty;
            EmailNHandOE = Convert.ToString(Request.Form["EnrollmentEmailNHandOE"]);
            string EmpSSN = string.Empty;
            string DependentSSN = string.Empty;
            if (file == null)
            {
                Response.Write("<script>alert('Please select File.');</script>");
            }
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



                int counterrow = 1;
                bool flag = true;

                long result = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    var objComEmpBasicInfo = new tblCompany_Employee_BasicInfo();
                    var objComEmpEmpInfo = new tblCompany_Employee_EmploymentInfo();
                    var objComEmpPerInfo = new tblCompany_Employee_PersonalInfo();
                    var dependentInfo = new tblCompany_Employee_DependentInfo();

                    objComEmpBasicInfo.CompanyId = CompanyID;
                    EmpSSN = objComman.RemoveMasking(Convert.ToString(dr["SSN"]).Trim());
                    DependentSSN = objComman.RemoveMasking(Convert.ToString(dr["Dependent_SSN"]).Trim());
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

                    if (EmpSSN == DependentSSN)
                    {
                        objComEmpEmpInfo.SSN = EmpSSN;
                        objComEmpBasicInfo.EmpCode = dr["EmployeeId"].ToString();
                        objComEmpBasicInfo.FirstName = dr["First_Name"].ToString();
                        objComEmpBasicInfo.MiddleName = dr["Middle_Name"].ToString();
                        objComEmpBasicInfo.LastName = dr["Last_Name"].ToString();
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Suffix"])))
                        {
                            objComEmpBasicInfo.NameTitle = dr["Suffix"].ToString();
                        }
                        else
                        {
                            objComEmpBasicInfo.NameTitle = "0";
                        }
                        objComEmpBasicInfo.Email = dr["Email"].ToString();
                        objComEmpBasicInfo.Title = dr["Title"].ToString();
                        objComEmpPerInfo.Phone = dr["Mobile_Phone"].ToString();
                        objComEmpPerInfo.WorkPhone = dr["Work_Phone"].ToString();
                        objComEmpPerInfo.EmergancyContactName = dr["Emergency_Contact_Name"].ToString();
                        objComEmpPerInfo.RelationWithContact = dr["Relationship_with_Contact"].ToString();
                        objComEmpPerInfo.EmergancyPhone = dr["Emergency_Phone"].ToString();

                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Work_State"])))
                        {
                            var empStateCode = objClientMngmt.GetEmployeeStateCode(Convert.ToString(dr["Work_State"]).Trim());
                            if (empStateCode != null)
                            {
                                objComEmpBasicInfo.WorkLocationID = Convert.ToInt32(empStateCode.Id); //Convert.ToInt32(dr["Work_State"]);
                            }
                            else
                            {
                                objComEmpBasicInfo.WorkLocationID = null;
                            }
                        }
                        else
                        {
                            objComEmpBasicInfo.WorkLocationID = null;
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Hire_Date"])))
                        {
                            objComEmpEmpInfo.StartDate = Convert.ToDateTime(dr["Hire_Date"]);
                        }
                        else
                        {
                            objComEmpEmpInfo.StartDate = null;
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["DOB"])))
                        {
                            objComEmpPerInfo.DOB = Convert.ToDateTime(dr["DOB"]);
                        }
                        else
                        {
                            objComEmpPerInfo.DOB = null;
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Employee_Type"])))
                        {

                            objComEmpEmpInfo.EmployeeType = (Convert.ToString(dr["Employee_Type"]).ToUpper().Trim() == "FT" ? 1 : Convert.ToString(dr["Employee_Type"]).ToUpper().Trim() == "PT" ? 2 : Convert.ToString(dr["Employee_Type"]).ToUpper().Trim() == "FT-E" ? 3 : 0);//Convert.ToInt32(dr["Employee_Type"]);
                        }
                        else
                        {
                            objComEmpEmpInfo.EmployeeType = null;
                        }


                        var department = Convert.ToString(dr["Department"]);
                        if (!string.IsNullOrEmpty(department))
                        {
                            objComEmpBasicInfo.DepartmentID = objComEmpMgnt.GetDepartmentIDByNameandCompanyID(department, Convert.ToInt32(objComEmpBasicInfo.CompanyId));
                            if (objComEmpBasicInfo.DepartmentID == 0)
                            {
                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Department not exists</b> at Row No. - " + counterrow + " </li>");
                                flag = false;
                            }

                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Division"])))
                        {
                            var divisionDet = objComEmpMgnt.GetDivisions(Convert.ToString(dr["Division"]), CompanyID);
                            if (divisionDet == null)
                            {
                                msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Division not exists</b> at Row No. - " + counterrow + "</li>");
                                flag = false;
                            }
                            else
                            {
                                objComEmpEmpInfo.DivisionID = divisionDet.ID;
                            }

                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Payroll_Frequency"])))
                        {
                            objComEmpEmpInfo.MonthlyBasisID = (Convert.ToString(dr["Payroll_Frequency"]).ToUpper().Trim() == "WEEKLY" ? 1 : Convert.ToString(dr["Payroll_Frequency"]).ToUpper().Trim() == "BIWEEKLY" ? 2 : Convert.ToString(dr["Payroll_Frequency"]).ToUpper().Trim() == "BIMONTHLY" ? 3 : Convert.ToString(dr["Payroll_Frequency"]).ToUpper().Trim() == "MONTHLY" ? 4 : 0);//Convert.ToInt32(dr["Payroll_Frequency"]);
                        }
                        else
                        {
                            objComEmpEmpInfo.MonthlyBasisID = null;
                        }


                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Annual_Salary"])))
                        {
                            objComEmpEmpInfo.Salary = Convert.ToInt32(dr["Annual_Salary"]);
                        }
                        else
                        {
                            objComEmpEmpInfo.Salary = null;
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Bonus"])))
                        {
                            objComEmpEmpInfo.Bonus = Convert.ToInt32(dr["Bonus"]);


                        }
                        else
                        {
                            objComEmpEmpInfo.Bonus = 0;
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Commission"])))
                        {
                            objComEmpEmpInfo.Commisiion = Convert.ToInt32(dr["Commission"]);

                        }
                        else
                        {
                            objComEmpEmpInfo.Commisiion = 0;
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Gender"]).Trim()))
                        {
                            var Gender = Convert.ToString(dr["Gender"]).Trim().ToLower();

                            if (Gender == "m" || Gender == "male")
                            {
                                objComEmpPerInfo.Gender = "1";
                            }
                            if (Gender == "f" || Gender == "female")
                            {
                                objComEmpPerInfo.Gender = "2";
                            }

                        }
                        //objComEmpPerInfo.Gender = dr["Gender"].ToString();
                        objComEmpBasicInfo.IsActive = true;
                        objComEmpBasicInfo.Password = null;
                        objComEmpPerInfo.Address = dr["Address"].ToString();
                        objComEmpPerInfo.Zip = dr["ZipCode"].ToString();
                        lngCountryID = objComman.GetCountry(dr["Country"].ToString());
                        //  lngStateID = objComman.GetState(dr["State"].ToString(), lngCountryID);
                        lngCityID = objComman.GetCity(dr["City"].ToString(), 0, lngCountryID);
                        objComEmpPerInfo.City = lngCityID;
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["State"])))
                        {
                            var empStateCode = objClientMngmt.GetEmployeeStateCode(Convert.ToString(dr["State"]).Trim());
                            if (empStateCode != null)
                            {
                                objComEmpPerInfo.State = Convert.ToInt32(empStateCode.Id);
                            }
                            else
                            {
                                objComEmpPerInfo.State = null;
                            }
                        }
                        else
                        {
                            objComEmpPerInfo.State = null;
                        }
                        // objComEmpPerInfo.State = lngStateID;
                        objComEmpPerInfo.Country = lngCountryID;
                        objComEmpBasicInfo.RoleID = 2; // Role id 2 is for employee
                        // check the validation proccess in Excel


                    }
                    else
                    {
                        // check  validation for depedent

                        dependentInfo.Dependent = dr["Relation"].ToString().ToLower();

                        if (dependentInfo.Dependent == "sp" || dependentInfo.Dependent == "spouse")
                        {
                            dependentInfo.Dependent = "Spouse";
                            dependentInfo.IsDomesticSpouse = false;
                            dependentInfo.SpouseIsEmployed = false;
                            dependentInfo.SpouseHasCoverage = false;
                        }
                        if (dependentInfo.Dependent == "ch" || dependentInfo.Dependent == "child")
                        {
                            dependentInfo.Dependent = "Child";
                        }
                        if (string.IsNullOrEmpty(dependentInfo.Dependent) || dependentInfo.Dependent == " ")
                        {
                            dependentInfo.Dependent = "Other";
                        }
                        if (dependentInfo.Dependent == "dp" || dependentInfo.Dependent == "domestic partner")
                        {
                            dependentInfo.Dependent = "Spouse";
                            dependentInfo.IsDomesticSpouse = true;
                            dependentInfo.SpouseIsEmployed = false;//dependentlist.SpouseIsEmployed;
                            dependentInfo.SpouseHasCoverage = false;//dependentlist.SpouseHasCoverage;
                        }
                        dependentInfo.SSN = DependentSSN;
                        dependentInfo.FirstName = dr["First_Name"].ToString();
                        dependentInfo.MiddleName = dr["Middle_Name"].ToString();
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Suffix"])))
                        {
                            dependentInfo.NameTitle = dr["Suffix"].ToString();
                        }
                        else
                        {
                            dependentInfo.NameTitle = "0";
                        }

                        //dependentInfo.NameTitle = dr["Suffix"].ToString();
                        dependentInfo.LastName = dr["Last_Name"].ToString();
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["DOB"])))
                        {
                            dependentInfo.DOB = Convert.ToDateTime(dr["DOB"]);
                        }
                        else
                        {
                            dependentInfo.DOB = null;
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(dr["Gender"]).Trim()))
                        {
                            var Gender = Convert.ToString(dr["Gender"]).Trim().ToLower();

                            if (Gender == "m" || Gender == "male")
                            {
                                dependentInfo.Gender = "1";
                            }
                            if (Gender == "f" || Gender == "female")
                            {
                                dependentInfo.Gender = "2";
                            }

                        }
                        //dependentInfo.Gender = dr["Gender"].ToString();
                        dependentInfo.IsStudent = false;
                        dependentInfo.IsSmoker = false;
                        dependentInfo.IsActive = true;
                        dependentInfo.IsDisable = false;


                    }


                    if (flag)
                    {

                        //if ((!string.IsNullOrEmpty(dr["SSN"].ToString())) == (!string.IsNullOrEmpty(dr["Dependent_SSN"].ToString())))
                        if (EmpSSN == DependentSSN)
                        {
                            result = objComEmpMgnt.AddEmployeeBulkDetails(objComEmpBasicInfo, objComEmpPerInfo, objComEmpEmpInfo, false);
                            var companyDetails = objClientMngmt.GetCompanyName(CompanyID);
                            var data = objComEmpMgnt.GetOpenEmrollmentDate(Convert.ToInt64(objComEmpEmpInfo.MonthlyBasisID), Convert.ToInt64(objComEmpEmpInfo.DivisionID));

                            if (result > 0)
                            {
                                finalflag = "Success";
                                string baseurl = Helper.GetBaseUrl();
                                if (EmailNHandOE == "1") // For Open Enrollment Email
                                {
                                    //string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/RegistrationConfirmation.html"));  // email template
                                    string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/RegistrationConfirmation_NewEmpAdd.html"));  // email template temp
                                    Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                                    Mailbody = Mailbody.Replace("###LoginUserName###", objComEmpBasicInfo.Email);
                                    Mailbody = Mailbody.Replace("###FirstName###", objComEmpBasicInfo.FirstName);
                                    if (!string.IsNullOrEmpty(objComEmpBasicInfo.Email))
                                    {
                                        Mailbody = Mailbody.Replace("###UserNAME###", Helper.Encrypt(objComEmpBasicInfo.Email));
                                    }

                                    Mailbody = Mailbody.Replace("###CompanyNAME###", companyDetails.CCompanyName);
                                    if (data != null)
                                    {
                                        Mailbody = Mailbody.Replace("###EndDate###", string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(data.EndDate)));
                                    }
                                    else
                                    {
                                        Mailbody = Mailbody.Replace("###EndDate###", "N/A");
                                    }

                                    if (!string.IsNullOrEmpty(objComEmpBasicInfo.Email))
                                    {
                                        Helper.SendEmail(objComEmpBasicInfo.Email, Mailbody, "Welcome to the " + companyDetails.CCompanyName + " Open Enrollment");
                                    }
                                }
                                if (EmailNHandOE == "2")// email for New Hire
                                {
                                    string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/RegistrationConfirmation_NewEmpAdd.html"));  // email template
                                    Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                                    Mailbody = Mailbody.Replace("###FirstName###", objComEmpBasicInfo.FirstName);
                                    if (!string.IsNullOrEmpty(objComEmpBasicInfo.Email))
                                    {
                                        Mailbody = Mailbody.Replace("###UserNAME###", Helper.Encrypt(objComEmpBasicInfo.Email));
                                    }
                                    Mailbody = Mailbody.Replace("###CompanyNAME###", companyDetails.CCompanyName);
                                    if (data != null)
                                    {
                                        Mailbody = Mailbody.Replace("###EndDate###", string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(data.EndDate)));
                                    }
                                    else
                                    {
                                        Mailbody = Mailbody.Replace("###EndDate###", "N/A");
                                    }

                                    if (!string.IsNullOrEmpty(objComEmpBasicInfo.Email))
                                    {
                                        Helper.SendEmail(objComEmpBasicInfo.Email, Mailbody, "Welcome to the " + companyDetails.CCompanyName + " Open Enrollment");
                                    }
                                }

                                if (EmailNHandOE == "3" && CompanyID == 11)// CompanyId 11 for jetro for live server  
                                {
                                    // this email only for live server and jetro company only not other use
                                    string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/RegistrationConfirmation_NewEmpAdd_GainEligibility.html"));  // email template
                                    Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                                    Mailbody = Mailbody.Replace("###FirstName###", objComEmpBasicInfo.FirstName);
                                    if (!string.IsNullOrEmpty(objComEmpBasicInfo.Email))
                                    {
                                        Mailbody = Mailbody.Replace("###UserNAME###", Helper.Encrypt(objComEmpBasicInfo.Email));
                                    }
                                    Mailbody = Mailbody.Replace("###CompanyNAME###", companyDetails.CCompanyName);
                                    if (data != null)
                                    {
                                        Mailbody = Mailbody.Replace("###EndDate###", string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(data.EndDate)));
                                    }
                                    else
                                    {
                                        Mailbody = Mailbody.Replace("###EndDate###", "N/A");
                                    }

                                    if (!string.IsNullOrEmpty(objComEmpBasicInfo.Email))
                                    {
                                        Helper.SendEmail(objComEmpBasicInfo.Email, Mailbody, "You’re Eligible for " + companyDetails.CCompanyName + " Benefits");
                                    }
                                }
                                int statusPhone = 0;
                                if (!string.IsNullOrEmpty(objComEmpPerInfo.Phone))
                                {
                                    if (EmailNHandOE == "1" || EmailNHandOE == "2") // For Open Enrollment and New Hire Message 
                                    {
                                        //if (data != null)
                                        //{
                                        //    statusPhone = SendTwilloMessage.SendmessageTwillo(objComEmpPerInfo.Phone, "Good news from " + companyDetails.CCompanyName + "! It's time to enroll in great benefits. Enrollment will remain open until " + data.EndDate + ". Click the link below to download the EnrollMyGroup mobile app and get started.");
                                        //}
                                        //else
                                        //{
                                        //    statusPhone = SendTwilloMessage.SendmessageTwillo(objComEmpPerInfo.Phone, "Good news from " + companyDetails.CCompanyName + "! It's time to enroll in great benefits. Enrollment will remain open until N/A. Click the link below to download the EnrollMyGroup mobile app and get started.");
                                        //}

                                        string URLReset = baseurl + "/Home/PasswordUserReset?email=" + Helper.Encrypt(objComEmpBasicInfo.Email);
                                        statusPhone = SendTwilloMessage.SendmessageTwillo(objComEmpPerInfo.Phone, "Welcome aboard " + companyDetails.CCompanyName + ", \n \n Enroll in our great benefits as soon as possible so you don't miss coverage. \n \n Click this link to set up a password and begin. \n \n" + URLReset);

                                    }

                                }

                            }
                            else
                            {
                                if (result == -1)
                                {
                                    finalflag = "-1";
                                }

                            }

                        }
                        else
                        {

                            var duplicateComRecord = new CompanyManagement().ChechDuplicateEmailandPhone("", "", 0, 4, dependentInfo.SSN);

                            if (duplicateComRecord.Count == 0)
                            {
                                if ((result > 0) && duplicateComRecord.Count == 0)
                                {
                                    string EMPssncheckcond = objComEmpMgnt.GetEmployementInfoGetSSN(result);

                                    string chSSN = EmpSSN;

                                    if (chSSN == EMPssncheckcond)
                                    {
                                        PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt1 = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
                                        dependentInfo.EmployeeId = result;//(result > 0 ? result : empSSN.EmployeeID);
                                        dependentInfo.CompanyId = CompanyID;
                                        //int result1 = objComEmpMgnt1.AddDependents(dependentInfo);
                                        var spousecheck = new CompanyManagement().GetDependentInfobyEmpId(Convert.ToInt64(dependentInfo.EmployeeId));
                                        if (spousecheck == null)
                                        {
                                            int result1 = objComEmpMgnt1.AddDependents(dependentInfo);
                                        }
                                        else
                                        {

                                            if (spousecheck != null)
                                            {
                                                if ((dr["Relation"].ToString().ToLower().Contains("ch") || dr["Relation"].ToString().ToLower().Contains("child")))
                                                {
                                                    int result1 = objComEmpMgnt1.AddDependents(dependentInfo);
                                                }
                                            }

                                        }
                                    }

                                }
                                else
                                {
                                    if (result == -1)
                                    {
                                        finalflag = "-1";
                                    }
                                    if (result == 0)
                                    {
                                        finalflag = "0";
                                    }

                                }

                            }
                            else
                            {
                                finalflag = "-1";
                            }

                        }

                    }
                    counterrow++;
                }


                if (finalflag == "Success")
                {
                    TempData["SuccessMessage"] = "File uploaded successfully.";
                    TempData["MessageType"] = "Success";

                }

                if (finalflag == "-1")
                {
                    TempData["SuccessMessage"] = "Duplicate values occured in SSN, Dependent SSN, EMPCode,Email or MobilePhone. Please check census file.";
                    TempData["MessageType"] = "Error";
                }


                if (finalflag == "0")
                {
                    TempData["SuccessMessage"] = "Nothing to be added. Please check census file.";
                    TempData["MessageType"] = "Error";
                }


            }
            return View();
            // return RedirectToAction("UploadCensus", "Employee");
        }

        [HttpPost]
        [sessionexpireattribute]
        public ActionResult UploadCensusValidate(string SCompanyID)
        {
            Comman objComman = new Comman();
            CompanyManagement objClientMngmt = new CompanyManagement();
            string finalflag = string.Empty;
            string strCity = string.Empty;
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            StringBuilder msgError = new StringBuilder();
            //long CompanyID = Convert.ToInt64(Request.Form["CompanyID"]);
            long CompanyID = Convert.ToInt64(SCompanyID);
            StringBuilder msgSuccess = new StringBuilder();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data1 = new SelectList(cdata, "ID", "CCompanyName");
            ViewBag.Company = data1;
            string EmpSSN = string.Empty;
            string DependentSSN = string.Empty;
            string MobilePhone = string.Empty;
            string WorkPhone = string.Empty;
            string EmergancyPhone = string.Empty;

            bool flag = true;
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
                            // if ssn and Dependent_SSN are equal that is employee
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

                                //if (!string.IsNullOrEmpty(Convert.ToString(dr["DOB"])))
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
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Division not exists</b> at Row No. - " + counterrow + "</li>");
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
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Department not exists</b> at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }

                                if (string.IsNullOrEmpty(Convert.ToString(dr["Hire_Date"] != DBNull.Value ? dr["Hire_Date"] : "")))
                                {
                                    objComEmpEmpInfo.StartDate = null;
                                }

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["Hire_Date"] != DBNull.Value ? dr["Hire_Date"] : "")))
                                {
                                    // Date Regex

                                    //bool checkDatefromat = ValidateDate(Convert.ToString(dr["Hire_Date"]));
                                    //if (!checkDatefromat)
                                    //{
                                    //    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Hire Date</b> format must be MM/DD/YYYY at Row No. - " + counterrow + "</li>");
                                    //    flag = false;
                                    //}
                                    DateTime a;
                                    if (DateTime.TryParse(Convert.ToString(dr["Hire_Date"]).Trim(), out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Hire Date </b> format must be MM/DD/YYYY Row No. - " + counterrow + "</li>");
                                        flag = false;
                                    }

                                }


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
                                    // decimal.TryParse(Salary, out a);
                                    //if (a == 0)
                                    if (decimal.TryParse(Salary, out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Salary </b> Must be numeric at Row No. - " + counterrow + " </li>");
                                        flag = false;
                                    }
                                }
                                if (!string.IsNullOrEmpty(Salary) && Convert.ToDecimal(Salary) < 1)
                                {
                                    msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>Salary </b> Must be greater than 0 at Row No. - " + counterrow + " </li>");
                                    flag = false;
                                }

                                string Bonus = Convert.ToString(dr["Bonus"]);

                                //if (string.IsNullOrEmpty(Convert.ToString(dr["Bonus"])))
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
                                    // decimal.TryParse(Bonus, out a);
                                    //if (a == 0)
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
                                    // decimal.TryParse(Commission, out a);
                                    //if (a == 0)
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
                                    // long.TryParse(Convert.ToString(dr["Emergancy_Phone"]), out a);
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
                                    DateTime a;
                                    if (DateTime.TryParse(Convert.ToString(dr["DOB"]).Trim(), out a) == false)
                                    {
                                        msgError.Append("<li><i class=\"fa fa-times-circle\"></i>&nbsp;&nbsp;&nbsp;<b style='text-decoration:underline'>DOB </b> Must be Date format at Row No. - " + counterrow + "</li>");
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

        public static bool ValidateDate(string date)
        {
            string strRegex = @"^(((0?[1-9]|1[012])/(0?[1-9]|1\d|2[0-8])|(0?[13456789]|1[012])/(29|30)|(0?[13578]|1[02])/31)/(19|[2-9]\d)\d{2}|0?2/29/((19|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(([2468][048]|[3579][26])00)))$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(date))
                return (true);
            else
                return (false);
        }

        [sessionexpireattribute]
        [HttpGet]
        public ActionResult Code14()
        {
            var data = cmm.GetCode14(0);
            ViewBag.Code14 = data;
            return View();
        }

        [sessionexpireattribute]
        [HttpPost]
        public ActionResult Code14(EMG_Code14 info)
        {
            EMG_Code14 obj = new EMG_Code14();
            long result = 0;

            result = cmm.AddCode14(info);
            if (result == 1)
            {
                TempData["MessageTypeC14"] = "Success";
                TempData["CustomMessageC14"] = "Code 14 saved successfully.";

            }
            else
            {
                TempData["MessageTypeC14"] = "Error";
                TempData["CustomMessageC14"] = "Already code 14 saved.";

            }


            return RedirectToAction("Code14", "Admin");
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditCode14(int ID)
        {
            EMG_Code14 details = null;
            try
            {
                details = cmm.EditCode14(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var data = cmm.GetCode14(0);
            ViewBag.Code14 = data;
            //  return RedirectToAction("PlanCategory", "Admin",details);
            return View("Code14", details);
        }

        [sessionexpireattribute]
        [HttpGet]
        public ActionResult SH16()
        {
            var data = cmm.GetSH16(0);
            ViewBag.SH16 = data;
            return View();
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult SH16(EMG_SH16 info)
        {
            EMG_SH16 obj = new EMG_SH16();
            long result = 0;

            result = cmm.AddSH16(info);
            if (result == 1)
            {

                TempData["MessageTypeSH16"] = "Success";
                TempData["CustomMessageSH16"] = "SH 16 saved successfully.";

            }
            else
            {
                TempData["MessageTypeSH16"] = "Error";
                TempData["CustomMessageSH16"] = "Already SH 16 saved.";

            }


            return RedirectToAction("SH16", "Admin");
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditSH16(int ID)
        {
            EMG_SH16 details = null;
            try
            {
                details = cmm.EditSH16(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
            }
            var data = cmm.GetSH16(0);
            ViewBag.SH16 = data;

            return View("SH16", details);
        }

        [sessionexpireattribute]
        public ActionResult DeleteLifeEvents(string EnrollId)
        {
            long ID = Convert.ToInt64(EnrollId);
            int result = cmm.DeleteLifeEvents(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult DeleteEMGProduct(string EnrollId)
        {
            long ID = Convert.ToInt64(EnrollId);
            int result = cmm.DeleteEMGProduct(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult DeletePlanDeduction(string EnrollId)
        {
            long ID = Convert.ToInt64(EnrollId);
            int result = cmm.DeletePlanDeduction(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult DeleteCode14(string EnrollId)
        {
            long ID = Convert.ToInt64(EnrollId);
            int result = cmm.DeleteCode14(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult DeleteSH16(string EnrollId)
        {
            long ID = Convert.ToInt64(EnrollId);
            int result = cmm.DeleteSH16(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult DeleteBPlanCategory(string EnrollId)
        {
            long ID = Convert.ToInt64(EnrollId);
            int result = cmm.DeleteBPlanCategory(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult PlanDeductions(string value)
        {

            var plandeductionData = cmm.GetPlanDeductions(value);
            ViewBag.PlanDeduction = plandeductionData;
            return PartialView("_PlanDeduction");
        }
        [sessionexpireattribute]
        public ActionResult GetDeletedPlanCategory(string value)
        {
            var CompanyDeptData = cmm.GetDeletedPlanCategory(value);
            ViewBag.PlanCategory = CompanyDeptData;
            return PartialView("_PlanCategory");
        }
        [sessionexpireattribute]
        public ActionResult GetDeletedLifeEvents(string value)
        {
            var lifeeventData = cmm.GetDeletedLifeEvents(value);
            ViewBag.LifeEvents = lifeeventData;
            return PartialView("_LifeEvents");
        }
        [sessionexpireattribute]
        public ActionResult GetDeletedEMGProducts(string value)
        {
            var lifeeventData = cmm.GetDeletedEMGProducts(value);
            ViewBag.EMGProducts = lifeeventData;
            return PartialView("_EMGProducts");
        }
        [sessionexpireattribute]
        public ActionResult GetDeletedCode14(string value)
        {
            var data = cmm.GetDeletedCode14(value);
            ViewBag.Code14 = data;
            return PartialView("_Code14");
        }
        [sessionexpireattribute]
        public ActionResult GetDeletedSH16(string value)
        {
            var data = cmm.GetDeletedSH16(value);
            ViewBag.SH16 = data;
            return PartialView("_SH16");
        }
        [sessionexpireattribute]
        public ActionResult UpdateCompanyDemoStatus(int ID, int flag)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            UserManagement um = new UserManagement();
            int i = um.UpdateCompanyDemoStatus(ID, flag);
            if (i == 0)
            {
                TempData["MessageTypeDS"] = "Success";
                TempData["CustomMessageDS"] = "Demo given successfully.";

            }
            else
            {
                TempData["MessageTypeDS"] = "Success";
                TempData["CustomMessageDS"] = "Company registered successfully.";
                CompanyMaster objCMaster = new CompanyMaster();
                objCMaster = cmm.GetCompanyDetails(ID);
                string body_CreateCompany = string.Empty;
                string baseurl = Helper.GetBaseUrl();
                string CreateCompany_email_subject = string.Empty;
                CreateCompany_email_subject = "Email Confirmation";

                body_CreateCompany = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/Add_New_Company.html"));

                body_CreateCompany = body_CreateCompany.Replace("###HostUrl####", baseurl);
                body_CreateCompany = body_CreateCompany.Replace("###LoginUserName###", objCMaster.CEmail);
                body_CreateCompany = body_CreateCompany.Replace("###UserName###", objCMaster.CCompanyName);
                body_CreateCompany = body_CreateCompany.Replace("###Password###", Helper.Decrypt(objCMaster.CPassword));
                body_CreateCompany = body_CreateCompany.Replace("###UserNAME###", Helper.Encrypt(objCMaster.CEmail));
                body_CreateCompany = body_CreateCompany.Replace("###CompanyNAME###", objCMaster.CCompanyName);
                if (!string.IsNullOrEmpty(objCMaster.CEmail))
                {
                    // Date21Sept as per client Discussion code comment 
                    Helper.SendEmail(objCMaster.CEmail, body_CreateCompany, CreateCompany_email_subject);
                }




            }

            return RedirectToAction("ManageCompanyDemos");
        }
        [sessionexpireattribute]
        public ActionResult ManageOpenEnrollment(int CID)
        {
            var CompanyDeptData = objClientMngmt.GetOpenEnrolment(CID);
            ViewBag.OpenEnrollment = CompanyDeptData;
            ViewBag.CompanyId = CID;
            return View();
        }
        [sessionexpireattribute]
        public ActionResult ManageOpenEnrollment1(int CID)
        {
            var CompanyDeptData = objClientMngmt.GetOpenEnrolment1(CID);
            var details = CompanyDeptData;
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult ManageLifeEvents(int CID)
        {
            var CompanyDeptData = objClientMngmt.GetLifeChangeEventsData(CID);
            ViewBag.LifeEvents = CompanyDeptData;
            ViewBag.CompanyId = CID;
            return View();
        }
        [sessionexpireattribute]
        public ActionResult ViewOpenEnrollment(int EmpID)
        {
            var compnyID = objClientMngmt.GetCompanyID(EmpID);
            var OpenEnrollData = objClientMngmt.GetOpenEnrollMent(EmpID, compnyID);
            ViewBag.OpenEnrollmentStart = OpenEnrollData;
            return View();
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult SystemConfigurationkey()
        {
            var data = cmm.GetSystemConfiguration();
            ViewBag.SystemConfiguration = data;
            return View();
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult SystemConfigurationkey(SystemConfiguration info)
        {
            EMG_Code14 obj = new EMG_Code14();
            long result = 0;
            result = cmm.AddSystemConfiguration(info);
            if (result == 1)
            {

                TempData["MessageTypeSCK"] = "Success";
                TempData["CustomMessageSCK"] = "System configuration updated successfully.";

            }
            else
            {
                TempData["MessageTypeSCK"] = "Error";
                TempData["CustomMessageSCK"] = "System configuration already saved.";

            }

            return RedirectToAction("SystemConfigurationkey", "Admin");
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult SystemConfigurationAdmin(SystemConfigurationAdmin info)
        {
            long result = 0;
            result = cmm.AddSystemConfigurationAdmin(info);
            if (result == 1)
            {
                TempData["MessageTypeSCA"] = "Success";
                TempData["CustomMessageSCA"] = "System configuration updated successfully.";
            }
            return RedirectToAction("SystemConfigurationAdmin", "Admin");
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditSystemConfiguration(int ID)
        {
            SystemConfiguration details = null;
            try
            {
                details = cmm.EditSystemConfiguration(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var data = cmm.GetSystemConfiguration();
            ViewBag.SystemConfiguration = data;
            //  return RedirectToAction("PlanCategory", "Admin",details);
            return View("SystemConfigurationkey", details);
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult SystemConfigurationAdmin()
        {
            var SystemConfigurationAdminData = cmm.GetSystemConfigurationAdmin(0);
            ViewBag.SystemConfigurationAdmin = SystemConfigurationAdminData;

            return View();
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditSystemConfigurationAdmin(int ID)
        {
            SystemConfigurationAdmin details = null;
            try
            {
                details = cmm.EditSystemConfigurationAdmin(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var data = cmm.GetSystemConfigurationAdmin(0);
            ViewBag.SystemConfigurationAdmin = data;
            //  return RedirectToAction("PlanCategory", "Admin",details);
            return View("SystemConfigurationAdmin", details);
        }
        [HttpGet]
        [sessionexpireattribute]
        public ActionResult ChangePassword()
        {
            return View();
        }
        public JsonResult ChangeAdminPassword(string OldPassword, string NewPassword)
        {
            int result = 0;
            try
            {
                string OldPswd = Helper.Encrypt(OldPassword);
                string NewPswd = Helper.Encrypt(NewPassword);
                UserManagement um = new UserManagement();
                string AdminEmail = Session["AdminEmail"].ToString();

                Administration objadmin = new Administration();
                result = objadmin.ChangePassword(AdminEmail, OldPswd, NewPswd);
                if (result > 0)
                {
                    TempData["MessageType"] = "Success";
                    TempData["CustomMessage"] = "Password successfully changed.";
                }
                else if (result == -1)
                {
                    TempData["MessageType"] = "Error";
                    TempData["CustomMessage"] = "Incorrect old password.";
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else if (result == 0)
                {
                    TempData["MessageType"] = "Error";
                    TempData["CustomMessage"] = "Old password and new password are same.";
                }
                else
                {
                    TempData["MessageType"] = "Error";
                    TempData["CustomMessage"] = "Some error while processing your request.";
                }
            }
            catch (Exception ex)
            {
                // ErrorLog.AdminErrorLog("Admin", "SubAdminController", "ActionResult ChangeAdminPassword()", "ActionResult=[HttpPost]", ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult ManageInactiveClients(int? page, string sort = null, string filter = null, string pageSize = null)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            long admID = Convert.ToInt64(Session["AdminID"].ToString());
            string search = Request.Form["search"];
            //ViewBag.ClientList = objClientMngmt.GetAllClient();
            //return View();
            int _PageSize = pagesize.ManageComapanyPageSize;
            int _pageSize = Convert.ToInt32(_PageSize);
            int Size = Convert.ToInt32(pageSize);
            if (Size != null && Size != 0)
                Size = Convert.ToInt32(Size);
            int pageNumber = (page ?? 1);
            TempData["Page"] = pageNumber;
            var ClientData = objClientMngmt.GetAllInactiveClients(search, admID);
            ViewBag.PageSize = Size;
            ViewBag.Total = ClientData.Count;
            ViewBag.CurrentFilter = filter == null ? "clientname" : filter;
            sort = sort == null ? "asc" : sort;
            if (Size == 0)
            {
                Size = 50;
            }
            switch (filter)
            {
                case "clientname":
                    ClientData = sort == "desc" ? ClientData.OrderByDescending(s => s.CName).ToList() : ClientData.OrderBy(s => s.CName).ToList();
                    break;
                case "Employee":
                    ClientData = sort == "desc" ? ClientData.OrderByDescending(s => s.tblCompany_Employee_BasicInfo.Where(a => a.CompanyId == s.ID).Count()).ToList() : ClientData.OrderBy(s => s.tblCompany_Employee_BasicInfo.Where(a => a.CompanyId == s.ID).Count()).ToList();
                    break;
                case "status":
                    ClientData = sort == "desc" ? ClientData.OrderByDescending(s => s.IsActive).ToList() : ClientData.OrderBy(s => s.IsActive).ToList();
                    break;
            }
            ViewBag.CurrentSort = sort;
            sort = sort == "asc" ? "desc" : "asc";
            ViewBag.SortingOrder = sort;
            // ViewBag.BrokerEmpCount = objClientMngmt.GetAllClient().Count;
            if (Size != 1)
            {
                ViewBag.PageNumber = pageNumber > ClientData.Count ? ClientData.Count : pageNumber;
                ViewBag.PageLength = pageNumber * Size > ClientData.Count ? ClientData.Count : pageNumber * Size;
            }
            else
            {
                //ViewBag.PageNumber = ClientData.Count/2;
                ViewBag.PageLength = ClientData.Count;
                Size = ClientData.Count / 1000;
                if (Size == 0)
                {
                    Size = 1000;
                }

            }
            return View(ClientData.ToPagedList(pageNumber, Size));
        }
        // Subscription Plans
        [sessionexpireattribute]
        public ActionResult SubscriptionPlan()
        {
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var subsData = cmm.GetSubscriptionPlan(0);
            ViewBag.SubscriptionData = subsData;

            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data = new SelectList(cdata, "ID", "CCompanyName", 0);
            ViewBag.Company = data;

            var cdata1 = objComEmpMgnt.GetEMGProduct();
            SelectList data1 = new SelectList(cdata1, "ID", "ProductName", 0);
            ViewBag.Product = data1;

            return View();
        }
        [sessionexpireattribute]
        public ActionResult GetCompanyDropdown()
        {
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            return Json(cdata, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult SubscriptionPlan(EMG_SubscriptionPlan obj)
        {
            EMG_SubscriptionPlan objdetails = new EMG_SubscriptionPlan();
            long result = 0;
            result = cmm.AddSubscriptionPlan(obj);
            if (result == 1)
            {

                TempData["MessageTypeSP"] = "Success";
                TempData["CustomMessageSP"] = "Subscription plan saved successfully.";

            }
            else
            {
                TempData["MessageTypeSP"] = "Error";
                TempData["CustomMessageSP"] = "Some error while processing your request.";

            }

            return RedirectToAction("SubscriptionPlan", "Admin");
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditSubscriptionPlan(int ID)
        {
            EMG_SubscriptionPlan details = null;
            try
            {
                PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
                details = cmm.EditSubscriptionPlan(ID);

                var cdata = objComEmpMgnt.GetCompanyDropdown();
                SelectList data = new SelectList(cdata, "ID", "CCompanyName", details.CompanyID);


                var cdata1 = objComEmpMgnt.GetEMGProduct();
                SelectList data1 = new SelectList(cdata1, "ID", "ProductName", details.SubcriptionPlanID);

                ViewBag.Company = data;
                ViewBag.Product = data1;

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var data2 = cmm.GetSubscriptionPlan(0);
            ViewBag.SubscriptionData = data2;
            //  return RedirectToAction("PlanCategory", "Admin",details);
            return View("SubscriptionPlan", details);
        }

        [sessionexpireattribute]
        public ActionResult DeleteSubscriptionPlan(string EnrollId)
        {
            long ID = Convert.ToInt64(EnrollId);
            int result = cmm.DeleteSubscriptionPlan(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult SubscriptionPlans(string value)
        {

            var plandeductionData = cmm.GetSubscriptionPlans(value);
            ViewBag.SubscriptionData = plandeductionData;
            return PartialView("_SubscriptionPlan");
        }
        [sessionexpireattribute]
        public ActionResult ManagePurchaseSubscription(int CompanyID = 0)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");

            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data = new SelectList(cdata, "ID", "CCompanyName", CompanyID);
            ViewBag.Company = data;
            long admID = Convert.ToInt64(Session["AdminID"].ToString());
            string search = Request.Form["search"];
            var ClientData = objClientMngmt.GetPurchaseSubscription(CompanyID);
            ViewBag.PurchaseSubscription = ClientData;
            return View("ManagePurchaseSubscription");
        }
        [sessionexpireattribute]
        public ActionResult ManagePurchaseHistory()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            var ClientData = cmm.GetPurchaseHiistory();
            ViewBag.PurchaseHistory = ClientData;
            return View();
        }
        [HttpGet]
        [sessionexpireattribute]
        public ActionResult AddIsConfigure(long cID)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");

            CompanyManagement objClientMngmt = new CompanyManagement();
            var ClientData1 = objClientMngmt.GetPurchaseSubscription(cID);
            ViewBag.PurchaseSubscription = ClientData1;
            return View("ManagePurchaseSubscription", new { CompanyID = cID });
        }
        [HttpPost]
        [sessionexpireattribute]
        public ActionResult AddIsConfigure(long SubscriptionID, long cID, string ProductCost)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");

            CompanyManagement objClientMngmt = new CompanyManagement();
            var ClientData = objClientMngmt.AddSubscriptionPlan(cID, SubscriptionID, ProductCost);
            var ClientData1 = objClientMngmt.GetPurchaseSubscription(cID);
            ViewBag.PurchaseSubscription = ClientData1;
            return RedirectToAction("ManagePurchaseSubscription", new { CompanyID = cID });
        }
        [sessionexpireattribute]
        public ActionResult InactiveSubscriptionPlan(long SubscriptionID, long cID)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");

            CompanyManagement objClientMngmt = new CompanyManagement();
            var ClientData = objClientMngmt.InactiveSubscriptionPlan(cID, SubscriptionID);
            var ClientData1 = objClientMngmt.GetPurchaseSubscription(cID);
            ViewBag.PurchaseSubscription = ClientData1;
            return View("ManagePurchaseSubscription");
        }
        [sessionexpireattribute]
        public ActionResult GetInvoiceDetails(long SubscribeID)
        {
            List<EMG_InvoiceDetails> details = null;
            details = objClientMngmt.GetInvoiceDetails(SubscribeID);
            int data = 0;
            for (int i = 0; i < details.Count; i++)
            {
                if (details[i].Month == (DateTime.UtcNow.ToString("MMMM") + " " + DateTime.UtcNow.Year).ToString())
                {
                    data = 1;
                }
            }
            if (data != 1)
            {
                details.Add(new EMG_InvoiceDetails { ID = 0, SubscribeID = SubscribeID, Month = (DateTime.UtcNow.ToString("MMMM") + " " + DateTime.UtcNow.Year).ToString(), Status = false });
            }
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public ActionResult SaveInvoiceDetails(long SubscribeID, string Month, bool Status)
        {

            string insertedStr = Month.Insert(Month.Length - 4, " ");
            int i = objClientMngmt.SaveInvoiceDetails(SubscribeID, insertedStr, Status);
            var ClientData = cmm.GetPurchaseHiistory();
            ViewBag.PurchaseHistory = ClientData;
            return View("ManagePurchaseHistory");
        }
        [sessionexpireattribute]
        public ActionResult SavePendingStatus(long ID)
        {
            int i = objClientMngmt.SavePendingStatus(ID);
            var ClientData = cmm.GetPurchaseHiistory();
            ViewBag.PurchaseHistory = ClientData;
            return View("ManagePurchaseHistory");
        }
        [sessionexpireattribute]
        public ActionResult GridData()
        {
            return View();
        }
        [sessionexpireattribute]
        public ActionResult expandableRowTemplate()
        {
            return View();

        }

        [sessionexpireattribute]
        public ActionResult EDIReport()
        {
            long admID = Convert.ToInt64(Session["AdminID"].ToString());
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var cdata = objComEmpMgnt.GetCompanylistAdmin();
            SelectList data = new SelectList(cdata, "ID", "CCompanyName");
            ViewBag.Company = data;

            return View();
        }

        [sessionexpireattribute]
        [HttpPost]
        public ActionResult EDIReport(Int64 CompanyID, int EDIType, string chkMedical, string checkDental, string checkVision)
        {
            long admID = Convert.ToInt64(Session["AdminID"].ToString());
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var cdata = objComEmpMgnt.GetCompanylistAdmin();
            SelectList data = new SelectList(cdata, "ID", "CCompanyName");
            ViewBag.Company = data;

            int result = DownloadCSV(objComEmpMgnt, CompanyID, EDIType, chkMedical, checkDental, checkVision);

            if (result == 1)
            {

                TempData["MessageTypeED"] = "Success";
                TempData["CustomMessageED"] = "CSV downloaded saved successfully.";

            }
            else
            {
                TempData["MessageTypeED"] = "Error";
                TempData["CustomMessageED"] = "No record found.";

            }

            return View();
        }
        public int DownloadCSV(PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt, Int64 CompanyID, int EDIType, string chkMedical, string checkDental, string checkVision)
        {
            int Result = 0;
            //using (DataTable dt = new DataTable())
            //{
            DataTable dt = new DataTable();
            dt = objComEmpMgnt.GetEDIData(CompanyID, EDIType, chkMedical, checkDental, checkVision);
            if (dt.Rows.Count > 0)
            {

                //Build the CSV file data as a Comma separated string.
                string csv = string.Empty;
                //foreach (DataColumn column in dt.Columns)
                //{
                //    //Add the Header row for CSV file.
                //    csv += column.ColumnName + ',';
                //}
                ////Add new line.
                //csv += "\r\n";
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName.ToString() != "SSN" && column.ColumnName.ToString() != "DepSSN" && column.ColumnName.ToString() != "CompanyId" && column.ColumnName.ToString() != "Seq" && column.ColumnName.ToString() != "Duplicate")
                            //Add the Data rows.
                            csv += row[column.ColumnName].ToString().Replace(",", ";") + '|';
                    }
                    //Add new line.
                    csv += "\r\n";
                }
                Result = 1;
                string fileName = string.Empty;
                int num = new Random().Next(1000, 9999);
                if (!string.IsNullOrEmpty(chkMedical))
                {
                    fileName = "HLT" + DateTime.Now.ToString("MMdd");
                }
                else if (!string.IsNullOrEmpty(checkDental))
                {
                    fileName = "DEN" + DateTime.Now.ToString("MMdd");
                }
                else if (!string.IsNullOrEmpty(checkVision))
                {
                    fileName = "VIS" + DateTime.Now.ToString("MMdd");
                }
                else
                {
                    fileName = "HLT" + DateTime.Now.ToString("MMdd");
                }
                //Download the CSV file.
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".csv");
                Response.Charset = "";
                //Response.ContentType = "application/text";
                Response.ContentType = "application/x-msexcel";
                Response.Output.Write(csv);
                Response.Flush();
                Response.End();
            }
            else
            {
                Result = 0;
            }
            return Result;
        }

        [sessionexpireattribute]

        public ActionResult EMGProducts()
        {
            var data = cmm.GetEMGProducts();
            ViewBag.EMGProducts = data;
            return View();
        }

        [sessionexpireattribute]
        [HttpPost]

        public ActionResult EMGProducts(EMG_Products obj)
        {
            EMG_Products objdetails = new EMG_Products();
            long result = 0;

            result = cmm.AddEMGProduct(obj);
            if (result == 1)
            {

                TempData["MessageTypeEMGP"] = "Success";
                TempData["CustomMessageEMGP"] = "EMG product saved successfully.";

            }
            else
            {
                TempData["MessageTypeEMGP"] = "Error";
                TempData["CustomMessageEMGP"] = "Some error while processing your request.";

            }


            return RedirectToAction("EMGProducts", "Admin");
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditEMGProducts(int ID)
        {
            EMG_Products details = null;
            try
            {
                details = cmm.EditEMGProducts(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var data = cmm.GetEMGProducts();
            ViewBag.EMGProducts = data;

            return View("EMGProducts", details);
        }
        [sessionexpireattribute]
        public ActionResult EmployeePasswordReset(string userName)
        {
            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    UserManagement um = new UserManagement();
                    userName = userName.Replace(" ", "+");

                    var request = um.ChangePasswordRecovery(Helper.Decrypt(userName));
                    if (request != null)
                    {
                        ViewData["MessageType"] = "success";
                        ViewData["Message"] = "Thank you for verifying, kindly create your password.";
                        ViewBag.UserID = request.ID;
                        ViewBag.UserName = Helper.Decrypt(userName);
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
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult EmployeePasswordReset()
        {
            try
            {
                int ID = Convert.ToInt32(Request.Form["userID"]);
                string Password = Helper.Encrypt(Request.Form["resetPassword"]);
                UserManagement um = new UserManagement();
                int i = um.ActivePasswordRequest(ID, Password);
                if (i > 0)
                {
                    var obj = objuser.EmployeeLoginByID(i);
                    Session["EmployeeId"] = obj.ID;
                    Session["EmployeeCompanyId"] = obj.CompanyId;
                    Session["EmployeeEmail"] = obj.Email;
                    Session["PopUp"] = "PopUp";
                    return RedirectToAction("Index", "Dashboard", new { area = "Employee" });
                }
                else
                {
                    ViewData["MessageType"] = "error";
                    ViewData["Message"] = "Some error occurred while processing the request. Please try again.";
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
        [sessionexpireattribute]
        public ActionResult SendMailonCompanyEmployees(string empID)
        {
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
                        data = objComEmpMgnt.GetOpenEmrollmentDateAdmin(Convert.ToInt64(employmentInfo.MonthlyBasisID), Convert.ToInt64(employmentInfo.DivisionID));
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

                    Mailbody = Mailbody.Replace("###CompanyNAME###", companyDetails.CCompanyName);
                    if (data != null)
                    {
                        if (companyDetails.CCompanyName == "MUFG Investor Services")
                        {
                            // this is for hard for temparary tommorw will changed
                            data.EndDate = Convert.ToDateTime("10-19-2018 00:00:00.000");
                        }
                        Mailbody = Mailbody.Replace("###EndDate###", string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(data.EndDate)));
                    }
                    // Email sending 
                    if (!string.IsNullOrEmpty(objComEmpBasicInfo[0].Email))
                    {
                        // Date21Sept as per client Discussion code comment 
                        Helper.SendEmail(objComEmpBasicInfo[0].Email, Mailbody, "Welcome to the " + companyDetails.CCompanyName + " Open Enrollment");
                    }




                    int statusPhone = 0;

                    if (!string.IsNullOrEmpty(employmentPersonalInfo.Phone))
                    {
                        // Twillo SMS api integrated
                        // Date21Sept as per client Discussion code comment
                        string URLReset = baseurl + "/Home/PasswordUserReset?email=" + Helper.Encrypt(objComEmpBasicInfo[0].Email);
                        string SmsText = objComEmpBasicInfo[0].FirstName + ", Welcome to " + companyDetails.CCompanyName + " new benefits management system.  Click this link to set up a password and get started " + URLReset;
                        statusPhone = SendTwilloMessage.SendmessageTwillo(employmentPersonalInfo.Phone, SmsText);
                    }
                    //StringBuilder str = new StringBuilder();
                    //str.Append("<div class=\"alert-cont\" style=\" width:700px\">");
                    //str.Append("<h4 class=\"alert-heading\" style=\"color:white\">");
                    //str.Append("Thanks for signing up !<br/><br/>");
                    //str.Append("Your account has been registered.<br/><br/>");
                    //str.Append("A confirmation email has been sent to your email - <font color='brown'>" + objComEmpBasicInfo[0].Email + "</font>");
                    //str.Append("<br/><br/>Thanks<br/><br/>");
                    //str.Append("</h4>");
                    //str.Append("</div>");


                }

            }

            return RedirectToAction("ManageEmployees");
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditPermissions(long CompanyID)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data = new SelectList(cdata, "ID", "CCompanyName", CompanyID);
            ViewBag.Company = data;
            if (CompanyID != 0)
            {
                ViewBag.Permissions = objComEmpMgnt.GetPermissionsByCompany(CompanyID);
            }

            return View();
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult ManagePermissions()
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            ViewBag.Company = objComEmpMgnt.GetCompanyByPermissions();
            return View();
        }
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult _EditPermissions(long CompanyID)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            var cdata = objComEmpMgnt.GetCompanyDropdown();
            SelectList data = new SelectList(cdata, "ID", "CCompanyName", CompanyID);
            ViewBag.Company = data;
            ViewBag.Permissions = objComEmpMgnt.GetPermissionsByCompany(CompanyID);
            return PartialView("_EditPermissions");
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult EditPermissions()
        {
            PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement objComEmpMgnt = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement();
            if (!string.IsNullOrEmpty(Request.Form["CompanyID"]))
            {
                long CompanyID = Convert.ToInt64(Request.Form["CompanyID"]);
                string AccessRightId = Request.Form["AccessRightIds"];
                int result = objComEmpMgnt.SavePermissionsByCompany(CompanyID, AccessRightId);
            }
            return RedirectToAction("ManagePermissions");
        }


        // For Property Group Master 
        [sessionexpireattribute]
        [HttpGet]
        // Listing Property Group Master 
        public ActionResult PropertyGroupMaster()
        {
            var PropertyGroupMasterData = cmm.GetPropertyGroupMaster(0);
            ViewBag.PropertyGroupMaster = PropertyGroupMasterData;
            GetDropDownValues Objddl = new GetDropDownValues();
            ViewBag.PlanCategory = Objddl.GetPlanCategoryMaster();
            return View();

        }

        [sessionexpireattribute]
        [HttpPost]
        public ActionResult PropertyGroupMaster(EMG_PropertyGroupMaster Pgm)
        {
            int groupid = Pgm.GroupId;
            long result = 0;
            int admID = Convert.ToInt32(Session["AdminID"].ToString());
            Pgm.CreatedBy = admID;
            Pgm.ModifiedBy = admID;
            Pgm.PropertyGroupName = Pgm.PropertyGroupName.Trim();
            result = cmm.AddPropertyGroupMaster(Pgm);
            if (result == 1)
            {
                if (result == 1 && groupid == 0)
                {
                    TempData["MessageTypePGM"] = "Success";
                    TempData["CustomMessagePGM"] = "Property group name saved successfully.";
                }
                else
                {
                    TempData["MessageTypePGM"] = "Success";
                    TempData["CustomMessagePGM"] = "Property group name updated successfully.";
                }

            }
            else
            {
                TempData["MessageTypePGM"] = "Error";
                TempData["CustomMessagePGM"] = "Property group name already exists. Please try another name";
            }

            return RedirectToAction("PropertyGroupMaster", "Admin");
        }
        // Edit the property group master
        [HttpGet]
        [sessionexpireattribute]
        public ActionResult EditPropertyGroupMaster(int ID)
        {
            GetDropDownValues Objddl = new GetDropDownValues();
            EMG_PropertyGroupMaster details = null;
            try
            {
                details = cmm.EditPropertyGroupMaster(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var PropertyGroupMasterData = cmm.GetPropertyGroupMaster(0);
            ViewBag.PropertyGroupMaster = PropertyGroupMasterData;
            ViewBag.PlanCategory = Objddl.GetPlanCategoryMaster();

            return View("PropertyGroupMaster", details);
        }
        // delete the property group master
        public ActionResult DeletePropertyGroupMaster(string GroupId)
        {
            long ID = Convert.ToInt64(GroupId);
            int result = cmm.DeletePropertyGroupMaster(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // get Deleted list
        [sessionexpireattribute]
        public ActionResult GetPropertyGroupName(int PlanCategoryId)
        {
            //var Data = cmm.GetPropertyGroupName(PlanCategoryId);

            return Json(cmm.GetPropertyGroupName(PlanCategoryId), JsonRequestBehavior.AllowGet);

        }
        [sessionexpireattribute]
        public ActionResult GetDeletedPropertyGroupMaster(string value)
        {
            var PropertyGroupMasterData = cmm.GetDeletedPropertyGroupMaster(value);
            ViewBag.PropertyGroupMaster = PropertyGroupMasterData;
            return PartialView("_PropertyGroupMaster");
        }

        #region
        // Property Master 
        [sessionexpireattribute]
        public ActionResult PropertyMaster()
        {
            EMG_PropertyMaster obj = new EMG_PropertyMaster();
            GetDropDownValues Objddl = new GetDropDownValues();

            obj.PlanCategoryId = Convert.ToInt32(TempData["PlanCategoryId"]);
            obj.GroupId = Convert.ToInt32(TempData["GroupId"]);
            var PropertyMasterData = cmm.GetPropertyMaster(0);
            ViewBag.PropertyMaster = PropertyMasterData;
            ViewBag.PropertyGroupMaster = Objddl.GetPropertyGroupMaster(Convert.ToInt32(obj.PlanCategoryId));
            ViewBag.PlanCategory = Objddl.GetPlanCategoryMaster();
            return View(obj);
        }

        [sessionexpireattribute]
        [HttpPost]
        public ActionResult PropertyMaster(EMG_PropertyMaster Pgm)
        {
            GetDropDownValues Objddl = new GetDropDownValues();
            EMG_PropertyMaster obj = new EMG_PropertyMaster();
            long result = 0;
            int admID = Convert.ToInt32(Session["AdminID"].ToString());
            Pgm.CreatedBy = admID;
            Pgm.ModifiedBy = admID;
            Pgm.PropertyName = Pgm.PropertyName.Trim();
            result = cmm.AddPropertyMaster(Pgm);
            if (result == 1)
            {

                TempData["PlanCategoryId"] = Pgm.PlanCategoryId;
                TempData["GroupId"] = Pgm.GroupId;

                TempData["MessageType"] = "Success";
                TempData["CustomMessage"] = "Property name saved successfully.";



            }
            else
            {

                TempData["PlanCategoryId"] = Pgm.PlanCategoryId;
                TempData["GroupId"] = Pgm.GroupId;
                TempData["MessageType"] = "Error";
                TempData["CustomMessage"] = "Property name already exists. Please try another name";
            }

            return RedirectToAction("PropertyMaster", "Admin");
        }
        // Edit the property group master
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditPropertyMaster(int ID)
        {
            GetDropDownValues Objddl = new GetDropDownValues();
            EMG_PropertyMaster details = null;
            try
            {
                details = cmm.EditPropertyMaster(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var PropertyMasterData = cmm.GetPropertyMaster(0);
            ViewBag.PropertyMaster = PropertyMasterData;
            ViewBag.PropertyGroupMaster = Objddl.GetPropertyGroupMaster(Convert.ToInt32(details.PlanCategoryId));
            ViewBag.PlanCategory = Objddl.GetPlanCategoryMaster();
            return View("PropertyMaster", details);
        }
        [sessionexpireattribute]
        // delete the property group master
        public ActionResult DeletePropertyMaster(string PropertyId)
        {
            long ID = Convert.ToInt64(PropertyId);
            int result = cmm.DeletePropertyMaster(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // get Deleted list
        [sessionexpireattribute]
        public ActionResult GetDeletedPropertyMaster(string value)
        {
            var PropertyMasterData = cmm.GetDeletedPropertyMaster(value);
            ViewBag.PropertyMaster = PropertyMasterData;
            return PartialView("_PropertyMaster");
        }
        [sessionexpireattribute]
        public ActionResult SearchPropertyMaster(string value)
        {
            var PropertyMasterData = cmm.SearchPropertyMaster(value);
            ViewBag.PropertyMaster = PropertyMasterData;
            return PartialView("_PropertyMaster");
        }

        #endregion

        #region  TerminateReason
        // For Property Group Master 
        [sessionexpireattribute]
        [HttpGet]

        // Listing Property Group Master 
        public ActionResult Reason()
        {
            var ReasonMasterData = cmm.GetReasonMaster(0);
            ViewBag.ReasonMaster = ReasonMasterData;
            return View();

        }

        [sessionexpireattribute]
        [HttpPost]
        public ActionResult Reason(EMG_Reason Pgm)
        {
            EMG_Reason obj = new EMG_Reason();
            long result = 0;
            int admID = Convert.ToInt32(Session["AdminID"].ToString());
            Pgm.CreatedBy = admID;
            Pgm.ModifiedBy = admID;
            Pgm.ReasonName = Pgm.ReasonName.Trim();
            result = cmm.AddReasonMaster(Pgm);
            if (result == 1)
            {

                TempData["MessageTypeReason"] = "Success";
                TempData["CustomMessageReason"] = "Reason saved successfully.";
            }
            else
            {
                TempData["MessageTypeReason"] = "Error";
                TempData["CustomMessageReason"] = "Reason name already exists. Please try another name.";
            }

            return RedirectToAction("Reason", "Admin");
        }
        // Edit the property group master
        [HttpGet]
        [sessionexpireattribute]
        public ActionResult EditReason(int ID)
        {

            EMG_Reason details = null;
            try
            {
                details = cmm.EditReasonMaster(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var ReasonMasterData = cmm.GetReasonMaster(0);
            ViewBag.ReasonMaster = ReasonMasterData;


            return View("Reason", details);
        }
        #endregion

        #region Add company
        [sessionexpireattribute]
        public ActionResult AddCompany(long ID = 0)
        {
            Company obj = new Company();
            List<CompanyClient> listClient = new List<CompanyClient>();
            GetDropDownValues Objddl = new GetDropDownValues();
            ViewBag.BrokerGet = Objddl.GetBrokerMaster();
            if (ID == 0)
            {
                CompanyClient client = new CompanyClient();
                listClient.Add(client);
                obj.Clients = listClient;
            }
            else
            {
                var details = objClientMngmt.GetClientDetils(ID);
                obj.ID = details.ID;
                obj.CustomerCode = details.CustomerCode;
                obj.CCompanyName = details.CCompanyName;
                obj.IsActive = details.IsActive;
                obj.CStreetName = details.CStreetName;
                obj.CCityName = details.CityMaster.City;
                obj.CStateName = details.StateMaster.State;
                obj.CCountryName = details.CountryMaster.Country;
                obj.CContact = details.CContact;
                obj.CZipCode = details.CZipCode;
                obj.URL = details.Url;
                obj.CEmail = details.CEmail;
                obj.BrokerId = details.BrokerId;
                obj.ISA05IDQualifierFedTaxID = details.ISA05IDQualifierFedTaxID;
                obj.ISA06SenderID = details.ISA06SenderID;
                obj.ISA07IDQualifier = details.ISA07IDQualifier;
                obj.ISA08ReceiverID = details.ISA08ReceiverID;

                obj.IsActive = (details.IsActive == null ? false : details.IsActive);

                List<CompanyClient> client = new List<CompanyClient>();
                client = (from a in details.tblCompany_Employee_BasicInfo.Where(a => a.RoleID == 1 && a.CompanyId == details.ID)
                          select new CompanyClient
                          {
                              ID = a.ID,
                              FirstName = a.FirstName,
                              LastName = a.LastName,
                              Email = a.Email,
                              AdministratorType = a.AdministratorType,
                              Phone = a.tblCompany_Employee_PersonalInfo.Where(q => q.EmployeeID == a.ID).FirstOrDefault().Phone,
                              WorkPhone = a.tblCompany_Employee_PersonalInfo.Where(q => q.EmployeeID == a.ID).FirstOrDefault().WorkPhone,
                              IsActive = a.IsActive
                          }).ToList();

                ViewBag.Clients = client;
                BrokerAssign objBrokerAssignUser = new BrokerAssign();

                objBrokerAssignUser = objClientMngmt.GetBrokerAssign(details.ID);
                if (objBrokerAssignUser != null)
                {
                    if (objBrokerAssignUser.StartDate != null)
                    {
                        obj.BrokerStartDate = Convert.ToDateTime(objBrokerAssignUser.StartDate);
                    }
                    if (objBrokerAssignUser.EndDate != null)
                    {
                        obj.BrokerEndDate = Convert.ToDateTime(objBrokerAssignUser.EndDate);
                    }


                }

                //tblBroker objbroker = details.tblBroker;
                //CompanyBroker brokerData = new CompanyBroker();
                //if (objbroker != null)
                //{
                //    brokerData.BrokerId = objbroker.BrokerId;
                //    brokerData.BrokerName = objbroker.Broker;
                //    brokerData.State = objbroker.StateMaster.State;
                //    brokerData.Country = objbroker.CountryMaster.Country;
                //    brokerData.City = objbroker.CityMaster.City;
                //    brokerData.Street = objbroker.Street;
                //    brokerData.ZipCode = objbroker.ZipCode;
                //    //brokerData.Email = objbroker.Email;
                //    brokerData.WorkPhone = objbroker.WorkPhone;
                //    //brokerData.Phone = objbroker.Phone;
                //    obj.Broker = brokerData;
                //}
                tbl_CustomerBilling objbilling = details.tbl_CustomerBilling.Where(a => a.CompanyId == details.ID).FirstOrDefault();
                Billing billingData = new Billing();
                if (objbilling != null)
                {
                    billingData.BillingId = objbilling.BillingId;
                    billingData.CompanyName = objbilling.CompanyName;
                    billingData.CompanyId = objbilling.CompanyId;
                    billingData.StreetName = objbilling.StreetName;
                    billingData.State = objbilling.StateMaster.State;
                    billingData.Country = objbilling.CountryMaster.Country;
                    billingData.City = objbilling.CityMaster.City;
                    billingData.ZipCode = objbilling.ZipCode;
                    billingData.Fax = objbilling.Fax;
                    billingData.Phone = objbilling.Phone;
                }
                obj.Billing = billingData;

            }

            return View(obj);
        }
        public ActionResult _AddCompany(List<CompanyClient> jsonObj, long CompanyId = 0)
        {
            Company obj = new Company();
            CompanyClient client = new CompanyClient();
            if (jsonObj == null)
            {
                jsonObj = new List<CompanyClient>();
            }
            jsonObj.Add(client);
            obj.Clients = jsonObj;
            ViewBag.addmore = CompanyId;
            return PartialView(obj);
        }

        public ActionResult _CompanyList(long empid = 0, int status = 0, int CID = 0, int flag = 1)
        {
            List<CompanyClient> client = new List<CompanyClient>();
            var details = objClientMngmt.GetClientDetils(CID);
            if (flag == 1)
            {
                var result = objClientMngmt.UpdateStatus(empid, Convert.ToBoolean(status));
                var objemp = details.tblCompany_Employee_BasicInfo.Where(a => a.ID == empid).FirstOrDefault();
                if (empid > 0 && !string.IsNullOrEmpty(objemp.Email))
                {
                    string baseurl = Helper.GetBaseUrl();
                    string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/DeactivateCompany.html"));  // email template
                    Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                    Mailbody = Mailbody.Replace("###Name###", objemp.FirstName);
                    Mailbody = Mailbody.Replace("###CompanyNAME###", objemp.CompanyMaster.CCompanyName);
                    if (!string.IsNullOrEmpty(objemp.Email))
                    {
                        // Date21Sept as per client Discussion code comment 
                        Helper.SendEmail(objemp.Email, Mailbody, "EnrollMyGroup Administration");
                    }

                }

            }
            client = (from a in details.tblCompany_Employee_BasicInfo.Where(a => a.RoleID == 1 && a.CompanyId == details.ID)
                      select new CompanyClient
                      {
                          ID = a.ID,
                          FirstName = a.FirstName,
                          LastName = a.LastName,
                          Email = a.Email,
                          Phone = a.tblCompany_Employee_PersonalInfo.Where(q => q.EmployeeID == a.ID).FirstOrDefault().Phone,
                          WorkPhone = a.tblCompany_Employee_PersonalInfo.Where(q => q.EmployeeID == a.ID).FirstOrDefault().WorkPhone,
                          IsActive = a.IsActive
                      }).ToList();

            ViewBag.Clients = client;
            return PartialView();
        }
        [sessionexpireattribute]
        [HttpPost]
        public ActionResult AddCompany(Company details)
        {
            long comID = details.ID;
            bool flag = true;
            //  var BrokerId = Convert.ToInt64(Request.Form["BrokerId"]);
            var SubbrokerId = Convert.ToString(Request.Form["hdnsubrokeruserids"]);
            GetDropDownValues Objddl = new GetDropDownValues();
            ViewBag.BrokerGet = Objddl.GetBrokerMaster();
            StringBuilder msgError = new StringBuilder();
            if (!string.IsNullOrEmpty(details.CContact))
            {
                details.CContact = objComman.RemoveMasking(details.CContact);
            }

            /////////----------------------------Duplicate checks for company--------------------------//////////////////////////////
            var duplicateComRecord = objClientMngmt.ChechDuplicateEmailandPhone(details.CContact, "", Convert.ToInt32(details.ID), 1);
            if (duplicateComRecord.Count > 0)
            {
                flag = false;
                //msgError.Append("Phone or Email already exists at " + details.CContact);
                msgError.Append("Phone already exists at " + details.CContact);
                TempData["CustomMessage"] = msgError;
                TempData["MessageType"] = "Error";
                return View(details);
            }
            if (details.Clients != null)
            { /////////----------------------------Duplicate checks for Employee--------------------------//////////////////////////////
                foreach (var item in details.Clients)
                {
                    if (!string.IsNullOrEmpty(item.Phone))
                    {
                        item.Phone = objComman.RemoveMasking(item.Phone);
                    }
                    if (!string.IsNullOrEmpty(item.WorkPhone))
                    {
                        item.WorkPhone = objComman.RemoveMasking(item.WorkPhone);
                    }


                    var duplicateEmpRecord = objClientMngmt.ChechDuplicateEmailandPhone("", item.Email, Convert.ToInt32(item.ID), 2);
                    if (duplicateEmpRecord.Count == 1)
                    {
                        if (duplicateEmpRecord[0].UserType == "E" && duplicateEmpRecord[0].CompanyId == details.ID)
                        {
                            item.ID = duplicateEmpRecord[0].ID;
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                            msgError.Append("Email already exists");
                            TempData["CustomMessage"] = msgError;
                            TempData["MessageType"] = "Error";
                            return View(details);
                        }

                    }
                    else if (duplicateEmpRecord.Count > 0)
                    {
                        flag = false;
                        msgError.Append("Phone or Email already exists at " + item.Phone);
                        TempData["CustomMessage"] = msgError;
                        TempData["MessageType"] = "Error";
                        return View(details);
                    }


                    //Check for phone Duplicate 
                    if (!string.IsNullOrEmpty(item.Phone))
                    {
                        var duplicateEmpRecordPhone = objClientMngmt.ChechDuplicateEmailandPhone(item.Phone, "", Convert.ToInt32(item.ID), 2);
                        if (duplicateEmpRecordPhone.Count == 1)
                        {
                            if (duplicateEmpRecordPhone[0].UserType == "E" && duplicateEmpRecordPhone[0].CompanyId == details.ID)
                            {
                                item.ID = duplicateEmpRecordPhone[0].ID;
                                flag = true;
                            }
                            else
                            {
                                flag = false;
                                msgError.Append("Phone already exists at " + item.Phone);
                                TempData["CustomMessage"] = msgError;
                                TempData["MessageType"] = "Error";
                                return View(details);
                            }

                        }
                    }



                }

            }

            /////////----------------------------Duplicate checks for Broker--------------------------//////////////////////////////
            //if (!string.IsNullOrEmpty(details.Broker.Phone))
            //{
            //    details.Broker.Phone = objComman.RemoveMasking(details.Broker.Phone);
            //}
            //if (!string.IsNullOrEmpty(details.Broker.WorkPhone))
            //{
            //    details.Broker.WorkPhone = objComman.RemoveMasking(details.Broker.WorkPhone);
            //}
            //if (!string.IsNullOrEmpty(details.Billing.Phone))
            //{
            //    details.Billing.Phone = objComman.RemoveMasking(details.Billing.Phone);
            //}


            //var duplicateBrokerRecord = objClientMngmt.ChechDuplicateEmailandPhone(details.Broker.Phone, details.Broker.Email, Convert.ToInt32(details.Broker.BrokerId), 3);

            //if (duplicateBrokerRecord.Count == 1)
            //{
            //    if (duplicateBrokerRecord[0].Phone == details.Broker.Phone && duplicateBrokerRecord[0].Email == details.Broker.Email)
            //    {
            //        details.Broker.BrokerId = duplicateBrokerRecord[0].ID;
            //        flag = true;
            //    }
            //    else
            //    {
            //        flag = false;
            //        msgError.Append("Phone or Email already exists at " + details.Broker.Phone);
            //        TempData["CustomMessage"] = msgError;
            //        TempData["MessageType"] = "Error";
            //        return View(details);
            //    }
            //}
            //else if (duplicateBrokerRecord.Count > 0)
            //{
            //    flag = false;
            //    msgError.Append("Phone or Email already exists at " + details.Broker.Phone);
            //    TempData["CustomMessage"] = msgError;
            //    TempData["MessageType"] = "Error";
            //    return View(details);
            //}

            /////////----------------------------Add data in all tables if data record was found duplicate--------------------------//////////////////////////////
            if (flag == true)
            {
                long result = objClientMngmt.AddCompany(details);
                if (details.IsActive == true)
                {
                    var mulAdmin = new PowerIBrokerBusinessLayer.Company.CompanyEmployeeManagement().GetMutipleCompanyAdmin(details.ID);
                    foreach (var item in mulAdmin)// in case of active mail will be send on multiple company admin.
                    {
                        if (string.IsNullOrEmpty(item.Password))
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
                    }
                }
                if (comID == 0)
                {
                    msgError.Append("Company added successfully.");
                    TempData["CustomMessage"] = msgError;
                    TempData["MessageType"] = "Success";
                }
                else
                {
                    msgError.Append("Company updated successfully.");
                    TempData["CustomMessage"] = msgError;
                    TempData["MessageType"] = "Success";
                }
                return RedirectToAction("AddCompany", new { ID = result });
            }
            else
            {
                return View(details);
            }
        }
        

        [sessionexpireattribute]
        public JsonResult SearchEmployee(string Name, long CompanyId = 0)
        {
            var result = objClientMngmt.SearchEmployee(Name, CompanyId);
            var data = (from a in result
                        select new SelectListItem
                        {
                            Text = a.FirstName + " " + a.LastName,
                            Value = a.ID.ToString()
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [sessionexpireattribute]
        public JsonResult EmployeeDetail(long EmpId)
        {
            var result = objClientMngmt.GetEmploymeInfo(EmpId);
            CompanyClient data = new CompanyClient();
            data.ID = result.ID;
            data.FirstName = result.FirstName;
            data.LastName = result.LastName;
            data.Email = result.Email;
            data.Phone = result.tblCompany_Employee_PersonalInfo.Where(a => a.EmployeeID == result.ID).FirstOrDefault().Phone;
            data.WorkPhone = result.tblCompany_Employee_PersonalInfo.Where(a => a.EmployeeID == result.ID).FirstOrDefault().WorkPhone;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _EditCompanyAdmin(long EmpId)
        {
            var result = objClientMngmt.GetEmploymeInfo(EmpId);
            CompanyClient data = new CompanyClient();
            data.ID = result.ID;
            data.FirstName = result.FirstName;
            data.LastName = result.LastName;
            data.Email = result.Email;
            data.AdministratorType = result.AdministratorType;
            data.Phone = result.tblCompany_Employee_PersonalInfo.Where(a => a.EmployeeID == result.ID).FirstOrDefault().Phone;
            data.WorkPhone = result.tblCompany_Employee_PersonalInfo.Where(a => a.EmployeeID == result.ID).FirstOrDefault().WorkPhone;
            return PartialView(data);
        }
        public ActionResult SaveCompanyAdmin(CompanyClient jsonObj = null, int Flag = 1)
        {
            if (Flag == 1)
            {
                bool flag = true;
                /////////----------------------------Duplicate checks for company--------------------------//////////////////////////////
                var duplicateComRecord = objClientMngmt.ChechDuplicateEmailandPhone(jsonObj.Phone, jsonObj.Email, Convert.ToInt32(jsonObj.ID), 2);
                if (duplicateComRecord.Count > 0)
                {
                    flag = false;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
            {
                long result = objClientMngmt.SaveCompanyAdmin(jsonObj);
                List<CompanyClient> client = new List<CompanyClient>();
                var empdetails = objClientMngmt.GetEmploymeInfo(jsonObj.ID);
                var details = objClientMngmt.GetClientDetils((long)empdetails.CompanyId);
                client = (from a in details.tblCompany_Employee_BasicInfo.Where(a => a.RoleID == 1 && a.CompanyId == details.ID)
                          select new CompanyClient
                          {
                              ID = a.ID,
                              FirstName = a.FirstName,
                              LastName = a.LastName,
                              Email = a.Email,
                              AdministratorType = a.AdministratorType,
                              Phone = a.tblCompany_Employee_PersonalInfo.Where(q => q.EmployeeID == a.ID).FirstOrDefault().Phone,
                              WorkPhone = a.tblCompany_Employee_PersonalInfo.Where(q => q.EmployeeID == a.ID).FirstOrDefault().WorkPhone,
                              IsActive = a.IsActive
                          }).ToList();

                ViewBag.Clients = client;
                return PartialView("_CompanyList", new { empid = jsonObj.ID, status = 0, CID = result, flag = 2 });
            }

        }
        public JsonResult BrokerDetail(long BrokerId)
        {
            var result = objClientMngmt.GetBrokerInfo(BrokerId);
            CompanyBroker data = new CompanyBroker();
            data.BrokerId = result.BrokerId;
            //data.FirstName = result.FirstName;
            //data.LastName = result.LastName;
            data.BrokerName = result.Broker;
            data.Street = result.Street;
            data.City = result.CityMaster.City;
            data.State = result.StateMaster.State;
            data.Country = result.CountryMaster.Country;
            data.ZipCode = result.ZipCode;
            data.Email = result.Email;
            data.Phone = result.Phone;
            data.WorkPhone = result.WorkPhone;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchBroker(string Name)
        {
            var result = objClientMngmt.SearchBroker(Name);
            var data = (from a in result
                        select new SelectListItem
                        {
                            Text = a.Broker,
                            Value = a.BrokerId.ToString()
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Broker
        [HttpGet]
        [sessionexpireattribute]
        public ActionResult AddBroker(long ID = 0)
        {
            if (Session["AdminEmail"] == null)
                return RedirectToAction("Index");
            CompanyBroker obj = new CompanyBroker();
            List<Broker> listBroker = new List<Broker>();
            var BrokerMasterData = cmm.GetBrokerMaster(0);
            ViewBag.BrokerMaster = BrokerMasterData;
            var BrokerUserData = cmm.GetBrokerUserMaster(0, 0);
            ViewBag.Clients = BrokerUserData;
            GetDropDownValues Objddl = new GetDropDownValues();
            ViewBag.BrokerGet = Objddl.GetBrokerMaster();
            if (ID == 0)
            {
                Broker broker = new Broker();
                listBroker.Add(broker);
                obj.Clients = listBroker;


            }

            return View(obj);
        }
        public ActionResult _AddBroker(List<Broker> jsonObj)
        {
            var BrokerMasterData = cmm.GetBrokerMaster(0);
            ViewBag.BrokerMaster = BrokerMasterData;
            GetDropDownValues Objddl = new GetDropDownValues();
            ViewBag.BrokerGet = Objddl.GetBrokerMaster();
            CompanyBroker obj = new CompanyBroker();
            Broker client = new Broker();
            if (jsonObj == null)
            {
                jsonObj = new List<Broker>();
            }
            jsonObj.Add(client);
            obj.Clients = jsonObj;
            return PartialView(obj);
        }
        public ActionResult _ManageBroker()
        {

            var BrokerMasterData = cmm.GetBrokerMaster(0);
            ViewBag.BrokerMaster = BrokerMasterData;
            return PartialView();
        }
        public ActionResult _EditSubBroker(long Subbrokerid)
        {
            GetDropDownValues Objddl = new GetDropDownValues();
            ViewBag.BrokerGet = Objddl.GetBrokerMaster();
            var result = objClientMngmt.GetSubBrokerInfo(Subbrokerid);
            Broker data = new Broker();
            data.SubBrokerId = result.SubBrokerId;
            data.BrokerName = result.Broker;
            data.FirstName = result.FirstName;
            data.LastName = result.LastName;

            data.Email = result.Email;
            data.Phone = result.Phone;
            data.WorkPhone = result.WorkPhone;
            data.BrokerId = result.BrokerId;
            data.ddlBrokerId = result.BrokerId;
            return PartialView(data);
        }
        public ActionResult _EditBroker(long Brokerid)
        {

            var result = objClientMngmt.GetBrokerDetils(Brokerid);
            CompanyBroker data = new CompanyBroker();

            data.BrokerName = result.Broker;
            data.Street = result.Street;
            data.City = result.CityMaster.City;
            data.State = result.StateMaster.State;
            data.Country = result.CountryMaster.Country;
            data.ZipCode = result.ZipCode;
            data.Email = result.Email;
            data.Phone = result.Phone;
            data.WorkPhone = result.WorkPhone;
            data.BrokerId = result.BrokerId;

            return PartialView(data);
        }

        public ActionResult _EditBrokerCompany(long Brokerid)
        {

            var result = objClientMngmt.GetBrokerDetils(Brokerid);
            CompanyBroker data = new CompanyBroker();
            if (result != null)
            {
                data.BrokerName = result.Broker;
                data.Street = result.Street;
                data.City = result.CityMaster.City;
                data.State = result.StateMaster.State;
                data.Country = result.CountryMaster.Country;
                data.ZipCode = result.ZipCode;
                data.Email = result.Email;
                data.Phone = result.Phone;
                data.WorkPhone = result.WorkPhone;
                data.BrokerId = result.BrokerId;
            }
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public ActionResult _SubbrokerList(int flag = 0, int BrokerId = 0)
        {
            var BrokerUserMasterData = cmm.GetBrokerUserMaster(0, BrokerId);

            ViewBag.Clients = BrokerUserMasterData;
            return PartialView();
        }
        public ActionResult _SubbrokerListCompany(int flag = 0, int BrokerId = 0, int Companyid = 0)
        {
            var BrokerUserMasterData = cmm.GetBrokerUserMasterCompany(0, BrokerId, Companyid);

            ViewBag.Clients = BrokerUserMasterData;
            return PartialView();
        }
        public ActionResult SaveBroker(CompanyBroker jsonObj = null)
        {

            bool flag = true;
            /////////----------------------------Duplicate checks for Broker --------------------------//////////////////////////////
            var duplicateComRecord = objClientMngmt.ChechDuplicateEmailandPhone(jsonObj.Phone, jsonObj.Email, Convert.ToInt32(jsonObj.BrokerId), 3);
            if (duplicateComRecord.Count == 0)
            {
                flag = true;
                long result = objClientMngmt.AddBroker(jsonObj);
            }
            else
            {
                flag = false;

            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveSubBroker(Broker jsonObj = null)
        {
            var subbId = jsonObj.SubBrokerId;

            bool flag = true;
            /////////----------------------------Duplicate checks for  Broker User --------------------------//////////////////////////////
            var duplicateComRecord = objClientMngmt.ChechDuplicateEmailandPhone(jsonObj.Phone, jsonObj.Email, Convert.ToInt32(jsonObj.SubBrokerId), 6);
            if (duplicateComRecord.Count == 0)
            {
                flag = true;
                long result = objClientMngmt.SaveSubBroker(jsonObj);
                if (subbId == 0)
                {
                    string baseurl = Helper.GetBaseUrl();
                    string Mailbody = System.IO.File.ReadAllText(Server.MapPath("~/EMGMailer/HTML/Add_New_Company_Broker.html"));  // email template
                    Mailbody = Mailbody.Replace("###HostUrl####", baseurl);
                    Mailbody = Mailbody.Replace("###LoginUserName###", jsonObj.Email);
                    Mailbody = Mailbody.Replace("###UserName###", jsonObj.FirstName);
                    Mailbody = Mailbody.Replace("###UserNAME###", Helper.Encrypt(jsonObj.Email));
                    //Mailbody = Mailbody.Replace("###CompanyName###", jsonObj.BrokerName);
                    if (!string.IsNullOrEmpty(jsonObj.Email))
                    {
                        Helper.SendEmail(jsonObj.Email, Mailbody, "Welcome to EnrollMyGroup");
                    }
                }


            }
            else
            {
                flag = false;

            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateStatusBroker(long BrokerId, bool Status)
        {
            bool flag = true;
            Status = !Status;
            var result = objClientMngmt.UpdateStatusBroker(BrokerId, Status);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateStatusSubBroker(long SubBrokerId, bool Status)
        {
            bool flag = true;
            Status = !Status;
            var result = objClientMngmt.UpdateStatusSubBroker(SubBrokerId, Status);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeactivateBroker(long BrokerId)
        {
            bool flag = true;
            var result = objClientMngmt.DeactivateBroker(BrokerId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteHomePageImage(long Id)
        {
            bool flag = true;
            flag = objClientMngmt.DeleteBannerHomePage(Id);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteContactUsPageImage(long Id)
        {
            bool flag = true;

            flag = objClientMngmt.DeleteBannerContactPage(Id);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteAboutUsPageImage(long Id, string ImageType = "")
        {
            bool flag = true;

            flag = objClientMngmt.DeleteBannerAboutUsPage(Id, ImageType);

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeactivateSubBroker(long SubBrokerId)
        {
            bool flag = true;
            var result = objClientMngmt.DeactivateSubBroker(SubBrokerId);
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _DropdownBrokerComapny()
        {
            GetDropDownValues Objddl = new GetDropDownValues();
            ViewBag.BrokerGet = Objddl.GetBrokerMaster();
            return PartialView();
        }
        public ActionResult _DropdownBrokerComapnyAddCompany()
        {
            GetDropDownValues Objddl = new GetDropDownValues();
            ViewBag.BrokerGet = Objddl.GetBrokerMaster();
            return PartialView();
        }
        public ActionResult _DropdownBrokerCompany()
        {
            GetDropDownValues Objddl = new GetDropDownValues();
            ViewBag.BrokerGet = Objddl.GetBrokerMaster();
            return PartialView();
        }
        #endregion Broker

        #region Age Range by Avdhesh

        [sessionexpireattribute]
        public ActionResult ImputedIncome()
        {
            AgeRangeLife obj = new AgeRangeLife();
            var ImputedIncomeData = cmm.GetImputedIncome();
            ViewBag.ImputedIncome = ImputedIncomeData;

            return View(obj);
        }

        [sessionexpireattribute]
        [HttpPost]
        public ActionResult ImputedIncome(AgeRangeLife ARange)
        {
            AgeRangeLife obj = new AgeRangeLife();
            long result = 0;
            result = cmm.AddImputedIncome(ARange);
            if (result == 1)
            {
                TempData["MessageType"] = "Success";
                TempData["CustomMessage"] = "Imputed Income saved successfully.";
            }
            else
            {
                TempData["MessageType"] = "Error";
                TempData["CustomMessage"] = "Imputed Income already exists. Please try another name";
            }

            return RedirectToAction("ImputedIncome", "Admin");
        }
        // Edit the property group master
        [sessionexpireattribute]
        [HttpGet]
        public ActionResult EditImputedIncome(int ID)
        {
            AgeRangeLife details = null;
            try
            {
                details = cmm.EditImputedIncome(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var ImputedIncomeData = cmm.GetImputedIncome();
            ViewBag.ImputedIncome = ImputedIncomeData;
            return View("ImputedIncome", details);
        }
        [sessionexpireattribute]
        // delete the property group master

        public ActionResult DeleteImputedIncome(int AgeID = 0)
        {
            long ID = Convert.ToInt64(AgeID);
            int result = cmm.DeleteImputedIncome(ID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //// get Deleted list
        //[sessionexpireattribute]
        //public ActionResult GetDeletedImputedIncome(string value)
        //{
        //    var ImputedIncomeData = cmm.GetDeletedImputedIncome(value);
        //    ViewBag.ImputedIncome = ImputedIncomeData;
        //    return PartialView("_ImputedIncome");
        //}
        //[sessionexpireattribute]
        //public ActionResult SearchImputedIncome(string value)
        //{
        //    var ImputedIncomeData = cmm.SearchImputedIncome(value);
        //    ViewBag.ImputedIncome = ImputedIncomeData;
        //    return PartialView("_ImputedIncome");
        //}

        #endregion

        #region Email and SMS Notifications Start
        [HttpGet]
        [sessionexpireattribute]
        public ActionResult Notification()
        {

            var NotificationAdminData = cmm.GetSystemNotificationAdmin();
            ViewBag.NotificationAdmin = NotificationAdminData;
            return View();
        }
        [sessionexpireattribute]
        public ActionResult _Notification()
        {
            return PartialView();
        }
        [HttpGet]
        [sessionexpireattribute]
        public ActionResult EditNotification(int ID)
        {
            SystemConfigurationAdmin details = null;
            try
            {
                details = cmm.EditNotificationAdmin(ID);

            }
            catch (Exception ex)
            {
                Helper.ExceptionHandler.WriteToLogFile(ex.Message);
                //return View();
            }
            var NotificationAdminData = cmm.GetSystemNotificationAdmin();
            ViewBag.NotificationAdmin = NotificationAdminData;

            return View("Notification", details);
        }
        [HttpPost]
        [sessionexpireattribute]
        public ActionResult Notification(SystemConfigurationAdmin obj)
        {
            long result = 0;
            result = cmm.AddNotificationAdmin(obj);
            if (result == 1)
            {
                TempData["MessageTypeNES"] = "Success";
                TempData["CustomMessageNES"] = "Notification updated successfully.";
            }
            else
            {
                TempData["MessageTypeNES"] = "Error";
                TempData["CustomMessageNES"] = "Some error occured.";
            }
            return RedirectToAction("Notification", "Admin");
        }
        #endregion Email and SMS Notifications End

    }

    public class EMGProduct
    {
        public int ID { get; set; }
        public long SubcriptionPlanID { get; set; }
        public string CostPerMonth { get; set; }
        public Nullable<bool> IsConfigure { get; set; }
        public Nullable<long> CompanyID { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string ProductName { get; set; }
    }

    //  for plan group Master

}
