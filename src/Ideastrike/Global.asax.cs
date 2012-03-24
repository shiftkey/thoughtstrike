using System.Configuration;
using System.Data.Entity.Migrations;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Ideastrike.Migrations;
using Ideastrike.Models;
using Ideastrike.Models.Repositories;

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
                name: "TopItems",
                url: "top",
                defaults: new { controller = "Home", action = "Top" }
            );

            routes.MapRoute(
                name: "NewItems",
                url: "new",
                defaults: new { controller = "Home", action = "New" }
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

            MigrateDatabase();
        }

        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());


            if (ConfigurationManager.ConnectionStrings.Count > 0 && ConfigurationManager.ConnectionStrings["Ideastrike"] != null)
                builder.RegisterType<IdeastrikeContext>()
                    .WithParameter(new NamedParameter("nameOrConnectionString", ConfigurationManager.ConnectionStrings["Ideastrike"].ConnectionString + ";MultipleActiveResultSets=true"))
                    .AsSelf()
                    .InstancePerLifetimeScope();

            else
                builder.RegisterType<IdeastrikeContext>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<IdeaRepository>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<ActivityRepository>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<FeatureRepository>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<SettingsRepository>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<UserRepository>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<ImageRepository>()
                .AsImplementedInterfaces()
                .SingleInstance();

            return builder.Build();
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
            topCssFiles.AddFile("~/Content/jquery.fileupload-ui.css");
            BundleTable.Bundles.Add(topCssFiles);

            Bundle bottomJsFiles = new Bundle("~/scripts/bottom", new JsMinify());
            bottomJsFiles.AddFile("~/Scripts/jquery-1.7.1.min.js");
            bottomJsFiles.AddFile("~/Scripts/jquery.validate.min.js");
            bottomJsFiles.AddFile("~/Scripts/jquery.fancybox.pack.js");
            bottomJsFiles.AddFile("~/Scripts/jquery.contra.min.js");
            bottomJsFiles.AddFile("~/Scripts/showdown.js");
            bottomJsFiles.AddFile("~/Scripts/ideastrike.js");
            bottomJsFiles.AddFile("~/Scripts/bootstrap-alerts.js");
            bottomJsFiles.AddFile("~/Scripts/bootstrap-dropdown.js");
            bottomJsFiles.AddFile("~/Scripts/social.js");
            bottomJsFiles.AddFile("~/Scripts/mustache.js");
            BundleTable.Bundles.Add(bottomJsFiles);
        }

        private static void MigrateDatabase()
        {
            var settings = new IdeastrikeDbConfiguration();
            var migrator = new DbMigrator(settings);
            migrator.Update();
        }
    }
}