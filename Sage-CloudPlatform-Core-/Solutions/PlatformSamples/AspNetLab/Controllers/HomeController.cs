using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetLab.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Home for reusable components.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Tell us we screwed up.";

            return View();
        }
    }
}