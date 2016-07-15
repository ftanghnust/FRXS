using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FRXS.Website.Common
{
    public class SiteConfig
    {
        private static SiteConfig _config;
        public static SiteConfig Config
        {
            get
            {
                if(_config==null)
                {
                    _config = new SiteConfig();
                }
                return _config;
            }
        }

        private string _sitename = "长沙血液中心机采交通费发放系统";           //网站名称
        private string _coname = "长沙血液中心";                 //公司名称
        private string _siteurl = "http://www.frxx.com";             //网站网址
        private string _icp = "湘ICP88888";                          //备案编号

        /// <summary>
        /// 网站名称
        /// </summary>
        public string SiteName
        {
            get { return _sitename; }
            set { _sitename = value; }
        }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string COName
        {
            get { return _coname; }
            set { _coname = value; }
        }

        /// <summary>
        /// 网站网址
        /// </summary>
        public string SiteUrl
        {
            get { return _siteurl; }
            set { _siteurl = value; }
        }

        /// <summary>
        /// 备案编号
        /// </summary>
        public string Icp
        {
            get { return _icp; }
            set { _icp = value; }
        }
    }
}