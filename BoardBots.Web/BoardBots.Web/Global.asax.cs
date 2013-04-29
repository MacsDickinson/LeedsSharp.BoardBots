using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BoardBots.Web.Infrastructure;
using Raven.Client;
using Raven.Client.Document;

namespace BoardBots.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        static IDocumentStore DocumentStore { get; set; }

        public MvcApplication()
        {
            BeginRequest += (source, args) =>
            {
                HttpContext.Current.Items["RavenSession"] = DocumentStore.OpenSession();
            };

            EndRequest += (source, args) =>
            {
                using (var session = HttpContext.Current.Items["RavenSession"] as IDocumentSession)
                {
                    if (session != null && Server.GetLastError() == null)
                    {
                        session.SaveChanges();
                    }
                }
            };
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            if (DocumentStore == null)
            {
                DocumentStore = new DocumentStore
                {
                    ConnectionStringName = "RavenHQ"
                }.Initialize();

                DocumentStore.RegisterConventions();
            }
        }
    }
}