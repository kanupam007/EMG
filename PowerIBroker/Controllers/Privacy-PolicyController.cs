using PowerIBrokerBusinessLayer;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class Privacy_PolicyController : Controller
    {
        //
        // GET: /Privacy-Policy/

        public ActionResult Index()
        {
            CmsManagement cmsObj = new CmsManagement();
            var privacyContent = cmsObj.GetOtherPageContent((int)Page.Policy, false);
            return View(privacyContent);
        }

    }
}
