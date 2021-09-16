using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PowerIBroker
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801


    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new HandleErrorAttribute());

        }
        protected void Application_Error(object sender, EventArgs e)
        {
            //Exception exception = Server.GetLastError();
            //Response.Clear();

            //HttpException httpException = exception as HttpException;
            //SystemException systemException = exception as SystemException;
            //if (httpException != null || systemException != null)
            //{
            //    string action;
            //    if (httpException != null)
            //    {
            //        switch (httpException.GetHttpCode())
            //        {
            //            case 404:
            //                // page not found
            //                action = "Error/NotFound";
            //                break;
            //            case 401:
            //                // page not found
            //                action = "Error/NotFound";
            //                break;
            //            case 500:
            //                // server error

            //                action = "Error/BadRequest";
            //                break;
            //            default:
            //                action = "Error/BadRequest";
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        action = "Error/BadRequest";
            //    }
            //    // clear error on server
            //    Server.ClearError();
            //   // Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["PageURL"] + action + "?returnUrl=" + (Request.UrlReferrer == null ? System.Configuration.ConfigurationManager.AppSettings["PageURL"].ToString() : Request.UrlReferrer.ToString()));
            //}
        }

    }
}