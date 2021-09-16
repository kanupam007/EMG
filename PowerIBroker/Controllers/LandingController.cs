using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PowerIBroker.Controllers
{
    public class LandingController : Controller
    {
        //
        // GET: /Landing/

        public ActionResult Index()
        {
            if (Session["AdminEmail"] != null || Session["EmployeeId"] != null)
            {

            }
            else
            {
                return RedirectToAction("Index", "home");
            }

            return View();
        }

    }
}
