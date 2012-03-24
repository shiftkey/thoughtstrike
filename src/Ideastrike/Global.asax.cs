using System.Collections.Generic;
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
            AddJavascriptBundle(
                "~/scripts/top",
                new[]
                    {
                        "~/Scripts/modernizr-2.5.3.js",
                        "~/Scripts/respond.min.js"
                    });

            AddCssBundle(
                "~/content/top",
                new[]
                    {
                        "~/Content/bootstrap.min.css",
                        "~/Content/media.css",
                        "~/Content/style.css",
                        "~/Content/ideastrike.css",
                        "~/Content/jquery.fancybox.css",
                        "~/Content/jquery.fileupload-ui.css"
                    });

            AddJavascriptBundle(
                "~/scripts/bottom",
                new[]
                    {
                        "~/Scripts/jquery-1.7.1.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.fancybox.pack.js",
                        "~/Scripts/jquery.contra.min.js",
                        "~/Scripts/showdown.js",
                        "~/Scripts/ideastrike.js",
                        "~/Scripts/bootstrap-alerts.js",
                        "~/Scripts/bootstrap-dropdown.js",
                        "~/Scripts/social.js",
                        "~/Scripts/mustache.js"
                    });

            AddJavascriptBundle(
                "~/scripts/home",
                new[]
                    {
                        "~/Scripts/modules/home.js"
                    });
        }

        private static void AddJavascriptBundle(string path, IEnumerable<string> files)
        {
            Bundle bundle = new Bundle(path, new JsMinify());
            foreach (var f in files)
                bundle.AddFile(f);
            AddBundle(bundle);
        }

        private static void AddCssBundle(string path, IEnumerable<string> files)
        {
            Bundle bundle = new Bundle(path, new CssMinify());
            foreach (var f in files)
                bundle.AddFile(f);
            AddBundle(bundle);
        }


        private static void AddBundle(Bundle bundle)
        {
            BundleTable.Bundles.Add(bundle);
        }

        private static void MigrateDatabase()
        {
            var settings = new IdeastrikeDbConfiguration();
            var migrator = new DbMigrator(settings);
            migrator.Update();
        }
    }
}