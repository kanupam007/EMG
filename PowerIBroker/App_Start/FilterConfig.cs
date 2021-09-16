using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PowerIBroker
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

    public class sessionexpireattribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filtercontext)
        {
            HttpContext ctx = HttpContext.Current;
            // check  sessions here
            if (HttpContext.Current.Session["AdminID"] == null)
            {
                filtercontext.Result = new RedirectResult("~/Admin/Index");
                return;
            }
            base.OnActionExecuting(filtercontext);
        }

    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null || filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            {

               // filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "Index", area = "", ReturnUrl = filterContext.HttpContext.Request.Url.ToString() }));
                UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
                string loginurl = urlHelper.Action("Index", "Home", new { area = "", ReturnUrl = filterContext.HttpContext.Request.Url.ToString() });
                filterContext.Result = new RedirectResult(loginurl);
             
            }
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class RedirectIfAuthenticate : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
            string accountselection_url = urlHelper.Action("AccountSelection", "Account").ToLower();
            HttpContextBase Context = filterContext.RequestContext.HttpContext;
            HttpRequestBase Request = filterContext.HttpContext.Request;
            if (Context.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult(accountselection_url);
            }
        }
    }


    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class BrokerSessionattribute : ActionFilterAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (object.Equals(HttpContext.Current.Session["BrokerAdmin"], null))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.HttpContext.Response.End();
                }
                else
                {

                    UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
                   string loginurl = urlHelper.Action("Index", "Home", new { area = "", ReturnUrl = filterContext.HttpContext.Request.Url.ToString() });
                   // string loginurl = urlHelper.Action("Index", "Home");
                    HttpContextBase Context = filterContext.RequestContext.HttpContext;
                    HttpRequestBase Request = filterContext.HttpContext.Request;
                    HttpContext ctx = HttpContext.Current;

                    if (HttpContext.Current.Session["SubBrokerId"] == null || HttpContext.Current.Session["BrokerAdmin"] == null)
                    {
                        filterContext.Result = new RedirectResult(loginurl);
                    }
                }


            }


        }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class BrokerUserSessionattribute : ActionFilterAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (object.Equals(HttpContext.Current.Session["BrokerUser"], null))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.HttpContext.Response.End();
                }
                else
                {

                    UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
                    string loginurl = urlHelper.Action("Index", "Home", new { area = "", ReturnUrl = filterContext.HttpContext.Request.Url.ToString() });
                    // string loginurl = urlHelper.Action("Index", "Home");
                    HttpContextBase Context = filterContext.RequestContext.HttpContext;
                    HttpRequestBase Request = filterContext.HttpContext.Request;
                    HttpContext ctx = HttpContext.Current;

                    if (HttpContext.Current.Session["SubBrokerId"] == null|| HttpContext.Current.Session["BrokerUser"]==null)
                    {
                        filterContext.Result = new RedirectResult(loginurl);
                    }
                }


            }


        }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class GASessionattribute : ActionFilterAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (object.Equals(HttpContext.Current.Session["GeneralAgent"], null))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.StatusCode = 401;
                    filterContext.HttpContext.Response.End();
                }
                else
                {

                    UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
                    string loginurl = urlHelper.Action("Index", "Home", new { area = "", ReturnUrl = filterContext.HttpContext.Request.Url.ToString() });
                    // string loginurl = urlHelper.Action("Index", "Home");
                    HttpContextBase Context = filterContext.RequestContext.HttpContext;
                    HttpRequestBase Request = filterContext.HttpContext.Request;
                    HttpContext ctx = HttpContext.Current;

                    if (HttpContext.Current.Session["GeneralAgentId"] == null)
                    {
                        filterContext.Result = new RedirectResult(loginurl);
                    }
                }


            }


        }
    }



}