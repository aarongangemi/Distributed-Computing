using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial4.Models;

namespace Tutorial4BusinessTier.Controllers
{
    public class BankApiController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [Route("api/BankApi/Save")]
        [HttpPost]
        public void SaveData()
        {
            Bank.bankData.SaveToDisk();
        }

        // POST api/<controller>
        [Route("api/BankApi/ProcessTransactions")]
        [HttpGet]
        public void ProcessTransactions()
        {
            Bank.bankData.ProcessAllTransactions();
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}