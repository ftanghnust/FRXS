using FRXS.Common;
using FRXS.Common.Excel;
using FRXS.Model;
using FRXS.Website.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FRXS.Website.Controllers
{
    public class TrafficFeeController : Controller
    {
        #region 视图
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddOrEdit()
        {
            return View();
        }

        public ActionResult Query()
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
            var sort = Request["sort"];
            var order = Request["order"];
            var name = Request["txtName"];
            var idCard = Request["txtIDCard"];
            var outReason = Request["OutReason"];
            var collectionNum = Request["CollectionNum"];
            var startDate = Request["StartDate"];
            var endDate = Request["EndDate"];
            var bz1 = Request["BZ1"];
            var type = Request["Type"];


            using (var db = new FRXSEntities())
            {
                var trafficFee = db.TrafficFee.Where(p => true);
                //where 条件
                if (!string.IsNullOrEmpty(name))
                {
                    trafficFee = trafficFee.Where(p => p.Name.Contains(name));
                }
                if (!string.IsNullOrEmpty(idCard))
                {
                    trafficFee = trafficFee.Where(p => p.IDCard.Contains(idCard));
                }
                if (!string.IsNullOrEmpty(outReason))
                {
                    trafficFee = trafficFee.Where(p => p.OutReason == outReason);
                }
                if (!string.IsNullOrEmpty(collectionNum))
                {
                    trafficFee = trafficFee.Where(p => p.CollectionNum == collectionNum);
                }

                if (!string.IsNullOrEmpty(startDate))
                {
                    DateTime StartDate = DateTime.Parse(startDate);
                    trafficFee = trafficFee.Where(p => p.CreateTime >= StartDate);
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    DateTime EndDate = DateTime.Parse(endDate).AddDays(1);
                    trafficFee = trafficFee.Where(p => p.CreateTime < EndDate);
                }

                if (!string.IsNullOrEmpty(bz1))
                {
                    trafficFee = trafficFee.Where(p => p.BZ1.Contains(bz1));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    if (type == "Add")   //新增，只显示当前天的记录
                    {
                        DateTime AddStartDate = DateTime.Now.Date;
                        DateTime AddEndDate = DateTime.Now.AddDays(1).Date;
                        trafficFee = trafficFee.Where(p => p.CreateTime >= AddStartDate);
                        trafficFee = trafficFee.Where(p => p.CreateTime < AddEndDate);
                    }
                }

                //总数
                var sum = trafficFee.Count();

                //排序及分页
                if (sort == "CreateTime" && order == "desc")
                {
                    trafficFee = trafficFee.OrderByDescending(p => p.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                }
                else if (sort == "CreateTime" && order == "asc")
                {
                    trafficFee = trafficFee.OrderBy(p => p.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                }
                else if (sort == "IDCard" && order == "desc")
                {
                    trafficFee = trafficFee.OrderByDescending(p => p.IDCard).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                }
                else if (sort == "IDCard" && order == "asc")
                {
                    trafficFee = trafficFee.OrderBy(p => p.IDCard).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                }


                var data = new
                {
                    total = sum,
                    rows = trafficFee.ToList()
                };

                return Content(data.ToJsonString());
                //return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 添加/更新TrafficFee
        /// </summary>
        /// <param name="orgUser"></param>
        /// <returns></returns>
        public ActionResult SaveTrafficFee(TrafficFee trafficFee)
        {
            string result = string.Empty;
            try
            {
                using (var db = new FRXSEntities())
                {
                    if (trafficFee.ID > 0)
                    {
                        //获取修改前的实体对象
                        var dbtrafficFee = db.TrafficFee.FirstOrDefault(p => p.ID == trafficFee.ID);
                        if (dbtrafficFee != null)
                        {
                            dbtrafficFee.IDCard = trafficFee.IDCard;
                            dbtrafficFee.Name = trafficFee.Name;
                            dbtrafficFee.AccountName = trafficFee.AccountName;
                            dbtrafficFee.BankAccount = trafficFee.BankAccount;
                            dbtrafficFee.BankName = trafficFee.BankName;
                            dbtrafficFee.IsPass = trafficFee.IsPass;
                            dbtrafficFee.OutReason = trafficFee.OutReason;
                            dbtrafficFee.CollectionNum = trafficFee.CollectionNum;
                            dbtrafficFee.Fee = trafficFee.Fee;
                            dbtrafficFee.ModifyTime = DateTime.Now;
                            dbtrafficFee.BZ1 = trafficFee.BZ1;  //手机号码
                        }
                    }
                    else
                    {
                        //添加
                        trafficFee.CreateTime = DateTime.Now;
                        trafficFee.WorkMan = CookieHelper.Cookie.GetCookie(CookieHelper.LoginCookieName);
                        db.TrafficFee.Add(trafficFee);
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
                    var entity = db.TrafficFee.FirstOrDefault(p => p.ID == i);
                    if (entity == null)
                    {
                        return Content(new ResultData
                        {
                            Flag = "FAIL",
                            Info = "数据库中不存在该费用信息"
                        }.ToJsonString());
                    }

                    db.TrafficFee.Remove(entity);
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
        public ActionResult GetTrafficFee(int id)
        {
            using (var db = new FRXSEntities())
            {
                var entity = db.TrafficFee.FirstOrDefault(p => p.ID == id);
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 自动补齐 接口
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public ActionResult GetTrafficFeeAutoComplete(string idcard, string q)
        {
            using (var db = new FRXSEntities())
            {
                var trafficFee = db.TrafficFee.Where(p => true);
                //where 条件
                if (!string.IsNullOrEmpty(idcard))
                {
                    trafficFee = trafficFee.Where(p => p.IDCard.Contains(idcard));
                }
                var result = trafficFee.GroupBy(p => p.IDCard).Take(10).Select(g => new
                {
                    IDCard = g.Max(item => item.IDCard),
                    Name = g.Max(item => item.Name),
                    AccountName = g.Max(item => item.AccountName),
                    BankAccount = g.Max(item => item.BankAccount),
                    BankName = g.Max(item => item.BankName),
                    BZ1 = g.Max(item => item.BZ1)
                });
                return Content(result.ToJsonString());
            }
        }

        /// <summary>
        /// 导出excel表格
        /// </summary>
        /// <returns></returns>
        public ActionResult DataExport()
        {
            IList<TrafficFeeModel> detailsmodel = new List<TrafficFeeModel>();
            var pageIndex = 1;
            var pageSize = 100000000;
            //var sort = "CreateTime";
            //var order = "desc";
            var name = Request["txtName"];
            var idCard = Request["txtIDCard"];
            var outReason = Request["OutReason"];
            var collectionNum = Request["CollectionNum"];
            var startDate = Request["StartDate"];
            var endDate = Request["EndDate"];
            var bz1 = Request["BZ1"];
            var type = "Query";


            using (var db = new FRXSEntities())
            {
                var trafficFee = db.TrafficFee.Where(p => true);
                //where 条件
                if (!string.IsNullOrEmpty(name))
                {
                    trafficFee = trafficFee.Where(p => p.Name.Contains(name));
                }
                if (!string.IsNullOrEmpty(idCard))
                {
                    trafficFee = trafficFee.Where(p => p.IDCard.Contains(idCard));
                }
                if (!string.IsNullOrEmpty(outReason))
                {
                    trafficFee = trafficFee.Where(p => p.OutReason == outReason);
                }
                if (!string.IsNullOrEmpty(collectionNum))
                {
                    trafficFee = trafficFee.Where(p => p.CollectionNum == collectionNum);
                }

                if (!string.IsNullOrEmpty(startDate))
                {
                    DateTime StartDate = DateTime.Parse(startDate);
                    trafficFee = trafficFee.Where(p => p.CreateTime >= StartDate);
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    DateTime EndDate = DateTime.Parse(endDate).AddDays(1);
                    trafficFee = trafficFee.Where(p => p.CreateTime < EndDate);
                }

                if (!string.IsNullOrEmpty(bz1))
                {
                    trafficFee = trafficFee.Where(p => p.BZ1.Contains(bz1));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    if (type == "Add")   //新增，只显示当前天的记录
                    {
                        DateTime AddStartDate = DateTime.Now.Date;
                        DateTime AddEndDate = DateTime.Now.AddDays(1).Date;
                        trafficFee = trafficFee.Where(p => p.CreateTime >= AddStartDate);
                        trafficFee = trafficFee.Where(p => p.CreateTime < AddEndDate);
                    }
                }

                //总数
                //var sum = trafficFee.Count();

                //排序及分页
                trafficFee = trafficFee.OrderByDescending(p => p.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);

                var temp = trafficFee.ToList();
                for (int i = 0; i < temp.Count; i++)
                {
                    TrafficFeeModel tempmodel = new TrafficFeeModel();
                    tempmodel.Name = temp[i].Name;
                    tempmodel.IDCard = temp[i].IDCard;
                    tempmodel.OutReason = temp[i].OutReason;
                    tempmodel.CollectionNum = temp[i].CollectionNum;
                    tempmodel.Fee = temp[i].Fee;
                    tempmodel.WorkMan = temp[i].WorkMan;
                    tempmodel.AccountName = temp[i].AccountName;
                    tempmodel.BankAccount = temp[i].BankAccount;
                    tempmodel.BankName = temp[i].BankName;
                    tempmodel.BZ1 = temp[i].BZ1;
                    tempmodel.CreateTime = temp[i].CreateTime;
                    detailsmodel.Add(tempmodel);
                }
                int maxRows = 5000;
                if (detailsmodel.Count > 0)
                {
                    string fileName = "机采交通费导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls"; //DateTime.Now.ToString("yyyyMMddHHmmssfff")
                    byte[] byteArr = NpoiExcelhelper.ExportExcel
                        (
                            detailsmodel,
                            maxRows,
                            Server.MapPath("UploadFile"),
                            fileName
                        );

                    return File(byteArr, "application/vnd.ms-excel", fileName);
                }
                return Content("数据不存在可能已被删除，不能导出！");
            }
        }
    }
}
