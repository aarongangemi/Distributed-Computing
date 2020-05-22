using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XSS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult CreateProfile()
        {
            return View();
        }

        public ActionResult TellAdmin()
        {

            return View();
        }

        public ActionResult Feedback()
        {
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
    }
}