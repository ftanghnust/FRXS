using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FRXS.Common
{
    public class CookieHelper
    {
        /// <summary>
        /// 单实例操作
        /// </summary>
        private static CookieHelper _cookie;
        public static CookieHelper Cookie
        {
            get
            {
                if(_cookie==null)
                {
                    _cookie=new CookieHelper();
                }
                return _cookie;
            }
        }

        /// <summary>
        /// 登录cookie名称
        /// </summary>
        public const string LoginCookieName = "LoginName";

        /// <summary>
        /// 真实姓名cookie名称
        /// </summary>
        public const string TrueCookieName = "TrueName";

        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptCookieName = "DeptName";

        /// <summary>
        /// 验证码cookie名称
        /// </summary>
        public const string VerifyCodeCookieName = "VerifyCode";

        /// <summary>
        /// 区域cookie名称
        /// </summary>
        public const string RegionCookieName = "Region";


        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            cookie.Expires = DateTime.Now.AddMinutes(120);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 读Cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        public string GetCookie(string strName)
        {
            return HttpContext.Current.Request.Cookies[strName] != null ? HttpContext.Current.Request.Cookies[strName].Value : "";
        }

        /// <summary>
        /// 销毁Cookie
        /// </summary>
        /// <param name="strName">名称</param>
        public void RemoveCookie(string strName)
        {
            var httpCookie = HttpContext.Current.Request.Cookies[strName];
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.AppendCookie(httpCookie);
            }
        }


        /// <summary>
        /// 延长Cookie时间
        /// </summary>
        /// <param name="strName">名称</param>
        public void ExtendCookie(string strName)
        {
            var httpCookie = HttpContext.Current.Request.Cookies[strName];
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddMinutes(20);
                HttpContext.Current.Response.AppendCookie(httpCookie);
            }
        }
    }
}
