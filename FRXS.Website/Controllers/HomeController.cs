using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FRXS.Common;
using FRXS.Model;
using System.Text;
using FRXS.Common.Log;

namespace FRXS.Website.Controllers
{
    public class HomeController : Controller
    {
        #region 视图
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var userName = CookieHelper.Cookie.GetCookie(CookieHelper.LoginCookieName);

            using (var db = new FRXSEntities())
            {
                var user = db.OrgUser.FirstOrDefault(p => p.UserName == userName);
                if (user != null)
                {
                    ViewBag.UserName = user.UserName;
                    ViewBag.UserTrueName = user.UserTrueName;
                }
            }

            return View();
        }

        /// <summary>
        /// 选择区域
        /// </summary>
        /// <returns></returns>
        public ActionResult SelRegion()
        {
            return View();
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        public ActionResult Test()
        {
            return View();
        }

        /// <summary>
        /// 登录页
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 登录提交
        /// </summary>
        /// <param name="passWord"></param>
        /// <param name="verifyCode"></param>
        /// <param name="adminName"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string adminName, string passWord, string verifyCode)
        {
            //验证验证码
            var verifyCodeCookieValue = CookieHelper.Cookie.GetCookie(CookieHelper.VerifyCodeCookieName);
            if (verifyCode.ToLower() != DEncrypt.DEncryptOpt.Decode(verifyCodeCookieValue, DEncrypt.VerifyCodeKey).ToLower())
            {
                ViewBag.ErrorImage = "<img src='../Content/images/warning.gif' align='absmiddle' style='padding-right: 3px;' />";
                ViewBag.ErrorMessage = "验证码错误";
                return View();
            }

            using (var db = new FRXSEntities())
            {
                var user = db.OrgUser.FirstOrDefault(p => p.UserName == adminName.Trim());
                if (user != null && user.Password == DEncrypt.DEncryptOpt.Md5Stirng(passWord.Trim()))
                {
                    //匹配成功保持Cookie
                    CookieHelper.Cookie.WriteCookie(CookieHelper.LoginCookieName, adminName);

                    CookieHelper.Cookie.WriteCookie(CookieHelper.TrueCookieName, HttpUtility.UrlEncode(user.UserTrueName, Encoding.GetEncoding("UTF-8")));

                    CookieHelper.Cookie.WriteCookie(CookieHelper.DeptCookieName, HttpUtility.UrlEncode(user.Dept, Encoding.GetEncoding("UTF-8")));

                    if (user.Dept == "机采科")
                    {
                        return RedirectToAction("SelRegion");
                    }

                    return RedirectToAction("Index");
                }

                ViewBag.ErrorImage = "<img src='../Content/images/warning.gif' align='absmiddle' style='padding-right: 3px;' />";
                ViewBag.ErrorMessage = "无效的用户信息";
                return View();
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            //清除Cookie
            CookieHelper.Cookie.RemoveCookie(CookieHelper.LoginCookieName);
            //清除Cookie
            CookieHelper.Cookie.RemoveCookie(CookieHelper.DeptCookieName);
            //清除Cookie
            CookieHelper.Cookie.RemoveCookie(CookieHelper.VerifyCodeCookieName);
            //清除Cookie
            CookieHelper.Cookie.RemoveCookie(CookieHelper.RegionCookieName);

            return RedirectToAction("Login");
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult VerifyCode()
        {
            Random rand = new Random();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string randomcode = FRXS.Common.RandomHelper.CreateRandomCode(4, FRXS.Common.RandomHelper.RandomCodeType.NumberAndLetters);

            //记录Cookie
            CookieHelper.Cookie.WriteCookie(CookieHelper.VerifyCodeCookieName, DEncrypt.DEncryptOpt.Encode(randomcode, DEncrypt.VerifyCodeKey));

            // 随机转动角度
            int randAngle = 45;
            int mapwidth = (int)(randomcode.Length * 20);
            // 创建图片背景
            Bitmap map = new Bitmap(mapwidth - 3, 27);
            Graphics graph = Graphics.FromImage(map);
            // 清除画面，填充背景
            graph.Clear(Color.AliceBlue);
            // 画一个边框
            graph.DrawRectangle(new Pen(Color.BurlyWood, 0), 0, 0, map.Width - 1, map.Height - 2);
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; //模式



            // 背景噪点生成
            Pen blackPen = new Pen(Color.LightGray, 0);
            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(0, map.Width);
                int y = rand.Next(0, map.Height);
                graph.DrawRectangle(blackPen, x, y, 1, 1);
            }

            // 验证码旋转，防止机器识别
            // 拆散字符串成单字符数组
            char[] chars = randomcode.ToCharArray();

            // 文字距中
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            // 定义颜色
            Color[] c =
                {
                    Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Brown, Color.DarkCyan, Color.Purple,
                    Color.DarkGreen
                };
            // 定义字体
            //string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial","Lucida Sans Unicode", Rockwell, Batang ,Times New Roman,Bernard MT Condensed};

            for (int i = 0; i < chars.Length; i++)
            {
                int cindex = rand.Next(7);
                int findex = rand.Next(4);

                // 字体样式(参数2为字体大小)
                Font f = new Font("Microsoft Sans Serif", 17, FontStyle.Bold);
                Brush b = new SolidBrush(c[cindex]);

                Point dot = new Point(14, 11);
                // 测试X坐标显示间距的
                //graph.DrawString(dot.X.ToString(),fontstyle,new SolidBrush(Color.Black),10,150);
                // 转动的度数
                float angle = rand.Next(-randAngle, randAngle);

                // 移动光标到指定位置
                graph.TranslateTransform(dot.X, dot.Y);
                graph.RotateTransform(angle);
                graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                //graph.DrawString(chars[i].ToString(),fontstyle,new SolidBrush(Color.Blue),1,1,format);
                // 转回去
                graph.RotateTransform(-angle);
                // 移动光标到指定位置
                graph.TranslateTransform(2, -dot.Y);
            }
            // 标准随机码
            //graph.DrawString(randomcode,fontstyle,new SolidBrush(Color.Blue),2,2); 

            // 生成图片
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

            Response.ClearContent();
            Response.ContentType = "image/gif";
            Response.BinaryWrite(ms.ToArray());

            graph.Dispose();
            map.Dispose();
            return null;
        }



        //根据权限获取菜单
        public ActionResult GetMenu()
        {
            string loginName =  CookieHelper.Cookie.GetCookie(CookieHelper.LoginCookieName);
            string deptName = HttpUtility.UrlDecode(CookieHelper.Cookie.GetCookie(CookieHelper.DeptCookieName), Encoding.GetEncoding("UTF-8")); 
            List<Node> roots = new List<Node>();

            List<Node> childrens1 = new List<Node>();
            Node temp3 = new Node()
            {
                menuid = "dcfee5be-651e-4aac-8968-ce127e457454",
                icon = "icon-add",
                menuname = "新增",
                url = "TrafficFee/Index"
            };
            Node temp4 = new Node()
            {
                menuid = "24ea7f2f-33c3-4e0d-8faa-7a114e4567b1",
                icon = "icon-search",
                menuname = "查询",
                url = "TrafficFee/Query"
            };
            if (deptName != "财务科")
            {
                childrens1.Add(temp3);
            }
            if (deptName != "机采科" || loginName.ToLower() == "y024" || loginName.ToLower() == "y088")  //对于部分机采科部分帐号可以查收的，需修改此处条件 ！！！
            {
                childrens1.Add(temp4);
            }
            Node temp1 = new Node()
            {
                menuid = "1",
                icon = "icon-role",
                menuname = "机采交通费",
                menus = childrens1
            };

            //Logger.GetInstance().DebugLog
            //(
            //    new NormalLog
            //    {
            //        LogTime = DateTime.Now,
            //        LogContent = string.Format("loginName：{0}，menus：{1}", loginName, childrens1.ToJson())
            //    }
            //);

            List<Node> childrens2 = new List<Node>();
            Node temp5 = new Node()
            {
                menuid = "dcfee5be-651e-4aac-8968-ce127e457454",
                icon = "icon-add",
                menuname = "用户管理",
                url = "orguser/index"
            };
            childrens2.Add(temp5);
            Node temp2 = new Node()
            {
                menuid = "2",
                icon = "icon-set",
                menuname = "系统管理",
                menus = childrens2
            };
            roots.Add(temp1);
            if (deptName != "财务科" && deptName != "机采科")
            {
                roots.Add(temp2);
            }
            string result = roots.ToJsonString();
            return Content(result);
        }

        /// <summary>
        /// 菜单对象
        /// </summary>
        private class Node
        {
            /// <summary>
            /// 
            /// </summary>
            public string menuid { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string icon { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string menuname { get; set; }

            public string url { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public List<Node> menus { get; set; }

        }
    }
}
