using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tutorial4.Models;

namespace Tutorial4.Controllers
{
    public class UserController : ApiController
    {
        // GET api/<controller>

        BankDB.UserAccessInterface userAccess = Bank.bankData.GetUserAccess();

        // GET api/<controller>/5
        [Route("api/User")]
        [HttpPost]
        public UserDetailsStruct createUser()
        {
            
            UserDetailsStruct uds = new UserDetailsStruct();
            uds.userId = userAccess.CreateUser();
            userAccess.SelectUser(uds.userId);
            userAccess.SetUserName("John", "Smith");
            userAccess.GetUserName(out uds.firstName, out uds.lastName);
            return uds;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
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