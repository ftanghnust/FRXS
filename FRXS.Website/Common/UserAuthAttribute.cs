using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using FRXS.Common;

namespace FRXS.Website.Common
{
    public class UserAuthAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }

            if (string.IsNullOrEmpty(CookieHelper.Cookie.GetCookie(CookieHelper.LoginCookieName)))
            {
                if (filterContext.HttpContext.Request.Url != null)
                {
                    string fromUrl = filterContext.HttpContext.Request.Url.AbsolutePath;
                    string redirectUrl = string.Format("?returl={0}", fromUrl);
                    //string loginUrl = new UrlHelper(filterContext.RequestContext).Action("Login", "Home") + redirectUrl;
                    //跳转到登录页
                    string loginUrl = new UrlHelper(filterContext.RequestContext).Action("Login", "Home");
                    filterContext.HttpContext.Response.Write("<script>parent.location.href='" + loginUrl + "';</script>");
                    filterContext.HttpContext.Response.End();
                }
            }
            else
            {
                //延长Cookie时间
                CookieHelper.Cookie.ExtendCookie(CookieHelper.LoginCookieName);
            }
        }
    }
}