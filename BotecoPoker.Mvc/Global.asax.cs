using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BotecoPoker.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var culture = new System.Globalization.CultureInfo("pt-BR");
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat = culture.DateTimeFormat;
            System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat = culture.DateTimeFormat;


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
