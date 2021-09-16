using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class CompanyController : Controller
    {
        //
        // GET: /Company/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard(string company)
        {
            if (Session["UserEmail"] == null)
                return RedirectToAction("Index","Home");
            return View();
        }

    }
}
