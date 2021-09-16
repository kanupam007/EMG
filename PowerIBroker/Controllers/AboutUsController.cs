using PowerIBrokerBusinessLayer;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class AboutUsController : Controller
    {
        //
        // GET: /AboutUs/

        public ActionResult Index()
        {
            CmsManagement cmsObj = new CmsManagement();
            var aboutUsContent = cmsObj.GetAboutUsPageContent();
            ViewBag.AboutUsContent = aboutUsContent;
            ViewBag.MetaTitle = aboutUsContent.MetaTitle;
            ViewBag.MetaDescription = aboutUsContent.MetaDescriptions;
            ViewBag.MetaKeywords = aboutUsContent.MetaKeywords;
            ViewBag.MetaRobots = aboutUsContent.MetaKeyPhrase;
            return View();


        }

    }
}
