using System.Web;
using System.Web.Mvc;
using WebApi.JwtServices;

namespace WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
           // filters.Add(new jwtMiddlewareAttribute());
        }
    }
}
