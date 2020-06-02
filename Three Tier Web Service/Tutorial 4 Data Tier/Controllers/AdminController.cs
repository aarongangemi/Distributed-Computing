using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial_4_Data_Tier.Models;

namespace Tutorial_4_Data_Tier.Controllers
{
    /// <summary>
    /// Purpose: The admin controller stores features that users should be completed automatically in the background.
    /// Author: Aaron Gangemi
    /// Date Modified: 24/05/2020
    /// </summary>
    public class AdminController : ApiController
    {
        // GET api/<controller>
        /// <summary>
        /// Purpose: To save the bank data to disk
        /// </summary>
        [Route("api/Save")]
        [HttpPost]
        public void Save()
        {
            Bank.bankData.SaveToDisk();
        }

        /// <summary>
        /// Purpose: to process all transactions currently in queue
        /// </summary>
        /// <returns>if transaction was successfully processed</returns>
        [Route("api/ProcessTransactions")]
        [HttpPost]
        public bool processTransactions()
        {
            try
            {
                Bank.bankData.ProcessAllTransactions();
                // Return true if transaction was successful
                return true;
            }
            catch(Exception)
            {
                // return false if exception occured
                return false;
            }
        }
    }
}