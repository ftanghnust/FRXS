using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FRXS.Model;
using Newtonsoft.Json.Linq;

namespace FRXS.Website.Controllers
{
    public class OrgUserController : Controller
    {
        //
        // GET: /OrgUser/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取列表-根据条件
        /// </summary>
        /// <returns></returns>
        public ActionResult GetList()
        {
            var pageIndex = Request["page"] == null ? 1 : int.Parse(Request["page"]);
            var pageSize = Request["rows"] == null ? 10 : int.Parse(Request["rows"]);
            var userName = Request["txtUserName"];
            var userTrueName = Request["txtUserTrueName"];
            var dept = Request["txtDept"];

            using (var db = new FRXSEntities())
            {
                var orgUser = db.OrgUser.Where(p => true);
                //where 条件
                if (!string.IsNullOrEmpty(userName))
                {
                    orgUser = orgUser.Where(p => p.UserName.Contains(userName));
                }
                if (!string.IsNullOrEmpty(userTrueName))
                {
                    orgUser = orgUser.Where(p => p.UserTrueName.Contains(userTrueName));
                }
                if (!string.IsNullOrEmpty(dept))
                {
                    orgUser = orgUser.Where(p => p.Dept.Contains(dept));
                }

                //总数
                var sum = orgUser.Count();
                //排序及分页
                orgUser = orgUser.OrderBy(p => p.UserId).Skip((pageIndex - 1) * pageSize).Take(pageSize);

                var data = new
                {
                    total = sum,
                    rows = orgUser.ToList()
                };


                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 添加/更新OrgUser
        /// </summary>
        /// <param name="orgUser"></param>
        /// <returns></returns>
        public ActionResult SaveOrgUser(OrgUser orgUser)
        {
            int result;
            using (var db = new FRXSEntities())
            {
                //UserId>0更新
                if (orgUser.UserId > 0)
                {
                    //获取修改前的实体对象
                    var dborgUser = db.OrgUser.FirstOrDefault(p => p.UserId == orgUser.UserId);
                    if (dborgUser != null)
                    {
                        dborgUser.UserName = orgUser.UserName;
                        dborgUser.UserTrueName = orgUser.UserTrueName;
                        dborgUser.Dept = orgUser.Dept;
                        dborgUser.Password = orgUser.Password;
                    }
                }
                else
                {
                    var dborgUser = db.OrgUser.FirstOrDefault(p => p.UserName == orgUser.UserName);
                    if (dborgUser != null) {
                        //return Content(result > 0 ? "success" : "error");
                    }
                    //添加
                    db.OrgUser.Add(orgUser);
                }
                result = db.SaveChanges();
            }

            return Content(result > 0 ? "success" : "error");
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult DeletebyIds(string ids)
        {
            int result;
            using (var db = new FRXSEntities())
            {
                var arr = ids.Split(',');
                foreach (var id in arr)
                {
                    var i = int.Parse(id);
                    var entity = db.OrgUser.FirstOrDefault(p => p.UserId == i);
                    db.OrgUser.Remove(entity);
                }
                result = db.SaveChanges();
            }
            return Content(result > 0 ? "success" : "error");
        }


        /// <summary>
        /// 获取单个实体对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetOrgUser(int id)
        {
            using (var db = new FRXSEntities())
            {
                var entity = db.OrgUser.FirstOrDefault(p => p.UserId == id);
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
