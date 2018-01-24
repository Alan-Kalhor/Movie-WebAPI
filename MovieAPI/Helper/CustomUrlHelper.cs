using System.Web.Http.Routing;

namespace MovieAPI.Helper
{
    public class CustomUrlHelper: IUrlHelper
    {
        public UrlHelper Url { get; set; }

        public string GetLink(string routeName, object routeValues)
        {
            return Url.Link(routeName, routeValues);
        }
    }
}
