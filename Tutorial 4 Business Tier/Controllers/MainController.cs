using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tutorial_4_Business_Tier.Controllers
{
    public class MainController : Controller
    {
        public ActionResult Users()
        {
            return View();
        }

        public ActionResult Accounts()
        {
            ViewBag.Message = "Your Accounts Page";

            return View();
        }

        public ActionResult Transactions()
        {
            ViewBag.Message = "Your Transactions Page.";

            return View();
        }
    }
}