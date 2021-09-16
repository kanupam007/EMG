using PowerIBrokerBusinessLayer;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class SecurityController : Controller
    {
        //
        // GET: /Security/

        public ActionResult Index()
        {
            CmsManagement cmsObj = new CmsManagement();
            var securityContent = cmsObj.GetOtherPageContent((int)Page.Security, false);
            return View(securityContent);
        }

    }
}
