﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    public class AdminController : ApiController
    {
        // GET api/<controller>
        [Route("api/Save")]
        [HttpPost]
        public void Save()
        {
            Bank.bankData.SaveToDisk();
        }

        [Route("api/ProcessTransactions")]
        [HttpPost]
        public bool processTransactions()
        {
            try
            {
                Bank.bankData.ProcessAllTransactions();
                return true;
            }
            catch
            {
                Debug.WriteLine("Unable to process transactions");
                return false;
            }
        }
    }
}