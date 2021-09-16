using PowerIBrokerBusinessLayer;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class Terms_ConditionsController : Controller
    {
        //
        // GET: /Terms-Conditions/

        public ActionResult Index()
        {
            CmsManagement cmsObj = new CmsManagement();
            var termsContent = cmsObj.GetOtherPageContent((int)Page.TermsConditions, false);
            return View(termsContent);
        }

    }
}
