using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tutorial_4_Hybrid_Tier.Controllers
{
    /// <summary>
    /// Purpose: To display each associated page
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class MainController : Controller
    {
        /// <summary>
        /// Display tutorial 5 index page
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Tutorial5Index()
        {
            ViewBag.Message = "Gangemi's Bank";
            return View();
        }

        /// <summary>
        /// Display users page
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Users()
        {
            return View();
        }

        /// <summary>
        /// display accounts page
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Accounts()
        {
            ViewBag.Message = "Your Accounts Page";

            return View();
        }
        /// <summary>
        /// Display transaction page
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Transactions()
        {
            ViewBag.Message = "Your Transactions Page.";

            return View();
        }
    }
}