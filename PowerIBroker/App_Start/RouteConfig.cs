using System.Web.Mvc;
using System.Web.Routing;
namespace PowerIBroker
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
               name: "TermsConditions",
               url: "Terms-Conditions",
               defaults: new { controller = "Terms_Conditions", action = "Index" }
                );
            routes.MapRoute(
              name: "PrivacyPolicy",
              url: "Privacy-Policy",
              defaults: new { controller = "Privacy_Policy", action = "Index" }
                );
            routes.MapRoute(
            name: "Contact",
            url: "contact",
            defaults: new { controller = "Contact", action = "Index" }
                );
            routes.MapRoute(
            name: "ContactUs",
            url: "ContactUs",
            defaults: new { controller = "Contact", action = "GotoHome" }
                );
            routes.MapRoute(
            name: "Controller",
            url: "Super-Admin/{action}/{id}",
            defaults: new { controller = "TempAdmin", action = "Index", id = UrlParameter.Optional }
                );
            routes.MapRoute(
            name: "BrokerAdmin",
            url: "Broker-Admin/{action}/{id}",
            defaults: new { controller = "TempBroker", action = "Index", id = UrlParameter.Optional }
                );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "PowerIBroker.Controllers" }
                );
        }
    }
}