using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using MusicServiceComparison.Controllers;
using MusicServiceComparison.MusicGatherer;
using MusicServiceComparison.MusicGatherer.Service;
using MusicServiceComparison.Repository;

namespace MusicServiceComparison
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ConfigureContainer();            
        }

        private void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterType<HomeController>().InstancePerRequest();
            builder.RegisterType<ArtistController>().InstancePerRequest();
            builder.RegisterType<SearchController>().InstancePerRequest();

            builder.RegisterType<LiteDBArtistRepository>().As<IArtistRepository>();
            
            builder.RegisterType<Gatherer>().As<IMusicGatherer>();

            builder.RegisterInstance(new SpotifyService());
            builder.RegisterInstance(new ITunesService());
            builder.RegisterInstance(new DeezerService());

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
        
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();
        }
    }
}
