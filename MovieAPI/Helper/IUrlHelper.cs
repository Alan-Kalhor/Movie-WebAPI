using System.Web.Http.Routing;

namespace MovieAPI.Helper
{
    public interface IUrlHelper
    {
        string GetLink(string routeName, object routeValues);
        UrlHelper Url { get; set; }
    }
}
