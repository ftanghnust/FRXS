using System.Web;
using System.Web.Mvc;
using FRXS.Website.Common;

namespace FRXS.Website
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new UserAuthAttribute());//注册
        }
    }
}