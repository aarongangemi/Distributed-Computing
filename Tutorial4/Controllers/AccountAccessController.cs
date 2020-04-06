using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BankDB;

namespace Tutorial4.Controllers
{
    public class AccountAccessController : Controller
    {
        // GET: AccountAccess
        public ActionResult Index()
        {
            return View();
        }
    }
}