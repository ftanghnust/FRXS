using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FRXS.Model;
using Newtonsoft.Json.Linq;
using FRXS.Common;

namespace FRXS.Website.Controllers
{
    public class OrgUserController : Controller
    {
        #region 视图
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UpdatePWD()
        {
            return View();
        }

        #endregion

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
            string result = string.Empty;
            try
            {
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
                            dborgUser.ModifyTime = DateTime.Now;
                        }
                    }
                    else
                    {
                        var dborgUser = db.OrgUser.FirstOrDefault(p => p.UserName == orgUser.UserName);
                        if (dborgUser != null)
                        {
                            return Content(new ResultData
                            {
                                Flag = "FAIL",
                                Info = string.Format("数据库中存在登录用户名为【{0}】的记录", orgUser.UserName)
                            }.ToJsonString());

                        }
                        orgUser.CreateTime = DateTime.Now;
                        orgUser.Password = DEncrypt.DEncryptOpt.Md5Stirng("123456");   //默认密码为 123456
                        //添加
                        db.OrgUser.Add(orgUser);
                    }
                    db.SaveChanges();

                    result = new ResultData
                    {
                        Flag = "SUCCESS",
                        Info = "OK"
                    }.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                result = new ResultData
                {
                    Flag = "EXCEPTION",
                    Info = string.Format("出现异常：{0}", ex.Message)
                }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult DeletebyId(string id)
        {
            string result = string.Empty;
            try
            {
                using (var db = new FRXSEntities())
                {
                    var i = int.Parse(id);
                    var entity = db.OrgUser.FirstOrDefault(p => p.UserId == i);
                    if (entity != null)
                    {
                        if (entity.UserName == "admin")
                        {
                            return Content(new ResultData
                            {
                                Flag = "FAIL",
                                Info = string.Format("不能删除管理员帐号【{0}】", entity.UserName)
                            }.ToJsonString());
                        }
                    }
                    else
                    {
                        return Content(new ResultData
                        {
                            Flag = "FAIL",
                            Info = "数据库中不存在该帐号信息"
                        }.ToJsonString());
                    }
                    db.OrgUser.Remove(entity);
                    db.SaveChanges();

                    result = new ResultData
                    {
                        Flag = "SUCCESS",
                        Info = "OK"
                    }.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                result = new ResultData
                {
                    Flag = "EXCEPTION",
                    Info = string.Format("出现异常：{0}", ex.Message)
                }.ToJsonString();
            }
            return Content(result);
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

        /// <summary>
        /// 删除 批量
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
        /// 更新帐号密码
        /// </summary>
        /// <param name="OldPwd"></param>
        /// <param name="NewPwd"></param>
        /// <param name="NewPwd1"></param>
        /// <returns></returns>
        public ActionResult UpdatePwdHandle(string OldPwd, string NewPwd, string NewPwd1)
        {
            if (NewPwd != NewPwd1)
            {
                return Content(new ResultData
                {
                    Flag = "FAIL",
                    Info = "两次输入的密码不一致"
                }.ToJsonString());
            }
            string result = string.Empty;
            try
            {
                var userName = CookieHelper.Cookie.GetCookie(CookieHelper.LoginCookieName);
                using (var db = new FRXSEntities())
                {
                    var user = db.OrgUser.FirstOrDefault(p => p.UserName == userName.Trim());
                    if (user != null && user.Password == DEncrypt.DEncryptOpt.Md5Stirng(OldPwd.Trim()))
                    {

                        user.Password = DEncrypt.DEncryptOpt.Md5Stirng(NewPwd.Trim());

                        db.SaveChanges();

                        result = new ResultData
                        {
                            Flag = "SUCCESS",
                            Info = "OK"
                        }.ToJsonString();
                    }
                    else {
                        result = new ResultData
                        {
                            Flag = "FAIL",
                            Info = "原始密码错误"
                        }.ToJsonString();
                    }
                }
            }
            catch (Exception ex)
            {
                result = new ResultData
                {
                    Flag = "EXCEPTION",
                    Info = string.Format("出现异常：{0}", ex.Message)
                }.ToJsonString();
            }
            return Content(result);
        }

    }
}
