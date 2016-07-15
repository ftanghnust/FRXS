using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FRXS.Website.Controllers
{
    public class TrafficFeeController : Controller
    {
        //
        // GET: /TrafficFee/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddOrEdit()
        {
            return View();
        }

    }
}
