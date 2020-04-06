using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BankDB;
using Tutorial4.Models;

namespace Tutorial4.Controllers
{
    public class TransactionAccessController : Controller
    {
        // GET: TransactionAccess
        public ActionResult Index()
        {
            return View();
        }
    }
}