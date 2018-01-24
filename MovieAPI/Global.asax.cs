using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using Unity.Lifetime;
using MovieAPI.Controllers;
using MovieAPI.Helper;
using MovieAPI.Caching;
using MovieAPI.Repository;

namespace MovieAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ConfigureApi(GlobalConfiguration.Configuration);
        }

        void ConfigureApi(HttpConfiguration config)
        {
            var unity = new UnityContainer();
            unity.RegisterType<MoviesController>();
            unity.RegisterType<IMovieRepository, MovieRepository>(
                new HierarchicalLifetimeManager());
            unity.RegisterType<IMovieCacheStorage, MovieCacheStorage>(
                new HierarchicalLifetimeManager());
            unity.RegisterType<IUrlHelper, CustomUrlHelper>(
                 new HierarchicalLifetimeManager());
            config.DependencyResolver = new IoCContainer(unity);
        }

    }
}
