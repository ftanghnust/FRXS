using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FRXS.Model;
using Newtonsoft.Json.Linq;

namespace FRXS.Website.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Test()
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
            var productName = Request["txtProductName"];

            using (var db = new FRXSEntities())
            {
                var product = db.Product.Where(p => true);
                //where 条件
                if (!string.IsNullOrEmpty(productName))
                {
                    product = product.Where(p => p.ProductName.Contains(productName));
                }

                //总数
                var sum = product.Count();
                //排序及分页
                product = product.OrderBy(p => p.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize);

                var data = new
                {
                    total = sum,
                    rows = product.ToList()
                };

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 添加/更新Product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public ActionResult SaveProduct(Product product)
        {
            int result;
            using (var db = new FRXSEntities())
            {
                //Id>0更新
                if (product.Id > 0)
                {
                    //获取修改前的实体对象
                    var dbproduct = db.Product.FirstOrDefault(p => p.Id == product.Id);
                    if (dbproduct != null)
                    {
                        dbproduct.ProductName = product.ProductName;

                    }
                }
                else
                {
                    //添加
                    db.Product.Add(product);
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
                    var entity = db.Product.FirstOrDefault(p => p.Id == i);
                    db.Product.Remove(entity);
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
        public ActionResult GetProduct(int id)
        {
            using (var db = new FRXSEntities())
            {
                var entity = db.Product.FirstOrDefault(p => p.Id == id);
                return Json(entity, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
