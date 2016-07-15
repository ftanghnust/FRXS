using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FRXS.Model;

namespace FRXS.Website.Controllers
{
    public class ControlController : Controller
    {
        /// <summary>
        /// 可折叠控件
        /// </summary>
        /// <returns></returns>
        public ActionResult Accordion()
        {
            return View();
        }

        /// <summary>
        /// 下拉组合框
        /// </summary>
        /// <returns></returns>
        public ActionResult Combobox()
        {
            return View();
        }

        /// <summary>
        /// Combobox测试数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetComboboxData()
        {
            var q = Request["q"]??"";
            using (var db = new FRXSEntities())
            {
                var orgUser = db.OrgUser.Where(p => true);

                var list = string.IsNullOrEmpty(q) ? orgUser.ToList() : orgUser.ToList().Where(p => p.UserName.Contains(q));
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Dialog对话框
        /// </summary>
        /// <returns></returns>
        public ActionResult Dialog()
        {
            return View();
        }

        /// <summary>
        /// Messager 提示框
        /// </summary>
        /// <returns></returns>
        public ActionResult Messager()
        {
            return View();
        }
        
        /// <summary>
        /// Tree 树形控件
        /// </summary>
        /// <returns></returns>
        public ActionResult Tree()
        {
            return View();
        }

        /// <summary>
        /// Combotree 组合数
        /// </summary>
        /// <returns></returns>
        public ActionResult Combotree()
        {
            return View();
        }
        
        /// <summary>
        /// Slider 滑动条
        /// </summary>
        /// <returns></returns>
        public ActionResult Slider()
        {
            return View();
        }

        /// <summary>
        /// Tabs 选项卡
        /// </summary>
        /// <returns></returns>
        public ActionResult Tabs()
        {
            return View();
        }

        /// <summary>
        /// Input 文本框
        /// </summary>
        /// <returns></returns>
        public ActionResult Input()
        {
            return View();
        }
        
        /// <summary>
        /// Treegrid 树形表格
        /// </summary>
        /// <returns></returns>
        public ActionResult Treegrid()
        {
            return View();
        }

    }
}
