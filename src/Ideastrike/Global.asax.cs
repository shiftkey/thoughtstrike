using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;

namespace Ideastrike
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            AddBundles();

            var container = CreateContainer();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }

        private IContainer CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterControllers(Assembly.GetExecutingAssembly());

            // TODO: other dependencies

            return containerBuilder.Build();
        }

        private static void AddBundles()
        {
            BundleTable.Bundles.RegisterTemplateBundles();

            Bundle topJsFiles = new Bundle("~/scripts/top", new JsMinify());
            topJsFiles.AddFile("~/Scripts/modernizr-2.5.3.js");
            topJsFiles.AddFile("~/Scripts/respond.min.js");

            Bundle topCssFiles = new Bundle("~/content/top", new CssMinify());
            topCssFiles.AddFile("~/Content/bootstrap.min.css");
            topCssFiles.AddFile("~/Content/media.css");
            topCssFiles.AddFile("~/Content/style.css");
            topCssFiles.AddFile("~/Content/ideastrike.css");
            topCssFiles.AddFile("~/Content/jquery.fancybox.css");
            BundleTable.Bundles.Add(topCssFiles);

            Bundle bottomJsFiles = new Bundle("~/scripts/bottom", new JsMinify());
            bottomJsFiles.AddFile("~/Scripts/jquery-1.7.1.min.js");
            bottomJsFiles.AddFile("~/Scripts/jquery.validate.min.js");
            bottomJsFiles.AddFile("~/Scripts/jquery.fancybox.pack.js");
            bottomJsFiles.AddFile("~/Scripts/showdown.js");
            bottomJsFiles.AddFile("~/Scripts/ideastrike.js");
            bottomJsFiles.AddFile("~/Scripts/bootstrap-alerts.js");
            bottomJsFiles.AddFile("~/Scripts/bootstrap-dropdown.js");
            bottomJsFiles.AddFile("~/Scripts/social.js");
            bottomJsFiles.AddFile("~/Scripts/mustache.js");
            BundleTable.Bundles.Add(bottomJsFiles);
        }
    }
}