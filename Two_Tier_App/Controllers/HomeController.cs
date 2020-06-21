using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Two_Tier_App.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Comments()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}