using PowerIBrokerBusinessLayer;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class FAQsController : Controller
    {
        //
        // GET: /FAQs/

        public ActionResult Index()
        {
            CmsManagement cmsObj = new CmsManagement();
            var faqPageContents = cmsObj.GetFaqPageContent(false);
            ViewBag.FaqPageContents = faqPageContents;
            ViewBag.FaqBanner = cmsObj.getFaqBanner(false);
            return View();
        }

    }
}
