using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult NotFound(string returnUrl=null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public ActionResult BadRequest(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

    }
}
